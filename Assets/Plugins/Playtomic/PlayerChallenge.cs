using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class PlayerChallenge : PDictionary
{

    public PlayerChallenge(): base() {}

    public PlayerChallenge(IDictionary data) : base(data){}

    public string ChallengeID
    {
        get { return GetString("challengeid"); }
        set { SetProperty("challengeid", value); }
    }

    public int CurrentTurn
    {
        get { return GetInt("currentturn"); }
        set { SetProperty("currentturn", value); }
    }

    public bool IsMyTurn(string id)
    {
        return (PlayerIDs[GetInt("playerturn")] == id);
    }

    public List<string> PlayerIDs
    {
        get { return GetList<string>("playerids"); }
        set { SetProperty("playerids", value); }
    }

    public List<string> PlayerNames
    {
        get { return GetList<string>("playernames"); }
        set { SetProperty("playernames", value); }
    }

    /// <summary>
    /// Dictionary<eventid,eventdata>
    /// </summary>
    public Dictionary<string, PChallengeEvent> Events
    {
        get { return GetDictionary<string, PChallengeEvent>("events"); }
        set { SetProperty("events", value); }
    }

    public string CurrentEvent
    {
        get { return GetString("currentevent"); }
        set { SetProperty("currentevent", value); }
    }

    public DateTime startdate
    {
        get
        {
            if (ContainsKey("startdate"))
            {
                return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + 
                    TimeSpan.FromSeconds((int)this["startdate"])).ToLocalTime();
            }
            else
            {
                return DateTime.Now;
            }
        }
    }

    public DateTime date
    {
        get
        {
            if (ContainsKey("date"))
            {
                return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) +
                    TimeSpan.FromSeconds((int)this["date"])).ToLocalTime();
            }
            else
            {
                return DateTime.Now;
            }
        }
    }

    public string rdate
    {
        get { return GetString("rdate"); }
    }
}
