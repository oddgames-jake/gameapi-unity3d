using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PChallengeEvent : PDictionary
{
    public PChallengeEvent(): base() {}

    public PChallengeEvent(IDictionary data) : base(data) { }

    public Dictionary<string, object> Data
    {
        get { return GetDictionary<string, object>("data"); }
    }

    /// <summary>
    /// Dictionary<playerid,player score/result, created serverside
    /// </summary>
    public Dictionary<string, object> Results
    {
        get { return GetDictionary<string, object>("results"); }
    }

    /// <summary>
    /// Dictionary<playerid, replaydata, normally cleaned out server side to save bandwidth
    /// </summary>
    public Dictionary<string, object> Replays
    {
        get
        {

            if (ContainsKey("replays"))
                return GetDictionary<string, object>("replays");
            else
                return null;
        }
    }

    public string LevelName
    {
        get { return GetString("levelname"); }
        set { SetProperty("levelname", value); }
    }

    public int LevelIndex
    {
        get { return GetInt("sceneindex"); }
        set { SetProperty("sceneindex", value); }
    }
}
