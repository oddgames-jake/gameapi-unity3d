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
        get
        {
            return GetStringList("playerids");           
        }
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
            else
            {
                return new Dictionary<string, PlayerChallengeInfo>();
            }
        }
    }

    public bool IsHidden(string id)
    {
        return (PlayerIDs[CurrentTurn] != id && GetBool("hide"));
    }

    /// <summary>
    /// Dictionary<eventid,eventdata>
    /// </summary>
    public Dictionary<string,PChallengeEvent> Events
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
        //get { return GetDictionary<string,PChallengeEvent>("events"); }
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
    
    /// <summary>
    /// DateTime the challenge was last updated
    /// </summary>
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
