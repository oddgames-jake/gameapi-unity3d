using UnityEngine;
using System.Collections;

public class PlayerProfile : PDictionary
{

    public PlayerProfile() : base() { }

    public PlayerProfile(IDictionary data) : base(data) { }

    public string playerid
    {
        get { return GetString("playerid"); }
        set { SetProperty("playerid", value); }
    }

    public string name
    {
        get { return GetString("playername"); }
        set { SetProperty("playername", value); }
    }

    public int elo
    {
        get { return GetInt("elo"); }
        set { SetProperty("elo", value); }
    }

    public int reponsetime
    {
        get { return GetInt("responsetime"); }
        set { SetProperty("responsetime", value); }
    }

    public int playtime
    {
        get { return GetInt("playtime"); }
        set { SetProperty("playtime", value); }
    }

}
