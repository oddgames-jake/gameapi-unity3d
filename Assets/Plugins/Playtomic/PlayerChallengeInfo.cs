using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerChallengeInfo : PDictionary
{
    public PlayerChallengeInfo() : base() {}

    public PlayerChallengeInfo(IDictionary data) : base(data) { }

    public string Name
    {
        get
        {
            try { return GetString("name"); }
            catch { return "error"; }
        }
    }

    public bool IsThisTurn
    {
        get { return GetBool("myturn"); }
    }

    public bool HasSeenChallenge
    {
        get { return GetBool("hasseen"); }
    }

    public int Wins
    {
        get
        {
            if (ContainsKey("wins"))
                return GetInt("wins");
            else
                return 0;
        }
        set
        {
            SetProperty("wins", value);
        }
    }

    public int Losses
    {
        get
        {
            if (ContainsKey("losses"))
                return GetInt("losses");
            else
                return 0;
        }
        set
        {
            SetProperty("losses", value);
        }
    }

}
