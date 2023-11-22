using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommentData : DataElement<CommentData>, IPoolElement<CommentData>
{
    public int commentID;

    public float timeOnCreate;

    public UserData user => _user;

    public float Delay => comment.Deley;

    public string textComment => comment.Text;

    public override bool Enable => user.Enable;

    public override string textIP => $"{user.textIP}.{ID}";

    public string textName => user.Name.RichTextColor(user.colorText);

    public Comment comment => dataBase.comments[commentID];

    public LinkedPool<CommentData> Parent { get; set; }
    public IPoolElement<CommentData> Next { get; set; }
    public bool inPool { get; set; }

    protected override IDataElement parent => _user;

    public event System.Action onDestroy;

    UserData _user;

    Timer timerDestroy;

    public void Destroy()
    {
        timerDestroy?.Stop();
        onDestroy?.Invoke();

        //Debug.Log("Tengo un padre cuando voy a destruirme? " + (Parent != null));

        Parent.Return(this);
    }

    public CommentData Create()
    {
        return new CommentData();
    }

    public void Init(int idStream, int idUser)
    {
        var stream = StreamerManager.instance[idStream].value;

        _user = stream?[idUser].value;

        if (IsServer)
            timerDestroy = TimersManager.Create(30, () =>
            {
                DataRpc.Create(Actions.Aplicate, textIP, (stream.Viewers.current + comment.Views , stream.Life.current + comment.Damage));
            }).SetMultiply(player.multiply);
    }

    public void Create(int id, int idComment)
    {
        this.ID = id;

        commentID = idComment;

        timeOnCreate = Time.realtimeSinceStartup;
    }
}


