using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpComment : PopUp
{
    [SerializeField]
    TMPro.TextMeshProUGUI textToShow;

    private void Awake()
    {
        eventManager.events.SearchOrCreate<EventParam<CommentView>>("onclickcomment").delegato += PopUp;
    }

    void PopUp(CommentView comment)
    {
        onActive.Invoke();

        textToShow.text = $"Usuario: {comment.user.Name}\nComentario: {comment.comment.Text}";

        callsManager.DestroyAll();

        if (comment.user.Enable)
            callsManager.Create("Ban" , () => comment.user.Ban(comment));

        
        callsManager.Create("Admonition", () => comment.user.Admonition(comment));

        //callsManager.Create("Eliminate", () => eliminate.delegato.Invoke(comment));
    }
}
