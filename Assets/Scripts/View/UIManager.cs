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

    private void StreamerManager_onStreamerChange(StreamerData obj)
    {
        streamImage.sprite = obj.streamer.streamImage;
        iconStreamerImage.sprite = obj.streamer.iconStreamerImage;
        title.text = obj.streamer.title;
        tagsStreamer.text = obj.streamer.tagsStreamer;
        nameStreamer.text = obj.streamer.nameStreamer;
    }

    private void Awake()
    {
        streamerManager.onStreamerChange.delegato += StreamerManager_onStreamerChange;
    }

}
