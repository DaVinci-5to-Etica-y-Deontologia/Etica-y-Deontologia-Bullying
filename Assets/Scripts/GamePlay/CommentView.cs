using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CommentView : MonoBehaviour
{
    Comment comment;

    [SerializeField]
    TextMeshProUGUI textMesh;

    [SerializeField]
    Image perfil;

    [SerializeField]
    ContentSizeFitter contain;

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
}
