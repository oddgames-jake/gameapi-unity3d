using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerChallengeInfo : PDictionary
{
    public PlayerChallengeInfo() : base() {}

    public PlayerChallengeInfo(IDictionary data) : base(data) { }

    public string name
    {
        get { return ContainsKey("name") ? GetString("name") : "error"; }
        set { SetProperty("name", value); }
    }

    public bool myturn
    {
        get { return ContainsKey("myturn") ? GetBool("myturn") : false; }
        set { SetProperty("myturn", value); }
    }

    public bool hasseen
    {
        get { return ContainsKey("hasseen") ? GetBool("hasseen") : false; }
        set { SetProperty("hasseen", value); }
    }

    public int wins
    {
        get { return ContainsKey("wins") ? GetInt("wins") : 0; }
        set { SetProperty("wins", value); }
    }

    public int losses
    {
        get { return ContainsKey("losses") ? GetInt("losses") : 0; }
        set { SetProperty("losses", value); }
    }

}
