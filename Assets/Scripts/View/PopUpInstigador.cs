using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpInstigador : PopUpComment
{
    [SerializeField]
    Slider leftSlider;

    [SerializeField]
    Slider middleSlider;

    [SerializeField]
    Slider rightSlider;

    float leftSliderValue
    {
        set
        {
            leftSlider.value = Mathf.Clamp(value, 0, 1);
        }
    }

    float rightSliderValue
    {
        set
        {
            rightSlider.value = Mathf.Clamp(value, 0, 1);
        }
    }

    protected override bool ExecutePopUp => !player.Moderator;

    Timer cooldownFirstHability;

    Timer cooldownSecondHability;


    public override void MyAwake(PopUpManager popUpManager)
    {
        base.MyAwake(popUpManager);

        cooldownFirstHability = TimersManager.Create(player.InstigatorCooldownFirst, First);

        cooldownSecondHability = TimersManager.Create(player.InstigatorCooldownSecond, Second);

        onExecute.AddListener(() =>
        {
            user.onMoralIndexChange -= RefreshMoralIndex;
            user.onMoralRangeChange -= RefreshMoralRange;
        });
    }


    protected override void PopUp(CommentView commentView)
    {
        base.PopUp(commentView);

        First();

        Second();

        RefreshMoralIndex(user.MoralIndex);
        RefreshMoralRange(user.MoralRange);

        user.onMoralIndexChange += RefreshMoralIndex;
        user.onMoralRangeChange += RefreshMoralRange;
    }

    void First()
    {
        if (cooldownFirstHability.Chck && isActiveAndEnabled)
            callsManager.Create("Corromper", () =>
            {
                Execute();
                cooldownFirstHability.Reset();

                if (!comment.Enable)
                    return;
                
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

                if (!comment.Enable)
                    return;

                DataRpc.Create(Actions.Picantear, comment.textIP);
            });
    }

    void RefreshMoralIndex(float moralIndex)
    {
        middleSlider.value = moralIndex;
    }

    void RefreshMoralRange(float moralRange)
    {
        leftSliderValue = middleSlider.value - moralRange;
        rightSliderValue = middleSlider.value + moralRange;
    }

}
