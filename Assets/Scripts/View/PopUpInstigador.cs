using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpInstigador : PopUpComment
{
    protected override bool ExecutePopUp => !player.Moderator;

    Timer cooldownFirstHability;

    Timer cooldownSecondHability;
    public override void MyAwake(PopUpManager popUpManager)
    {
        base.MyAwake(popUpManager);

        cooldownFirstHability = TimersManager.Create(player.InstigatorCooldownFirst, First);

        cooldownSecondHability = TimersManager.Create(player.InstigatorCooldownSecond, Second);
    }


    protected override void PopUp(CommentView commentView)
    {
        base.PopUp(commentView);

        First();

        Second();
    }

    void First()
    {
        if (cooldownFirstHability.Chck && isActiveAndEnabled)
            callsManager.Create("Corromper", () =>
            {
                Execute();
                cooldownFirstHability.Reset();
                DataRpc.Create(Actions.Corromper, comment.textIP);
            });
    }

    void Second()
    {
        if (cooldownSecondHability.Chck && isActiveAndEnabled)
            callsManager.Create("Picantear", () =>
            {
                Execute();
                cooldownSecondHability.Reset();
                DataRpc.Create(Actions.Picantear, comment.textIP);
            });
    }

}
