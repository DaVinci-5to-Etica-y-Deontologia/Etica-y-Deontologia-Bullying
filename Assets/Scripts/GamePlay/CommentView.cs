using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CommentView : MonoBehaviour
{
    [field: SerializeField]
    public Comment comment { get; private set; }

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

    public void Create(Comment comment, ContentSizeFitter contain)
    {
        this.contain = contain;

        this.comment = comment;

        perfil.sprite = comment.Parent.Perfil;

        textMesh.text = comment.Comentario.RichText("color", "#" + ColorUtility.ToHtmlStringRGBA(comment.Parent.colorText));
    }

    public void AnimRefresh()
    {
        contain.enabled = true;
    }

    public void OnClick()
    {
        eventManager.events.SearchOrCreate<EventParam<CommentView>>("onclickcomment").delegato.Invoke(this);
    }
}
