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
    Slider views;

    [SerializeField]
    Slider life;

    [SerializeField]
    EventCallsManager eventCalls;

    StreamerData streamerData;

    bool bottomPressed;
    Color originalText;
    Color originalBackGround;

    private void StreamerManager_onStreamerChange(StreamerData obj)
    {
        if (streamerData == obj)
            return;

        if(streamerData!=null)
        {
            streamerData.Life.onChange -= Life_onChange;

            streamerData.Viewers.onChange -= Viewers_onChange;
        }


        streamImage.sprite = obj.streamer.streamImage;
        iconStreamerImage.sprite = obj.streamer.iconStreamerImage;
        title.text = obj.streamer.title;
        tagsStreamer.text = obj.streamer.tagsStreamer;
        nameStreamer.text = obj.streamer.nameStreamer;        

        streamerData = obj;

        streamerData.Viewers.onChange += Viewers_onChange;

        streamerData.Life.onChange += Life_onChange;
    }

    private void Viewers_onChange(IGetPercentage arg1, float arg2)
    {
        views.value = arg1.Percentage();
    }

    private void Life_onChange(IGetPercentage arg1, float arg2)
    {
        life.value = arg1.Percentage();
    }

    void TopPressed(EventCall button)
    {
        /*
        if (!bottomPressed)
            return;

        BottonPressed(button);
        */
    }

    void BottonPressed(EventCall button)
    {
        /*
        bottomPressed = !bottomPressed;

        Color auxText = originalText;
        Color auxBackground = originalBackGround;

        if(bottomPressed)
        {
            auxText = originalBackGround;
            auxBackground = originalText;
        }

        TimersManager.Create(() => button.backgroundImage.color, auxBackground, 2, Color.Lerp, (s) => button.backgroundImage.color = s);

        TimersManager.Create(() => button.textMeshPro.color, auxText, 2, Color.Lerp, (s) => button.textMeshPro.color = s);
        */
    }

    private void Awake()
    {
        streamerManager.onStreamerChange.delegato += StreamerManager_onStreamerChange;
        streamerManager.onStreamerCreate.delegato += 
            (StreamerData stream) => eventCalls
            .Create(stream.streamer.iconStreamerImage , () => streamerManager.ChangeStreamByID(stream.ID));

        originalText = bottomButton.textMeshPro.color;
        originalBackGround = bottomButton.backgroundImage.color;

        topButton.eventToCall.AddListener(TopPressed);

        bottomButton.eventToCall.AddListener(BottonPressed);
    }

}