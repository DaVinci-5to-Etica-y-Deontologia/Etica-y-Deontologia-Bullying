using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpFinalPartida : PopUpElement
{
    [SerializeField]
    TextMeshProUGUI titleTMP;

    [SerializeField]
    TextMeshProUGUI summaryTMP;

    [SerializeField]
    Transform container;

    [SerializeField]
    SummaryItem summaryItem;

    [SerializeField]
    Color victoryColor;

    [SerializeField]
    Color defeatColor;

    [SerializeField]
    Color drawColor;

    public override void MyAwake(PopUpManager popUpManager)
    {
        base.MyAwake(popUpManager);
        eventManager.events.SearchOrCreate<EventParam>("finishday").delegato += PopUp;
    }

    void PopUp()
    {
        onActive.Invoke();
        callsManager.DestroyAll();
        MakeSummary();

        callsManager.Create("Volver a jugar", () =>
        {
            ScenesLoader.instance.ReloadCurrentScene();
            TransitionManager.instance.SetTransition(TransitionManager.SquaresStart);
        });

        callsManager.Create("Volver al menú", () =>
        {
            ScenesLoader.instance.LoadScene("MainMenu");
            TransitionManager.instance.SetTransition(TransitionManager.SquaresStart);
            //Destruir sesion
        });

        var result = StreamerManager.instance.MatchResults();

        if (result == 0)
        {
            titleTMP.text = "Empate";
            summaryTMP.text = "Ningun equipo consigió superar al otro\n\n Empate técnico";
            return;
        }

        if (player.Moderator)
        {
            if (result == 1)
            {
                titleTMP.text = "Victoria";
                summaryTMP.text = "Lograste cazar a los haters y evitar que el odio destruya a los streamers\n\n ¡Felicidades eres un gran moderador!";
            }
            else
            {
                titleTMP.text = "Derrota";
                summaryTMP.text = "Los haters lograron su objetivo, el odio destruyó a los streamers\n\n Has fracasado como moderador";
            }
        }
        else
        {
            if (result == 2)
            {
                titleTMP.text = "Victoria";
                summaryTMP.text = "Superaste a los moderadores y corrompiste a los usuarios\n\n ¡Felicidades destruiste a los streamers!";
            }
            else
            {
                titleTMP.text = "Derrota";
                summaryTMP.text = "No corrompiste a los suficientes usuarios para vencer a los moderadores\n\n Has fracasado como instigador";
            }
        }
    }

    void MakeSummary()
    {
        foreach (var item in StreamerManager.instance.FinishedStreams())
        {
            var summItem = Instantiate(summaryItem, container);
            (string, Color) data;

            if (!item.defeat)
            {
                data.Item1 = "Vivo";
                data.Item2 = victoryColor;
            }
            else
            {
                data.Item1 = "Muerto";
                data.Item2 = defeatColor;
            }

            summItem.SetItem(data.Item1, item.streamer.iconStreamerImage, data.Item2);
        }
        //Streams Empatados
    }

}
