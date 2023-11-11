using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommentData : IDirection, IPoolElement<CommentData>
{
    public int ID;

    [SerializeReference]
    public Comment comment;

    public float timeOnCreate;

    public User user => _user;

    public float Delay => comment.Deley;

    public string textComment => comment.Text;

    public bool Enable => user.Enable;

    public string textIP => $"{user.textIP}.{ID}";

    public string textName => user.Name.RichText("color", "#" + ColorUtility.ToHtmlStringRGBA(user.colorText));

    public BD database => user.database;

    public EventManager eventManager => user.eventManager;

    public LinkedPool<CommentData> Parent { get; set; }
    public IPoolElement<CommentData> Next { get; set; }
    public bool inPool { get; set; }


    public event System.Action onDestroy;

    User _user;

    public void Destroy()
    {
        onDestroy?.Invoke();
        Parent.Return(this);
    }

    public CommentData Create()
    {
        return new CommentData();
    }

    public void Create(int id, Comment comment)
    {
        this.ID = id;

        this.comment = comment;

        timeOnCreate = Time.realtimeSinceStartup;
    }

    public void Init(int idStream, int idUser)
    {
        _user = StreamerManager.instance[idStream].value?[idUser].value;
    }
}

public interface IDirection
{
    public string textIP { get; }
}
