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

        foreach (Transform item in container)
        {
            Destroy(item.gameObject);
        }

        /*
        callsManager.Create("Volver a jugar", () =>
        {
            ScenesLoader.instance.ReloadCurrentScene();
            TransitionManager.instance.SetTransition(TransitionManager.SquaresStart);
        });
        */

        callsManager.Create("Volver al menú", () => BackMenu() );

        var result = MakeSummary();

        if (result == StreamState.Empate)
        {
            titleTMP.text = "Empate";
            summaryTMP.text = "Ningun equipo consigió superar al otro\n\n Empate técnico";
            return;
        }

        if (player.Moderator)
        {
            if (result == StreamState.Completado)
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
            if (result == StreamState.Fallido)
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

    public void BackMenu()
    {
        ScenesLoader.GoToMenu();
        AudioManager.instance.Play("Click3");
    }

    StreamState MakeSummary()
    {
        int modsWins = 0;
        int instigatorsWins = 0;
        int draws = 0;

        foreach (var item in StreamerManager.instance.streamersData.streamers)
        {
            var summItem = Instantiate(summaryItem, container);
            (string, Color) data;

            if (item.Value.State == StreamState.Completado)
            {
                data.Item1 = "Vivo";
                data.Item2 = victoryColor;
                modsWins++;
            }
            else if (item.Value.State == StreamState.Fallido)
            {
                data.Item1 = "Muerto";
                data.Item2 = defeatColor;
                instigatorsWins++;
            }
            else
            {
                data.Item1 = "Empate";
                data.Item2 = drawColor;
                draws++;
            }

            summItem.SetItem(data.Item1, item.Value.streamerBase.iconStreamerImage, data.Item2);
        }

        if (modsWins == instigatorsWins)
            return StreamState.Empate;
        else if (modsWins > instigatorsWins)
            return StreamState.Completado;
        else
            return StreamState.Fallido;
    }
}

public enum StreamState
{
    Empate,
    Completado,
    Fallido
}