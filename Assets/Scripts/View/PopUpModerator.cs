using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpModerator : PopUpElement
{
    [SerializeField]
    TMPro.TextMeshProUGUI textToShow;

    [SerializeField]
    Transform placeToCreate;

    GameObject old;

    public override void MyAwake(PopUpManager popUpManager)
    {
        base.MyAwake(popUpManager);
        eventManager.events.SearchOrCreate<EventParam<CommentView>>("onclickcomment").delegato += PopUp;
    }

    void PopUp(CommentView commentView)
    {
        var comment = commentView.commentData;

        callsManager.DestroyAll();

        foreach (Transform item in placeToCreate)
        {
            Destroy(item.gameObject);
        }

        Instantiate(commentView, placeToCreate.position, Quaternion.identity, placeToCreate);

        onActive.Invoke();

        textToShow.text = $"Usuario: {comment.textName}";

        if (comment.Enable)
            callsManager.Create("Ban" , () =>
            {
                DataRpc.Create(Actions.Ban, comment.textIP); 
                Execute();
            });

        
        callsManager.Create("Admonition", () =>
        {
            DataRpc.Create(Actions.Admonition, comment.textIP);
            Execute();
        });

    }

    void Execute()
    {
        onExecute.Invoke();        
    }
}
