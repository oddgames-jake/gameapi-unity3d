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
    }

    public int CurrentTurn
    {
        get { return GetInt("currentturn"); }
    }

    public string EventID
    {
        get { return GetString("eventid"); }
        set { SetProperty("eventid", value); }
    }
    public PChallengeEvent CurrentEvent
    {
        get { return Events[GetString("eventid")]; }
    }

    public bool IsMyTurn(string id)
    {
        return id == PlayerIDs[CurrentTurn] || IsIdle;
    }

    public bool IsIdle
    {
        get { return GetBool("idle"); }
    }

    public List<string> PlayerIDs
    {
        get { return GetStringList("playerids"); }
    }

    public Dictionary<string, PlayerChallengeInfo> PlayerData
    {
        get
        {
            if (ContainsKey("playerinfo"))
            {
                var toRet = new Dictionary<string, PlayerChallengeInfo>();

                foreach (var item in (Dictionary<string, object>)this["playerinfo"])
                {
                    toRet.Add(item.Key, new PlayerChallengeInfo((IDictionary)item.Value));
                }
                return toRet;
            }
                return new Dictionary<string, PlayerChallengeInfo>();
        }
    }

    public bool IsHidden(string id)
    {
        return (PlayerIDs[CurrentTurn] != id && GetBool("hide"));
    }

    public Dictionary<string, PChallengeEvent> Events
    {
        get
        {
            if (ContainsKey("events"))
            {
                var toRet = new Dictionary<string, PChallengeEvent>();
                if (this["events"].GetType() == typeof(Dictionary<string, PChallengeEvent>))
                {
                    foreach (var item in (Dictionary<string, PChallengeEvent>)this["events"])
                    {
                        toRet.Add(item.Key, new PChallengeEvent((IDictionary)item.Value));
                    }
                }
                else
                {
                    foreach (var item in (Dictionary<string, object>)this["events"])
                    {
                        toRet.Add(item.Key, new PChallengeEvent((IDictionary)item.Value));
                    }
                }
                return toRet;
            }
            else
            {
                SetProperty("events", new Dictionary<string, PChallengeEvent>());
                return Events;
            }
        }

        set { SetProperty("events", value); }
    }

    /// <summary>
    /// Datetime the challenge was created
    /// </summary>
    public DateTime startdate
    {
        get
        {
            if (ContainsKey("startdate"))
                return TimeUtils.FromUnixTime(GetInt("startdate"));
            else
                return DateTime.Now;
        }
    }
    
    /// <summary>
    /// DateTime the challenge was last updated
    /// </summary>
    public DateTime date
    {
        get
        {
            if (ContainsKey("date"))
                return TimeUtils.FromUnixTime(GetInt("date"));
            else
                return DateTime.Now;
        }
    }

    public string rdate
    {
        get {
            if (ContainsKey("rdate"))
                return GetString("rdate");
            else
                return "Just Now";
        }
    }
}
