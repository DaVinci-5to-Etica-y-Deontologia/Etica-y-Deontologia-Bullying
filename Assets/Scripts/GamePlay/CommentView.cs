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

    public Pool<CommentView> Parent { get; set; }

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
public class CommentData : IDirection
{
    public int ID;

    User user;

    public float time;

    public string textComment => comment.Text;

    public bool Enable => user.Enable;

    public string textIP => $"{user.textIP}.{ID}";

    public string textName => user.Name.RichText("color", "#" + ColorUtility.ToHtmlStringRGBA(user.colorText));

    public Sprite perfil => user.Perfil;

    EventManager eventManager => user.eventManager;

    [SerializeReference]
    public Comment comment;

    public event System.Action onDestroy;

    public void OnClick()
    {
        eventManager.events.SearchOrCreate<EventParam<CommentData>>("onclickcomment").delegato.Invoke(this);
    }
    public void Create(int id, Comment comment)
    {
        this.ID = id;

        this.comment = comment;

        time = Time.realtimeSinceStartup;
    }

    public void Init(int idStream, int idUser)
    {
        this.user = StreamerManager.instance[idStream]?[idUser];
    }

    public void Destroy()
    {
        onDestroy?.Invoke();
    }
}

public interface IDirection
{
    public string textIP { get; }
}

