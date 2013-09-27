using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class PlayerChallenge : PDictionary
{

    public PlayerChallenge(): base() {}

    public PlayerChallenge(IDictionary data) : base(data){}

    public string challengeid
    {
        get { return GetString("challengeid"); }
        set { SetProperty("challengeid",value); }
    }

    public int currentturn
    {
        get { return GetInt("currentturn"); }
        set { SetProperty("currentturn", value); }
    }

    public string eventid
    {
        get { return ContainsKey("eventid") ? GetString("eventid") : ""; }
        set { SetProperty("eventid", value); }
    }
    public PChallengeEvent CurrentEvent
    {
        get { return events.ContainsKey(eventid) ? events[eventid] : null; }
    }

    public bool IsMyTurn(string id)
    {
        return id == playerids[currentturn] || idle;
    }

    public bool idle
    {
        get { return GetBool("idle"); }
        set { SetProperty("idle", value); }
    }

    public List<string> playerids
    {
        get { return GetStringList("playerids"); }
        set { SetProperty("playerids", value); }
    }

    public bool hide
    {
        get { return GetBool("hide"); }
        set { SetProperty("hide", value); }
    }

    public Dictionary<string, PlayerChallengeInfo> playerinfo
    {
        get
        {
            if (ContainsKey("playerinfo"))
            {
                var toRet = new Dictionary<string, PlayerChallengeInfo>();
                if (this["playerinfo"].GetType() == typeof(Dictionary<string, object>))
                {
                    foreach (var item in (Dictionary<string, object>)this["playerinfo"])
                    {
                        if (item.Value.GetType() == typeof(Dictionary<string, object>))
                            toRet.Add(item.Key, new PlayerChallengeInfo((IDictionary)item.Value));
                    }
                }
                else
                {
                    foreach (var item in (Dictionary<string, PlayerChallengeInfo>)this["playerinfo"])
                    {
                        if (item.Value.GetType() == typeof(PlayerChallengeInfo))
                            toRet.Add(item.Key, new PlayerChallengeInfo((IDictionary)item.Value));
                    }
                }
                return toRet;
            }
                return new Dictionary<string, PlayerChallengeInfo>();
        }
        set
        {
            SetProperty("playerinfo", value);
        }
    }

    public bool IsHidden(string id)
    {
        return (playerids[currentturn] != id && hide);
    }

    public Dictionary<string, PChallengeEvent> events
    {
        get
        {
            if (ContainsKey("events"))
            {
                var toRet = new Dictionary<string, PChallengeEvent>();
                if (this["events"].GetType() == typeof(Dictionary<string, object>))
                {
                    foreach (var item in (Dictionary<string, object>)this["events"])
                    {
                        if (item.Value.GetType() == typeof(Dictionary<string, object>))
                            toRet.Add(item.Key, new PChallengeEvent((IDictionary)item.Value));
                    }
                }

                if (this["events"].GetType() == typeof(Dictionary<string, PChallengeEvent>))
                {
                    foreach (var item in (Dictionary<string, PChallengeEvent>)this["events"])
                    {
                        if (item.Value.GetType() == typeof(Dictionary<string, object>))
                            toRet.Add(item.Key, new PChallengeEvent((IDictionary)item.Value));
                        if (item.Value.GetType() == typeof(PChallengeEvent))
                            toRet.Add(item.Key, item.Value);
                    }
                }
                return toRet;
            }
            else
            {
                SetProperty("events", new Dictionary<string, PChallengeEvent>());
                return events;
            }
        }

        set { SetProperty("events", value); }
    }

    /// <summary>
    /// Datetime the challenge was created
    /// </summary>
    public int startdate
    {
        get { return ContainsKey("startdate") ? GetInt("startdate") : TimeUtils.ToUnixTime(DateTime.Now); }
        set { SetProperty("startdate", value); }
    }
    
    /// <summary>
    /// DateTime the challenge was last updated
    /// </summary>
    public int date
    {
        get { return ContainsKey("date") ? GetInt("date") : TimeUtils.ToUnixTime(DateTime.Now); }
        set { SetProperty("date", value); }
    }

    public string rdate
    {
        get { return ContainsKey("rdate") ? GetString("rdate") : "Just Now"; }
        set { SetProperty("rdate", value); }
    }
}
