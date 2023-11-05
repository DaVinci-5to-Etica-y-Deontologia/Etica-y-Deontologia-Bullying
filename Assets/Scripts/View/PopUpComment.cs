using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpComment : PopUp
{
    [SerializeField]
    TMPro.TextMeshProUGUI textToShow;

    EventParam<CommentView> ban;

    EventParam<CommentView> kick;

    EventParam<CommentView> eliminate;

    private void Awake()
    {
        eventManager.events.SearchOrCreate<EventParam<CommentView>>("onclickcomment").delegato += PopUp;

        ban = eventManager.events.SearchOrCreate<EventParam<CommentView>>("ban");

        kick = eventManager.events.SearchOrCreate<EventParam<CommentView>>("kick");

        eliminate = eventManager.events.SearchOrCreate<EventParam<CommentView>>("eliminate");

        ban.delegato += (c)=> onExecute.Invoke();

        kick.delegato += (c) => onExecute.Invoke();

        eliminate.delegato += (c) => onExecute.Invoke();
    }

    void PopUp(CommentView comment)
    {
        onActive.Invoke();

        textToShow.text = $"Usuario: {comment.comment.Parent.name}\nComentario: {comment.comment.Comentario}";

        callsManager.DestroyAll();

        if (!comment.comment.Parent.Ban)
            callsManager.Create("Ban" , ()=> ban.delegato.Invoke(comment));

        if (comment.comment.Parent.Enable)
            callsManager.Create("Kick", () => kick.delegato.Invoke(comment));

        callsManager.Create("Eliminate", () => eliminate.delegato.Invoke(comment));
    }
}
