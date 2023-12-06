using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    StreamerManager streamerManager;

    [SerializeField]
    EventCall topButton;

    [SerializeField]
    EventCall bottomButton;

    [SerializeField]
    Image streamImage;

    [SerializeField]
    Image iconStreamerImage;

    [SerializeField]
    TextMeshProUGUI title;

    [SerializeField]
    TextMeshProUGUI nameStreamer;

    [SerializeField]
    TextMeshProUGUI tagsStreamer;

    [SerializeField]
    TextMeshProUGUI remanentTime;

    [SerializeField]
    TextMeshProUGUI viewsText;

    [SerializeField]
    Slider views;

    [SerializeField]
    Slider life;

    [SerializeField]
    EventCallsManager eventCalls;

    StreamerData streamerData;

    static List<(int, EventCall)> streamsButtons = new List<(int, EventCall)>();
    //static Dictionary<int, EventCall> streamsButtons = new Dictionary<int, EventCall>();

    bool bottomPressed;
    Color originalText;
    Color originalBackGround;

    public static void PressButtonByID(int ID)
    {
        foreach (var item in streamsButtons)
        {
            if (item.Item1 == ID)
            {
                item.Item2.button.onClick.Invoke();
                return;
            }
        }
    }

    private void StreamerManager_onStreamerChange(StreamerData obj)
    {
        if (streamerData == obj)
            return;

        if(streamerData!=null)
        {
            streamerData.Life.onChange -= Life_onChange;

            streamerData.Viewers.onChange -= Viewers_onChange;
        }


        streamImage.sprite = obj.streamerBase.streamImage;
        iconStreamerImage.sprite = obj.streamerBase.iconStreamerImage;
        title.text = obj.streamerBase.title;
        tagsStreamer.text = obj.streamerBase.tagsStreamer;
        nameStreamer.text = obj.streamerBase.nameStreamer;        

        streamerData = obj;

        streamerData.Viewers.onChange += Viewers_onChange;

        streamerData.Life.onChange += Life_onChange;
    }

    private void Viewers_onChange(IGetPercentage arg1, float arg2)
    {
        var current = arg1.current - streamerData.streamerBase.minimalViewers;

        var max = arg1.total - streamerData.streamerBase.minimalViewers;

        views.value = current / max;

        var actual = arg1.current.ToString();

        actual = actual.RichTextColor(Color.Lerp(Color.red, Color.green, views.value));

        viewsText.text = actual + "/" + arg1.total;
    }

    private void Life_onChange(IGetPercentage arg1, float arg2)
    {
        life.value = arg1.Percentage();
    }

    private void EndGame_onChange(IGetPercentage arg1, float arg2)
    {
        remanentTime.text = ((int)(arg1.current / 60)).ToString() + ":" + Seconds(arg1);
    }

    public void MyAwake()
    {
        streamerManager.onStreamerChange.delegato += StreamerManager_onStreamerChange;
        streamerManager.onStreamerCreate.delegato +=
            (StreamerData stream) => CreateStreamButton(stream);

        originalText = bottomButton.textMeshPro.color;
        originalBackGround = bottomButton.backgroundImage.color;

        streamerManager.streamersData.endGame.onChange += EndGame_onChange;
    }

    EventCall CreateStreamButton(StreamerData stream)
    {
        var ID = stream.ID;

        var button = eventCalls.Create(stream.streamerBase.iconStreamerImage, () =>
        {
            streamerManager.ChangeStreamByID(ID);
        });
        streamsButtons.Add((ID, button));

        return button;
    }

    string Seconds(IGetPercentage arg1)
    {
        var aux = (int)(arg1.current % 60);
        if (aux >= 10)
            return aux.ToString();
        else
            return "0" + aux.ToString();
    }
}
