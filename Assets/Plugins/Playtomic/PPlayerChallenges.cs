using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PPlayerChallenges
{
    private const string SECTION = "playerchallenges";
    private const string SAVE = "save";
    private const string LIST = "list";
    private const string LOAD = "load";
    private const string UPDATE = "update";
    private const string POSTRESULT = "postresult";
    private const string GETREPLAY = "getreplay";
    private const string MATCHMAKER = "matchmaker";
    private const string FIND = "find";

    /// <summary>
    /// Saves a challenge
    /// </summary>
    /// <param name="challenge"> The challenge to save</param>
    /// <param name="callback"> Callback function</param>
    public void Save(PlayerChallenge challenge, Action<PlayerChallenge, PResponse> callback)
    {
        Save<PlayerChallenge>(challenge, callback);
        //Playtomic.API.StartCoroutine(SendSaveLoadRequest(SECTION, SAVE, (Dictionary<string, object>)challenge, callback));
    }

    /// <summary>
    /// Generic Save Function
    /// </summary>
    /// <typeparam name="T">Type of Challenge (T : PlayerChallenge)</typeparam>
    /// <param name="challenge"></param>
    /// <param name="callback"></param>
    public void Save<T>(T challenge, Action<T, PResponse> callback) where T : PlayerChallenge, new()
    {
        Playtomic.API.StartCoroutine(SendSaveLoadRequest(SECTION, SAVE, (Dictionary<string, object>)challenge, callback));
    }

    /// <summary>
    /// Loads a PlayerChallenge
    /// </summary>
    /// <param name="challengeid"> ChallengeID to load</param>
    /// <param name="callback"> Callback function</param>
    public void Load(string challengeid, Action<PlayerChallenge, PResponse> callback)
    {
        Load<PlayerChallenge>(challengeid, callback);
    }

    /// <summary>
    /// Loads a PlayerChallenge
    /// </summary>
    /// <typeparam name="T">Type of Challenge(T : PlayerChallenge)</typeparam>
    /// <param name="challengeid">The ID to load</param>
    /// <param name="callback"> The callback function</param>
    public void Load<T>(string challengeid, Action<T, PResponse> callback) where T : PlayerChallenge, new()
    {
        var postdata = new Dictionary<string, object>
		{
			{"challengeid", challengeid }
		};

        Playtomic.API.StartCoroutine(SendSaveLoadRequest<T>(SECTION, LOAD, postdata, callback));
    }

    private IEnumerator SendSaveLoadRequest<T>(string section, string action, Dictionary<string, object> postdata, Action<T, PResponse> callback) where T : PlayerChallenge
    {
        var www = PRequest.Prepare(section, action, postdata);
        yield return www;

        var response = PRequest.Process(www);
        T challenge = default(T);

        if (response.success)
        {
            challenge =(T) new PlayerChallenge((Dictionary<string,object>)response.json["challenge"]);
        }

        callback(challenge, response);
    }

    /// <summary>
    /// Returns a List of Challenges a Player is in
    /// </summary>
    /// <param name="playerID"> The player id to search for</param>
    /// <param name="callback"> Callback Function</param>
    public void List(string playerID, Action<List<PlayerChallenge>, int, PResponse> callback)
    {
        List<PlayerChallenge>(playerID, callback);
    }

    /// <summary>
    /// Returns a List of Challenges a Player is in
    /// </summary>
    /// <typeparam name="T"> DataType of Challenge (T : PlayerChallenge)</typeparam>
    /// <param name="playerID"></param>
    /// <param name="callback"></param>
    public void List<T>(string playerID, Action<List<T>, int, PResponse> callback) where T  : PlayerChallenge, new()
    {
        var postdata = new Dictionary<string, object>();
        postdata.Add("playerid", playerID);
        Playtomic.API.StartCoroutine(SendListRequest(SECTION, LIST, postdata, callback));
    }

    private IEnumerator SendListRequest<T>(string section, string action, Dictionary<string, object> postdata, Action<List<T>, int, PResponse> callback) where T: PlayerChallenge,new()
    {

        var www = PRequest.Prepare(SECTION, LIST, postdata);
        yield return www;

        var response = PRequest.Process(www);

        List<T> challenges = null;
        int numchallenges = 0;

        if (response.success)
        {
            var data = (Dictionary<string, object>)response.json;
            
            challenges = new List<T>();
            numchallenges = (int)(double)data["numchallenges"];

            var challengearr = (List<object>)data["challenges"];

            for (var i = 0; i < challengearr.Count; i++)
            {
                challenges.Add((T) new PlayerChallenge((Dictionary<string,object>) challengearr[i]));
            }
        }

        callback(challenges, numchallenges, response);
    }

    /// <summary>
    /// Finds a new challenge partner
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="ignoredIDs">A list of IDs to ignore, should include existing matches and blocked players</param>
    /// <param name="callback"></param>
    public void Find(PlayerProfile profile, List<string> ignoredIDs, Action<PlayerChallenge, PResponse> callback)
    {
        Find<PlayerProfile, PlayerChallenge>(profile,ignoredIDs, callback);
    }

    /// <summary>
    /// Finds a new challenge partner
    /// </summary>
    /// <typeparam name="T"> Type of Profile (T : PlayerProfile)</typeparam>
    /// <typeparam name="U"> Type of Challenge (U : PlayerChallenge)</typeparam>
    /// <param name="profile">Players current profile</param>
    /// <param name="ignoredIDs"> A list of IDs to ignore, should include existing matches and blocked players</param>
    /// <param name="callback"></param>
    public void Find<T, U>(T profile, List<string> ignoredIDs, Action<U, PResponse> callback)
        where T : PlayerProfile, new()
        where U : PlayerChallenge, new()
    {
        var postdata = new Dictionary<string, object>();
        postdata.Add("playerid", profile.ID);
        postdata.Add("elo", profile.Elo);
        postdata.Add("playername", profile.Name);
        postdata.Add("playtime", profile.PlayTime);
        postdata.Add("responsetime", profile.ResponseTime);
        postdata.Add("blockedids", ignoredIDs);

        Playtomic.API.StartCoroutine(FindChallenge<U>(postdata, callback));
    }

    private IEnumerator FindChallenge<T>(Dictionary<string, object> postdata, Action<T, PResponse> callback) where T: PlayerChallenge
    {
        var www = PRequest.Prepare(MATCHMAKER, FIND, postdata);
        yield return www;

        var response = PRequest.Process(www);
        T challenge = default(T);

        if (response.success)
        {
            challenge = (T) response.json["profile"];
        }

        callback(challenge, response);
    }

    /// <summary>
    /// Updates the servers copy of the PlayerChallenge
    /// </summary>
    /// <param name="challenge"> The updated challenge</param>
    /// <param name="callback"> Callback function</param>
    public void Update(PlayerChallenge challenge, Action<PlayerChallenge, PResponse> callback)
    {
        Update<PlayerChallenge>(challenge, callback);
    }

    /// <summary>
    /// Updates challenge info on server
    /// </summary>
    /// <typeparam name="T">Type of Challenge (T : PlayerChallenge)</typeparam>
    /// <param name="challenge"></param>
    /// <param name="callback"></param>
    public void Update<T>(T challenge, Action<T, PResponse> callback) where T : PlayerChallenge, new()
    {
        Playtomic.API.StartCoroutine(SendUpdate<T>(SECTION, UPDATE, (Dictionary<string, object>)challenge, callback));
    }

    private IEnumerator SendUpdate<T>(string section, string action, Dictionary<string, object> postdata, Action<T, PResponse> callback) where T : PlayerChallenge
    {
        var www = PRequest.Prepare(section, action, postdata);
        yield return www;

        var response = PRequest.Process(www);
        T challenge = default(T);

        if (response.success)
        {
            challenge = (T) new PlayerChallenge((Dictionary<string, object>) response.json["challenge"]);
        }

        callback(challenge, response);
    }

    /// <summary>
    /// Sends replay and result data to the server
    /// </summary>
    /// <typeparam name="T"> Replay DataType</typeparam>
    /// <typeparam name="U"> Result DataType</typeparam>
    /// <param name="playerid"> Current player's ID</param>
    /// <param name="challengeid"> Current ChallengeID</param>
    /// <param name="eventid"> The event ID</param>
    /// <param name="replay"> Replay Data</param>
    /// <param name="result"> Result Data</param>
    /// <param name="callback"> Callback</param>
    public void SendReplay<T,U>(bool endturn, string playerid,PlayerChallenge pc,T replay,U result, Action<bool, PResponse> callback)
    {
        var postdata = new Dictionary<string, object>();
        postdata.Add("playerid", playerid);
        postdata.Add("challengeid", pc.ChallengeID);
        postdata.Add("result", result);
        postdata.Add("replay", replay);
        postdata.Add("eventid", pc.EventID);
        postdata.Add("levelname", pc.Events[pc.EventID].LevelName);
        postdata.Add("sceneindex", pc.Events[pc.EventID].LevelIndex);
        postdata.Add("endturn", endturn);
        Playtomic.API.StartCoroutine(sendReplay(SECTION, POSTRESULT, postdata, callback));
    }

    private IEnumerator sendReplay(string section, string action, Dictionary<string,object> postdata, Action<bool, PResponse> callback)
    {
        var www = PRequest.Prepare(section, action, postdata);
        yield return www;

        var response = PRequest.Process(www);
        Debug.Log(response.success + " " + response.errorcode);
        bool success = response.success;

        callback(success, response);
    }

    /// <summary>
    /// Downloads ReplayData
    /// </summary>
    /// <typeparam name="T">The type of Replay Data</typeparam>
    /// <param name="challengeid"> Challenge ID</param>
    /// <param name="eventid"> The Event ID</param>
    /// <param name="retrieveid"> The ID of the player who recorded replay</param>
    /// <param name="callback"> Callback function</param>
    public void GetReplay<T>(string challengeid, string eventid, string retrieveid, Action<T, PResponse> callback)
    {
        var postdata = new Dictionary<string, object>();
        postdata.Add("challengeid", challengeid);
        postdata.Add("eventid", eventid);
        postdata.Add("retrieveid", retrieveid);
        Playtomic.API.StartCoroutine(getReplay(SECTION, GETREPLAY, postdata, callback));
    }

    private IEnumerator getReplay<T>(string section,string action, Dictionary<string,object> postdata, Action<T,PResponse> callback)
    {
        var www = PRequest.Prepare(section, action, postdata);
        yield return www;
        
        var response = PRequest.Process(www);
        T replay = default(T);
        if (response.success)
        {
            if (typeof(T).IsSubclassOf(typeof(PDictionary)))
            {
                replay = 
                    (T) (object) new PDictionary((Dictionary<string, object>) ((Dictionary<string, object>) response.json["challenge"])["replay"]);
            }
            else
                replay = (T)((Dictionary<string, object>)response.json["challenge"])["replay"];
        }
        callback(replay, response);
    }

}