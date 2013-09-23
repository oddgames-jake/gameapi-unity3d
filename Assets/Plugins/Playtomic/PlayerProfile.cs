using UnityEngine;
using System.Collections;

public class PlayerProfile : PDictionary
{

    public PlayerProfile() : base() { }

    public PlayerProfile(IDictionary data) : base(data) { }

    public string ID
    {
        get { return GetString("playerid"); }
    }

    public string Name
    {
        get { return GetString("playername"); }
        set { SetProperty("playername", value); }
    }

    public int Elo
    {
        get { return GetInt("elo"); }
        set { SetProperty("elo", value); }
    }

    public int ResponseTime
    {
        get { return GetInt("responsetime"); }
        set { SetProperty("responsetime", value); }
    }

    public int PlayTime
    {
        get { return GetInt("playtime"); }
        set { SetProperty("playtime", value); }
    }

}
