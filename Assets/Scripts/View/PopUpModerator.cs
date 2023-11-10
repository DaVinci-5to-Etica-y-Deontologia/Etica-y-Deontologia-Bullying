using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpModerator : PopUpElement
{
    [SerializeField]
    TMPro.TextMeshProUGUI textToShow;

    public override void Awake()
    {
        base.Awake();
        eventManager.events.SearchOrCreate<EventParam<CommentData>>("onclickcomment").delegato += PopUp;
    }

    void PopUp(CommentData comment)
    {
        onActive.Invoke();

        textToShow.text = $"Usuario: {comment.textName}\nComentario: {comment.textComment}";

        callsManager.DestroyAll();

        if (comment.Enable)
            callsManager.Create("Ban" , () => StreamerManager.Execute(Actions.Ban, comment.textIP));

        
        callsManager.Create("Admonition", () => StreamerManager.Execute(Actions.Admonition, comment.textIP));

        //callsManager.Create("Eliminate", () => eliminate.delegato.Invoke(comment));
    }
}
