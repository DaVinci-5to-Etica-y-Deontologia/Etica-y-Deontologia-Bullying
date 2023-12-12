using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class LoadView : MonoBehaviour, IPointerClickHandler
{
    public bool endLoad;

    public UnityEngine.Events.UnityEvent onReadyAll;

    [SerializeField]
    TextMeshProUGUI tutorialText;

    [SerializeField]
    Image imageToShow;

    [SerializeField]
    BD dataBase;

    [SerializeField]
    Timer timerToChange;

    Tutos[] tutos => dataBase.Tutos;

    Pictionarys<string, bool> playersReady => StreamerManager.instance.streamersData.playersReady;

    EventManager eventManager => StreamerManager.instance.eventManager;

    Player player => StreamerManager.instance.player;

    System.Action MyUpdate;

    static bool ready;

    int index;

    public void EndLoad()
    {
        endLoad = true;

        timerToChange.Stop();
    }

    public void UpdateReady(bool b)
    {
        if (playersReady == null || playersReady.Count == 0)
            return;

        tutorialText.text = $"Presiona {"click".RichTextColor(Color.green)} si estas listo"+
          "\nListo: " + (ready ? "si".RichTextColor(Color.green) : "no".RichTextColor(Color.red)) +
          "\nJugadores listos: " + playersReady.Where((p) => p.Value).Count() + "/" + playersReady.Count;

        if (b)
            onReadyAll.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!endLoad)
        {
            ChangeTuto();
            timerToChange.Reset();
        }
        else
        {
            ready = !ready;
            DataRpc.Create(Actions.StartGame, "", player.ID.ToString());
        }
    }

    void ChangeTuto()
    {
        index++;

        if (index >= tutos.Length)
            index = 0;

        imageToShow.sprite = tutos[index].sprite;

        tutorialText.text = tutos[index].texts.ToString();
    }

    void VerifyEsc()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TransitionManager.instance.SetTransition(TransitionManager.SquaresStart);
            ScenesLoader.instance.LoadScene("MainMenu");
            StreamerManager.instance?.Runner?.Shutdown(false);
        }
    }

    private void Awake()
    {
        timerToChange = TimersManager.Create(6, ChangeTuto).SetLoop(true);

        eventManager.events.SearchOrCreate<EventParam<bool>>("allready").delegato += UpdateReady;

        ready = false;

        onReadyAll.AddListener(() => MyUpdate -= VerifyEsc);
    }

    private void Update()
    {
        MyUpdate?.Invoke();
    }

    private void OnEnable()
    {
        index = Random.Range(0, tutos.Length);

        ChangeTuto();

        timerToChange.Reset();

        MyUpdate += VerifyEsc;
    }
}

