using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpModerator : PopUpComment
{
    protected override bool ExecutePopUp => player.Moderator;

    protected override void PopUp(CommentView commentView)
    {
        base.PopUp(commentView);
        
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
}
