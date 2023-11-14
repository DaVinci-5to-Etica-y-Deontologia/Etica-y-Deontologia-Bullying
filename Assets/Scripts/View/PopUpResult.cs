using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class PopUpResult : PopUpElement
{
    [SerializeField]
    TMPro.TextMeshProUGUI damageNumber;

    [SerializeField]
    TMPro.TextMeshProUGUI viwersNumber;

    [SerializeField]
    float timeToShow=1;

    public override void MyAwake(PopUpManager popUpManager)
    {
        base.MyAwake(popUpManager);
        eventManager.events.SearchOrCreate<EventParam<StreamerData>>("streamend").delegato += PopUp;

        eventManager.events.SearchOrCreate<EventParam<StreamerData>>("streamchange").delegato += 
            (s)=> 
            { 
                if(isActiveAndEnabled)
                    onExecute.Invoke(); 
            };
    }

    void PopUp(StreamerData resultado)
    {
        onActive.Invoke();
        callsManager.DestroyAll();

        callsManager.Create("Cambiar de stream", () =>
        {
            StreamerManager.instance.NextStreamer();
        });

        StartCoroutine(ViewerNumber(resultado));
    }

    IEnumerator ViewerNumber(StreamerData resultado)
    {
        damageNumber.text = string.Empty;

        viwersNumber.text = "0";

        for (int i = 1; i <= resultado.Viewers.current; i++)
        {
            yield return new WaitForSeconds(timeToShow / resultado.Viewers.current);

            viwersNumber.text = i.ToString();

            if(resultado.streamer.minimalViews >= resultado.Viewers.current)
            {
                viwersNumber.text = viwersNumber.text.RichTextColor(Color.red);
            }
            else if(resultado.Viewers.current< resultado.Viewers.total)
            {
                viwersNumber.text = viwersNumber.text.RichTextColor(Color.white);
            }
            else
            {
                viwersNumber.text = viwersNumber.text.RichTextColor(Color.green);
            }
        }

        damageNumber.text = resultado.Life.current.ToString();

        if (resultado.Life.current <= 0)
            damageNumber.text = damageNumber.text.RichText("color", "red");
        else
            damageNumber.text = damageNumber.text.RichText("color", "green");        
    }
}
