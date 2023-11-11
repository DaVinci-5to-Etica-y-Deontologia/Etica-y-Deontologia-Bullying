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

    public (CommentData value, int ID, int index) this[int ID]
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

    StreamerData stream;
    

    #region Moderator

    public void Admonition(int index)
    {
        _admonition++;
        CoolDown = 30;
        RemoveComment(index);
    }

    public void Ban()
    {
        for (int i = comments.Count - 1; i >= 0; i--)
        {
            RemoveComment(i);
        }

        Destroy();
    }

    #endregion

    #region Instigator
    
        //algo a futuro

    #endregion

    #region rpc
    public void AddComment(string json)
    {
        var newCommentData = poolCommentData.Obtain().Self;

        JsonUtility.FromJsonOverwrite(json, newCommentData);

        newCommentData.Init(stream.ID, ID);

        var auxPic = new Internal.Pictionary<int, CommentData>(newCommentData.ID, newCommentData);

        comments.Add(auxPic);

        onCreateComment?.Invoke(newCommentData);
    }

    public void RemoveComment(int index)
    {
        CommentData comment = comments.GetTByIndex(index).value;

        onLeaveComment?.Invoke(comment);

        comments.RemoveAt(index);

        comment.Destroy();

        if(!Enable && comments.Count==0)
            stream.users.Remove(ID);
    }

    #endregion

    public void Aplicate(int views, int damage ,string textIP)
    {
        //Debug.Log($"Aplicar el danio: {commentView.comment.Damage} ganancia de viewers: {commentView.comment.Views}");

        stream.Users(views);

        DataRpc.Create(Actions.RemoveComment, textIP);
    }


    public void Destroy()
    {
        _coolDown.Stop();
        Enable = false;
        //stream.users.Remove(ID);
    }

    public void CreateComment()
    {
        System.Action lambda = () => 
        {
            if (!Enable)
                return;

            var aux = database.SelectComment(MoralIndex, MoralRange);

            var newCommentData = poolCommentData.Obtain().Self;

            newCommentData.Create(comments.lastID + 1, aux);

            CoolDown = newCommentData.Delay;

            DataRpc.Create(Actions.AddComment, textIP, newCommentData);

            newCommentData.Destroy();
        };

        StreamerManager.eventQueue.Enqueue(lambda);
    }

    public override string ToString()
    {
        return Name + " " + comments.Count;
    }

    public void Init(StreamerData stream)
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
