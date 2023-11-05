using Euler;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase destinada a ser un user
/// </summary>
[System.Serializable]
public class User
{
    [field: SerializeField]
    public HashSet<CommentView> comments { get; private set; } = new();

    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public float MoralIndex { get; set; }

    [field: SerializeField]
    public float MoralRange { get; set; }

    [field: SerializeField]
    public bool Enable { get; set; } = true;

    [field: SerializeField]
    public Sprite Perfil { get; set; }

    [field: SerializeField]
    public Color colorText { get; set; } = new Color{ a=1 };

    BD database => stream.dataBase;

    public float CoolDown { get=>_coolDown.current; set=> _coolDown.Set(value); }

    int _Admonition
    {
        get => _admonition;
        set
        {
            if (_admonition + value <= 2)
            {
                _admonition = value;
            }
            else 
                Destroy();
        }
    }

    int _admonition;

    Timer _coolDown;

    ChatManager stream;

    public override string ToString()
    {
        return Name + " " + comments.Count;
    }

    #region Moderator

    public void Admonition(CommentView commentView)
    {
        _admonition++;
        CoolDown = 30;
        Eliminate(commentView);
    }

    public void Ban(CommentView commentView)
    {
        Ban();
    }

    #endregion

    public void Aplicate(CommentView commentView)
    {
        Debug.Log($"Aplicar el danio: {commentView.comment.Damage} ganancia de viewers: {commentView.comment.Views}");

        stream.Users(commentView.comment.Views);

        Eliminate(commentView);
    }

    public void Eliminate(CommentView commentView)
    {
        comments.Remove(commentView);

        stream.LeaveComment(commentView);
    }

    public void Destroy()
    {
        _coolDown.Stop();
        Enable = false;
        stream.users.Remove(this);
    }

    void Ban()
    {
        foreach (var item in comments)
        {
            stream.LeaveComment(item);
        }

        Destroy();
    }    

    void CreateComment()
    {
        var aux = database.SelectComment(MoralIndex, MoralRange);

        CoolDown = aux.Deley;

        comments.Add(stream.CreateComment(this, aux));
    }

    public User(ChatManager chat)
    {
        stream = chat;
        int rng = Random.Range(5,8);

        string chars = "abcdefghijklmnñopqrstuvwxyz";

        for (int i = 0; i < rng; i++)
        {
            Name += chars[Random.Range(0, chars.Length)];
        }
        
        _coolDown = TimersManager.Create(Random.Range(10, 15), CreateComment);
    }
}
