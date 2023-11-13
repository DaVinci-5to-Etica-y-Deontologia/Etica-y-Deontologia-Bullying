using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpInstigador : PopUpComment
{
    protected override bool ExecutePopUp => !player.Moderator;

    protected override void PopUp(CommentView commentView)
    {
        base.PopUp(commentView);


    }
}
