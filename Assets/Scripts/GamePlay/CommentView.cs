using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CommentView : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI textMesh;

    [SerializeField]
    Image perfil;

    [SerializeField]
    Button button;

    public CommentData commentData
    {
        get => _commentData;

        set
        {
            _commentData = value;

            perfil.sprite = _commentData.perfil;

            textMesh.text = _commentData.textComment;

            _commentData.onDestroy += _commentData_onDestroy;
        }

    }

    private void _commentData_onDestroy()
    {
        _commentData = null;
        Destroy(gameObject);
    }

    [SerializeField]
    CommentData _commentData;

    
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
            _commentData.onDestroy -= _commentData_onDestroy;
    }
}

[System.Serializable]
public class CommentData : IDirection
{
    public int ID;

    User user;

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