using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CommentView : MonoBehaviour
{
    public User user;

    [field: SerializeField]
    public Comment comment { get; private set; }

    public LinkedListNode<CommentView> node;

    [SerializeField]
    EventManager eventManager;

    [SerializeField]
    TextMeshProUGUI textMesh;

    [SerializeField]
    Image perfil;

    [SerializeField]
    ContentSizeFitter contain;

    [SerializeField]
    Button button;

    public void Create(User user,Comment comment, ContentSizeFitter contain)
    {
        this.user = user;

        this.contain = contain;

        this.comment = comment;

        perfil.sprite = user.Perfil;

        textMesh.text = comment.Text.RichText("color", "#" + ColorUtility.ToHtmlStringRGBA(user.colorText));
    }

    public void AnimRefresh()
    {
        contain.enabled = true;
    }

    public void Aplicate()
    {
        user.Aplicate(this);
    }

    public void OnClick()
    {
        eventManager.events.SearchOrCreate<EventParam<CommentView>>("onclickcomment").delegato.Invoke(this);
    }
}
