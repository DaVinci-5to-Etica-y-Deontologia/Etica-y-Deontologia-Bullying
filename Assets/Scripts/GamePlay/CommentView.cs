using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CommentView : MonoBehaviour, IPoolElement<CommentView>
{
    [SerializeField]
    TextMeshProUGUI textMesh;

    [SerializeField]
    Image perfil;

    [SerializeField]
    Button button;

    CommentData _commentData;

    public LinkedPool<CommentView> Parent { get; set; }

    public IPoolElement<CommentView> Next { get; set; }

    public bool inPool { get; set; }

    public CommentData commentData
    {
        get => _commentData;

        set
        {
            _commentData = value;

            if (_commentData == null)
                return;

            perfil.sprite = _commentData.perfil;

            textMesh.text = _commentData.textComment;

            _commentData.onDestroy += Destroy;
        }
    }

    public CommentView Create()
    {
        var aux = StreamerManager.SpawnComment();
        aux.SetActiveGameObject(false);
        return aux;
    }

    public void Destroy()
    {
        _commentData = null;
        this.SetActiveGameObject(false);
        Parent.Return(this);
    }

    public void OnClick()
    {
        _commentData.OnClick();
    }


    public void AnimRefresh()
    {
    }


    private void OnDestroy()
    {
        if(_commentData!=null)
            _commentData.onDestroy -= Destroy;
    }

}

[System.Serializable]
public class CommentData : IDirection, IPoolElement<CommentData>
{
    public int ID;

    [SerializeReference]
    public Comment comment;

    public float timeOnCreate;

    public float Delay => comment.Deley;

    public string textComment => comment.Text;

    public bool Enable => user.Enable;

    public string textIP => $"{user.textIP}.{ID}";

    public string textName => user.Name.RichText("color", "#" + ColorUtility.ToHtmlStringRGBA(user.colorText));

    public Sprite perfil => user.Perfil;

    EventManager eventManager => user.eventManager;

    public LinkedPool<CommentData> Parent { get ; set ; }
    public IPoolElement<CommentData> Next { get; set ; }
    public bool inPool { get ; set ; }

    User user;

    public event System.Action onDestroy;

    public void OnClick()
    {
        eventManager.events.SearchOrCreate<EventParam<CommentData>>("onclickcomment").delegato.Invoke(this);
    }

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
        this.user = StreamerManager.instance[idStream]?[idUser];
    }
}

public interface IDirection
{
    public string textIP { get; }
}

