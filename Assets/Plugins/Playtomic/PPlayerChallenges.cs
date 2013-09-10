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

    /// <summary>
    /// Saves a challenge
    /// </summary>
    /// <param name="challenge"> The challenge to save</param>
    /// <param name="callback"> Callback function</param>
    public void Save(PlayerChallenge challenge, Action<PlayerChallenge, PResponse> callback)
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
        var postdata = new Dictionary<string, object>
		{
			{"challengeid", challengeid }
		};

        Playtomic.API.StartCoroutine(SendSaveLoadRequest(SECTION, LOAD, postdata, callback));
    }

    private IEnumerator SendSaveLoadRequest(string section, string action, Dictionary<string, object> postdata, Action<PlayerChallenge, PResponse> callback)
    {
        var www = PRequest.Prepare(section, action, postdata);
        yield return www;

        var response = PRequest.Process(www);
        PlayerChallenge challenge = null;

        if (response.success)
        {
            challenge = new PlayerChallenge((Dictionary<string, object>)response.json["challenge"]);
        }

        callback(challenge, response);
    }

    /// <summary>
    /// Returns a list of challenges a player is in
    /// </summary>
    /// <param name="playerID"> The player id to search for</param>
    /// <param name="callback"> Callback Function</param>
    public void List(string playerID, Action<List<PlayerChallenge>, int, PResponse> callback)
    {
        var postdata = new Dictionary<string, object>();
        postdata.Add("playerid", playerID);
        Playtomic.API.StartCoroutine(SendListRequest(SECTION, LIST, postdata, callback));
    }

    private IEnumerator SendListRequest(string section, string action, Dictionary<string, object> postdata, Action<List<PlayerChallenge>, int, PResponse> callback)
    {
        var www = PRequest.Prepare(SECTION, LIST, postdata);
        yield return www;

        var response = PRequest.Process(www);
        List<PlayerChallenge> challenges = null;
        int numchallenges = 0;

        if (response.success)
        {
            var data = (Dictionary<string, object>)response.json;
            challenges = new List<PlayerChallenge>();
            numchallenges = (int)(double)data["numchallenges"];

            var challengearr = (List<object>)data["challenges"];

            for (var i = 0; i < challengearr.Count; i++)
            {
                challenges.Add(new PlayerChallenge((Dictionary<string, object>)challengearr[i]));
            }
        }

        callback(challenges, numchallenges, response);
    }

    /// <summary>
    /// Updates the servers copy of the PlayerChallenge
    /// </summary>
    /// <param name="challenge"> The updated challenge</param>
    /// <param name="callback"> Callback function</param>
    public void Update(PlayerChallenge challenge, Action<PlayerChallenge, PResponse> callback)
    {
        Playtomic.API.StartCoroutine(SendUpdate(SECTION, UPDATE, (Dictionary<string, object>)challenge, callback));
    }

    private IEnumerator SendUpdate(string section, string action, Dictionary<string, object> postdata, Action<PlayerChallenge, PResponse> callback)
    {
        var www = PRequest.Prepare(section, action, postdata);
        yield return www;

        var response = PRequest.Process(www);
        PlayerChallenge challenge = null;

        if (response.success)
        {
            challenge = new PlayerChallenge((Dictionary<string, object>)response.json["challenge"]);
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
    public void SendReplay<T,U>(string playerid,string challengeid, string eventid,T replay,U result, Action<bool, PResponse> callback)
    {
        var postdata = new Dictionary<string, object>();
        postdata.Add("playerid", playerid);
        postdata.Add("challengeid", challengeid);
        postdata.Add("result", result);
        postdata.Add("replay", replay);
        postdata.Add("eventid", eventid);
        Playtomic.API.StartCoroutine(sendReplay(SECTION, POSTRESULT, postdata, callback));
    }

    private IEnumerator sendReplay(string section, string action, Dictionary<string,object> postdata, Action<bool, PResponse> callback)
    {
        var www = PRequest.Prepare(section, action, postdata);
        yield return www;

        var response = PRequest.Process(www);

        bool success = response.success;

        callback(success, response);
    }

    public void GetReplay(PlayerChallenge pc, string eventid, string retrieveid, Action<object, PResponse> callback)
    {
        GetReplay(pc.ChallengeID, eventid, retrieveid, callback);
    }

    public void GetReplay(string challengeid, string eventid, string retrieveid, Action<object, PResponse> callback)
    {
        var postdata = new Dictionary<string, object>();
        postdata.Add("challengeid", challengeid);
        postdata.Add("eventid", eventid);
        postdata.Add("retrieveid", retrieveid);
        Playtomic.API.StartCoroutine(getReplay(SECTION, GETREPLAY, postdata, callback));
    }

    private IEnumerator getReplay(string section,string action, Dictionary<string,object> postdata, Action<object,PResponse> callback)
    {
        var www = PRequest.Prepare(section, action, postdata);
        yield return www;
        
        var response = PRequest.Process(www);
        object replay = null;
        if (response.success)
            replay = ((Dictionary<string, object>)response.json["challenge"])["replay"];

        callback(replay, response);
    }
}