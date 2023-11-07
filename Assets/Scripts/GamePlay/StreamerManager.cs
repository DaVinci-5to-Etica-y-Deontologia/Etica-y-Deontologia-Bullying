using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamerManager : SingletonMono<StreamerManager>
{
    public DataPic<Streamer> streamers = new();
}
