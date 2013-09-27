using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class PPlayerChallenges
{
    private const string SECTION = "playerchallenges";
    private const string CREATE = "create";
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
    }

    /// <summary>
    /// Generic Save Function
    /// </summary>
    /// <typeparam name="T">Type of Challenge (T : PlayerChallenge)</typeparam>
    /// <param name="challenge"></param>
    /// <param name="callback"></param>
    public void Save<T>(T challenge, Action<T, PResponse> callback) where T : PlayerChallenge, new()
    {
        Playtomic.API.StartCoroutine(SendSaveLoadRequest(CREATE, challenge, callback));
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

        Playtomic.API.StartCoroutine(SendSaveLoadRequest<T>(LOAD, postdata, callback));
    }

    private IEnumerator SendSaveLoadRequest<T>(string action, Dictionary<string, object> postdata, Action<T, PResponse> callback) where T : PlayerChallenge
    {
        var www = PRequest.Prepare(SECTION, action, postdata);
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
        var postdata = new Dictionary<string, object>
        {
            {"playerid", playerID}
        };
        Playtomic.API.StartCoroutine(SendListRequest(postdata, callback));
    }

    private IEnumerator SendListRequest<T>(Dictionary<string, object> postdata, Action<List<T>, int, PResponse> callback) where T: PlayerChallenge,new()
    {

        var www = PRequest.Prepare(SECTION, LIST, postdata);
        yield return www;
        
        string data = www.text;
        List<T> challenges = null;
        int numchallenges = 0;

        var response = default(QuickResponse<ListChallengeResponse<T>>);

        Thread t = new System.Threading.Thread(new ThreadStart(() =>
        {
            response = PRequest.FastProcessThreadsafe<ListChallengeResponse<T>>(data);
        }));

        t.Start();

        //wait for thread to finish
        while (t.ThreadState == ThreadState.Running)
        {
            yield return null;
        }

        if (response.success)
        {
            var ChallengeArray = response.ResponseObject.challenges;

            challenges = new List<T>();

            for (int i = 0; i < ChallengeArray.Length; i++)
            {
                challenges.Add(ChallengeArray[i]);
            }
                    
            numchallenges = challenges.Count;
        }

        PResponse resp = new PResponse();
        resp.errorcode = response.errorcode;
        resp.success = response.success;

        callback(challenges, numchallenges, resp);
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
        var postdata = new Dictionary<string, object>
        {
            {"playerid", profile.playerid},
            {"elo", profile.elo},
            {"playername", profile.name},
            {"playtime", profile.playtime},
            {"responsetime", profile.reponsetime},
            {"blockedids", ignoredIDs}
        };

        Playtomic.API.StartCoroutine(FindChallenge<U>(postdata, callback));
    }

    private IEnumerator FindChallenge<T>(Dictionary<string, object> postdata, Action<T, PResponse> callback) where T: PlayerChallenge
    {
        var www = PRequest.Prepare(MATCHMAKER, FIND, postdata);
        yield return www;
        T challenge = default(T);
        
        string data = www.text;

        var response = default(QuickResponse<FindChallengeResponse<T>>);

        Thread t = new System.Threading.Thread(new ThreadStart(() =>
        {
            response = PRequest.FastProcessThreadsafe<FindChallengeResponse<T>>(data);
        }));

        t.Start();

        //wait for thread to finish
        while (t.ThreadState == ThreadState.Running)
        {
            yield return null;
        }

        if (response.success)
            challenge = response.ResponseObject.profile;

        var resp = new PResponse();
        resp.success = response.success;
        resp.errorcode = response.errorcode;       

        callback(challenge, resp);
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
        Playtomic.API.StartCoroutine(SendUpdate<T>(SECTION, UPDATE, challenge, callback));
    }

    private IEnumerator SendUpdate<T>(string section, string action, Dictionary<string, object> postdata, Action<T, PResponse> callback) where T : PlayerChallenge
    {
        var www = PRequest.Prepare(section, action, postdata);
        yield return www;
        T challenge = default(T);
        
        string data = www.text;

        var response = default(QuickResponse<UpdateChallengeResponse<T>>);

        Thread t = new System.Threading.Thread(new ThreadStart(() =>
        {
            response = PRequest.FastProcessThreadsafe<UpdateChallengeResponse<T>>(data);
        }));

        t.Start();

        //wait for thread to finish
        while (t.ThreadState == ThreadState.Running)
        {
            yield return null;
        }

        challenge = response.ResponseObject.challenge;

        var resp = new PResponse();
        resp.errorcode = response.errorcode;
        resp.success  = response.success;

        callback(challenge, resp);
    }

    /// <summary>
    /// Sends replay and result data to the server
    /// </summary>
    /// <typeparam name="T"> Replay DataType</typeparam>
    /// <typeparam name="U"> Result DataType</typeparam>
    /// <param name="endturn"> Does this result end a turn</param>
    /// <param name="playerid"> Current player's ID</param>
    /// <param name="pc"> Current PlayerChallenge</param>
    /// <param name="replay"> Replay Data</param>
    /// <param name="result"> Result Data</param>
    /// <param name="callback"> Callback</param>
    public void SendReplay<T,U>(bool endturn, string playerid,PlayerChallenge pc,T replay,U result, Action<bool> callback)
    {
        var postdata = new Dictionary<string, object>
        {
            {"playerid", playerid},
            {"challengeid", pc.challengeid},
            {"result", result},
            {"replay", replay},
            {"eventid", pc.eventid},
            {"levelname", pc.events[pc.eventid].levelname},
            {"sceneindex", pc.events[pc.eventid].sceneindex},
            {"endturn", endturn}
        };
        Playtomic.API.StartCoroutine(sendReplay(postdata, callback));
    }

    private IEnumerator sendReplay(Dictionary<string,object> postdata, Action<bool> callback)
    {
        var www = PRequest.Prepare(SECTION, POSTRESULT, postdata);
        yield return www;
        // no point threading this one due to minute data
        var response = PRequest.FastProcess<ReplaySentResponse>(www);

        callback(response.success);
    }

    class ReplaySentResponse : ResponseBase
    {
        public bool challenge;
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
        var postdata = new Dictionary<string, object>
        {
            {"challengeid", challengeid},
            {"eventid", eventid},
            {"retrieveid", retrieveid}
        };
        Playtomic.API.StartCoroutine(getReplay<T>(SECTION, GETREPLAY, postdata, callback));
    }

    private IEnumerator getReplay<T>(string section,string action, Dictionary<string,object> postdata, Action<T,PResponse> callback)
    {
        var www = PRequest.Prepare(section, action, postdata);
        yield return www;
        
        T replay = default(T);

        string data = www.text;

        var response = default(QuickResponse<GetReplayResponse<T>>);

        // spool deserialization off to another thread, allows UI to continue updating in the meantime
        Thread t = new System.Threading.Thread(new ThreadStart(() =>
        {
            response = PRequest.FastProcessThreadsafe<GetReplayResponse<T>>(data);
        }));
        t.Start();

        //wait for thread to finish
        while (t.ThreadState == ThreadState.Running)
        {
            yield return null;
        }
        replay = response.ResponseObject.challenge["replay"];

        PResponse resp = new PResponse();
        resp.errorcode = response.errorcode;
        resp.success = response.success;
        callback(replay, resp);
    }
}

public class ListChallengeResponse<T> : ResponseBase
{
    public T[] challenges;
    public int numchallenges;
}

public class GetReplayResponse<T> : ResponseBase
{
    public Dictionary<string,T> challenge;
}

public class UpdateChallengeResponse<T> : ResponseBase
{
    public T challenge;
}

public class FindChallengeResponse<T> : ResponseBase
{
    public T profile;
}