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

    private void Awake()
    {
        streamerManager.onStreamerChange.delegato += StreamerManager_onStreamerChange;
        streamerManager.onStreamerCreate.delegato += 
            (StreamerData stream) => eventCalls
            .Create(stream.streamer.iconStreamerImage , () => streamerManager.ChangeStreamByID(stream.ID));
    }

}
