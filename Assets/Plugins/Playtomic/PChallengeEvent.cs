using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PChallengeEvent : PDictionary
{
    public PChallengeEvent(): base() {}

    public PChallengeEvent(IDictionary data) : base(data) { }

    public Dictionary<string, object> data
    {
        get { return GetDictionary<string, object>("data"); }
    }

    /// <summary>
    /// Dictionary<playerid,player score/result, created serverside
    /// </summary>
    public Dictionary<string, object> results
    {
        get { return ContainsKey("results") ? GetDictionary<string, object>("results") : null; }
        set { SetProperty("results", value); }
    }

    /// <summary>
    /// Dictionary<playerid, replaydata, normally cleaned out server side to save bandwidth
    /// </summary>
    public Dictionary<string, object> replays
    {
        get
        {
            return ContainsKey("replays") ? GetDictionary<string, object>("replays") : null;
        }
        set { SetProperty("replays", value); }
    }

    public string levelname
    {
        get { return ContainsKey("levelname") ? GetString("levelname") : "error"; }
        set { SetProperty("levelname", value); }
    }

    public int sceneindex
    {
        get { return ContainsKey("sceneindex") ? GetInt("sceneindex") : 0; }
        set { SetProperty("sceneindex", value); }
    }
}
