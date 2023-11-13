using Euler;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase destinada a ser un user
/// </summary>
[System.Serializable]
public class User : IDirection
{
    static LinkedPool<CommentData> poolCommentData = new LinkedPool<CommentData>(new CommentData());

    public int ID;

    [field: SerializeField]
    public DataPic<CommentData> comments { get; private set; }

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

    public event System.Action<CommentData> onCreateComment;

    public event System.Action<CommentData> onLeaveComment;

    BD database => stream.dataBase;

    public EventManager eventManager => stream.eventManager;

    public string textIP => $"{stream.textIP}.{ID}";

    public float CoolDown { get=>_coolDown.current; set=> _coolDown.Set(value); }

    public CommentData this[int ID]
    {
        get
        {
            return comments.GetTByID(ID);
        }
    }

    int _Admonition
    {
        get => _admonition;
        set
        {
            if (_admonition + value <= 2)
            {
                _admonition += value;
            }
            else 
                Destroy();
        }
    }

    

    int _admonition;

    Timer _coolDown;

    Streamer stream;

    public override string ToString()
    {
        return Name + " " + comments.Count;
    }

    #region Moderator

    public void Admonition(int ID)
    {
        _admonition++;
        CoolDown = 30;
        LeaveComment(ID);
    }

    public void Ban(int ID)
    {
        Ban();
    }

    #endregion

    public void Aplicate(CommentData commentView)
    {
        //Debug.Log($"Aplicar el danio: {commentView.comment.Damage} ganancia de viewers: {commentView.comment.Views}");

        stream.Users(commentView.comment.Views);

        LeaveComment(commentView.ID);
    }


    public void Destroy()
    {
        _coolDown.Stop();
        Enable = false;
        stream.users.Remove(ID);
    }

    void Ban()
    {
        for (int i = comments.Count - 1; i >= 0; i--)
        {
            LeaveComment(comments.GetTByIndex(i).ID);
        }

        Destroy();
    }    

    public void CreateComment()
    {
        System.Action lambda = () => 
        {
            if (!Enable)
                return;

            var aux = database.SelectComment(MoralIndex, MoralRange);

            var newCommentData = poolCommentData.Obtain().Self;

            var auxPic = comments.Add(newCommentData);

            newCommentData.Create(auxPic.Key, aux);

            newCommentData.Init(stream.ID, ID);

            var timerDestroy = TimersManager.Create(30, () => Aplicate(newCommentData));

            newCommentData.onDestroy += () => timerDestroy.Stop();

            CoolDown = newCommentData.Delay;

            onCreateComment?.Invoke(newCommentData);
        };

        StreamerManager.eventQueue.Enqueue(lambda);
    }

    public void LeaveComment(int id)
    {
        var index = comments.GetIndexByID(id);

        var comment = comments.GetTByIndex(index);

        onLeaveComment?.Invoke(comment);

        comments.RemoveAt(index);

        comment.Destroy();
    }

    public void Init(Streamer stream)
    {
        this.stream = stream;

        comments = new();

        _coolDown = TimersManager.Create(Random.Range(10, 15), CreateComment);
    }

    public User(int id)
    {
        this.ID = id;

        int rng = Random.Range(5,8);

        string chars = "abcdefghijklmnñopqrstuvwxyz";

        for (int i = 0; i < rng; i++)
        {
            Name += chars[Random.Range(0, chars.Length)];
        }
    }
}
