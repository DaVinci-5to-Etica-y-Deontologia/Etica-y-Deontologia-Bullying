using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpModerator : PopUpComment
{
    [SerializeField]
    TMPro.TMP_Dropdown dropdown;

    protected override bool ExecutePopUp => player.Moderator;

    protected override void PopUp(CommentView commentView)
    {
        base.PopUp(commentView);

        dropdown.value = user.Suspect;

        callsManager.Create("Ban" , () =>
        {
            Execute();
            DataRpc.Create(Actions.Ban, comment.textIP); 
        });

        
        callsManager.Create("Admonition", () =>
        {
            Execute();
            DataRpc.Create(Actions.Admonition, comment.textIP);
        });
    }

    public void DropDown(int index)
    {
        if(user.comments.Count > 0)
            DataRpc.Create(Actions.Suspect, comment.textIP, index.ToString());
    }
}
