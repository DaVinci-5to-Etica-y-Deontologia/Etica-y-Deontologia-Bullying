using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpModerator : PopUpElement
{
    [SerializeField]
    TMPro.TextMeshProUGUI userName;

    [SerializeField]
    TMPro.TextMeshProUGUI textToShow;

    [SerializeField]
    Transform placeToCreate;

    CommentView commentView;
    public override void MyAwake(PopUpManager popUpManager)
    {
        base.MyAwake(popUpManager);
        eventManager.events.SearchOrCreate<EventParam<CommentView>>("onclickcomment").delegato += PopUp;
        eventManager.events.SearchOrCreate<EventParam<CommentData>>("leavecomment").delegato += OnLeaveComment;
    }

    void OnLeaveComment(CommentData commentData)
    {
        if(commentView.commentData == commentData)
        {
            callsManager.DestroyAll();
            textToShow.text = "No hay posibles acciones".RichText("color","red");
        }
            
    }

    void PopUp(CommentView commentView)
    {
        this.commentView = commentView;

        var comment = commentView.commentData;

        callsManager.DestroyAll();

        foreach (Transform item in placeToCreate)
        {
            Destroy(item.gameObject);
        }

        Instantiate(commentView, placeToCreate.position, Quaternion.identity, placeToCreate);

        onActive.Invoke();

        userName.text = $"Usuario: {comment.textName}";

        textToShow.text = "Posibles acciones:";

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
