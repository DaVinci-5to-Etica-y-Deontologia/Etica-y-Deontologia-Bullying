using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpComment : PopUp
{
    [SerializeField]
    TMPro.TextMeshProUGUI textToShow;
    /*
    EventParam<CommentView> ban;

    EventParam<CommentView> kick;

    EventParam<CommentView> eliminate;
    */

    private void Awake()
    {
        eventManager.events.SearchOrCreate<EventParam<CommentView>>("onclickcomment").delegato += PopUp;
        /*
        ban = eventManager.events.SearchOrCreate<EventParam<CommentView>>("ban");

        kick = eventManager.events.SearchOrCreate<EventParam<CommentView>>("kick");

        eliminate = eventManager.events.SearchOrCreate<EventParam<CommentView>>("eliminate");
        
        ban.delegato += (c)=> onExecute.Invoke();

        kick.delegato += (c) => onExecute.Invoke();

        eliminate.delegato += (c) => onExecute.Invoke();
        */
}

void PopUp(CommentView comment)
    {
        onActive.Invoke();

        textToShow.text = $"Usuario: {comment.user.Name}\nComentario: {comment.comment.Text}";

        callsManager.DestroyAll();

        if (comment.user.Enable)
            callsManager.Create("Ban" , ()=>comment.user.Ban(comment));

        
        callsManager.Create("Admonition", () => comment.user.Admonition(comment));

        //callsManager.Create("Eliminate", () => eliminate.delegato.Invoke(comment));
    }
}
