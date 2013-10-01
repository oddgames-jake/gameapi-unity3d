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

    private IEnumerator SendSaveLoadRequest<T>(string action, 
                                               Dictionary<string, object> postdata,
                                               Action<T, PResponse> callback) where T : PlayerChallenge
    {
        var www = PRequest.Prepare(SECTION, action, postdata);
        yield return www;

        var response = default(QuickResponse<UpdateChallengeResponse<T>>);
        var challenge = default(T);

        if (PRequest.WWWSuccess(www))
        {
            string data = www.text;

            Thread t = new Thread(() =>
            {
                response = PRequest.FastProcessUnityThread<UpdateChallengeResponse<T>>(data);
            });

            t.Start();

            // wait for thread to finish
            while (t.ThreadState == ThreadState.Running)
                yield return null;

        }
        else
        {
            response = new QuickResponse<UpdateChallengeResponse<T>> {success = false, errorcode = 1};
        }

        if (response.success)
            challenge = response.ResponseObject.challenge;
        
        var resp = new PResponse {success = response.success, errorcode = response.errorcode};

        callback(challenge, resp);
    }

    /// <summary>
    /// Returns a List of Challenges a Player is in
    /// </summary>
    /// <param name="playerID"> The player id to search for</param>
    /// <param name="replays"> Return replay data as well</param>
    /// <param name="callback"> Callback Function</param>
    public void List(string playerID, bool replays,Action<List<PlayerChallenge>, int, PResponse> callback)
    {
        List<PlayerChallenge>(playerID, replays, callback);
    }

    /// <summary>
    /// Returns a List of Challenges a Player is in
    /// </summary>
    /// <typeparam name="T"> DataType of Challenge (T : PlayerChallenge)</typeparam>
    /// <param name="playerID"></param>
    /// <param name="replays"> Return replay data as well</param>
    /// <param name="callback"></param>
    public void List<T>(string playerID, bool replays, Action<List<T>, int, PResponse> callback) where T  : PlayerChallenge, new()
    {
        var postdata = new Dictionary<string, object>{{"playerid", playerID}};
        if (replays)
            postdata.Add("full", true);
        Playtomic.API.StartCoroutine(SendListRequest(postdata, callback));
    }

    private IEnumerator SendListRequest<T>(Dictionary<string, object> postdata, 
                                           Action<List<T>, int, PResponse> callback) where T: PlayerChallenge, new()
    {

        var www = PRequest.Prepare(SECTION, LIST, postdata);
        yield return www;

        List<T> challenges = null;
        int numchallenges = 0;
        var response = default(QuickResponse<ListChallengeResponse<T>>);

        if (PRequest.WWWSuccess(www))
        {
            string data = www.text;

            Thread t = new Thread(() =>
            {
                response = PRequest.FastProcessUnityThread<ListChallengeResponse<T>>(data);
            });

            t.Start();

            // wait for thread to finish
            while (t.ThreadState == ThreadState.Running)
                yield return null;
        }
        else
        {
            response = new QuickResponse<ListChallengeResponse<T>> {success = false, errorcode = 1};
        }

        if (response.success)
        {
            var challengeArray = response.ResponseObject.challenges;

            challenges = new List<T>();

            for (int i = 0; i < challengeArray.Length; i++)
            {
                challenges.Add(challengeArray[i]);
            }
                    
            numchallenges = challenges.Count;
        }

        var resp = new PResponse {errorcode = response.errorcode, success = response.success};

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
            {"responsetime", profile.responsetime},
            {"blockedids", ignoredIDs}
        };

        Playtomic.API.StartCoroutine(FindChallenge<U>(postdata, callback));
    }

    private IEnumerator FindChallenge<T>(Dictionary<string, object> postdata, Action<T, PResponse> callback) where T: PlayerChallenge
    {
        var www = PRequest.Prepare(MATCHMAKER, FIND, postdata);
        yield return www;

        var challenge = default(T);
        var response = default(QuickResponse<FindChallengeResponse<T>>);

        if (PRequest.WWWSuccess(www))
        {

            string data = www.text;

            Thread t = new Thread(() =>
            {
                response = PRequest.FastProcessUnityThread<FindChallengeResponse<T>>(data);
            });

            t.Start();

            // wait for thread to finish
            while (t.ThreadState == ThreadState.Running)
                yield return null;
        }
        else
        {
            response = new QuickResponse<FindChallengeResponse<T>> {success = false, errorcode = 1};
        }

        if (response.success)
            challenge = response.ResponseObject.profile;

        var resp = new PResponse {success = response.success, errorcode = response.errorcode};

        callback(challenge, resp);
    }

    /// <summary>
    /// Updates the servers copy of the PlayerChallenge
    /// </summary>
    /// <param name="challenge"> The updated challenge</param>
    /// <param name="callback"> Callback function</param>
    public void Update(PlayerChallenge challenge, Action<bool, PResponse> callback)
    {
        Update<PlayerChallenge>(challenge, callback);
    }

    /// <summary>
    /// Updates challenge info on server
    /// </summary>
    /// <typeparam name="T">Type of Challenge (T : PlayerChallenge)</typeparam>
    /// <param name="challenge"></param>
    /// <param name="callback"></param>
    public void Update<T>(T challenge, Action<bool, PResponse> callback) where T : PlayerChallenge, new()
    {
        Playtomic.API.StartCoroutine(SendUpdate<T>(SECTION, UPDATE, challenge, callback));
    }

    private IEnumerator SendUpdate<T>(string section, 
                                      string action, 
                                      Dictionary<string, object> postdata, 
                                      Action<bool, PResponse> callback) where T : PlayerChallenge, new()
    {

        var www = PRequest.Prepare(section, action, postdata);
        yield return www;

        var response = default(QuickResponse<ReplaySentResponse>);

        if (PRequest.WWWSuccess(www))
        {
            string data = www.text;
            Thread t = new Thread(() =>
            {
                response = PRequest.FastProcessUnityThread<ReplaySentResponse>(data);
            });

            t.Start();

            // wait for thread to finish
            while (t.ThreadState == ThreadState.Running)
                yield return null;
        }
        else
        {
            response = new QuickResponse<ReplaySentResponse> {success = false, errorcode = 1};
        }
        
        var resp = new PResponse {errorcode = response.errorcode, success = response.success};

        callback(response.success, resp);
    }

    /// <summary>
    /// Sends replay and result data to the server
    /// </summary>
    /// <typeparam name="T"> Replay DataType (class,dictionary, json/xml data etc)</typeparam>
    /// <typeparam name="U"> Result DataType (int, float etc)</typeparam>
    /// <param name="endturn"> Does this result end a turn</param>
    /// <param name="playerid"> Current player's ID</param>
    /// <param name="pc"> Current PlayerChallenge</param>
    /// <param name="replay"> Replay Data</param>
    /// <param name="result"> Result Data</param>
    /// <param name="callback"> Callback</param>
    public void SendReplay<T,U>(bool endturn, 
                                string playerid,
                                PlayerChallenge pc,
                                T replay,
                                U result, 
                                Action<bool> callback)
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
        
        // no point threading this one due to tiny ammount of data
        var response = PRequest.FastProcess<ReplaySentResponse>(www);

        callback(response.success);
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
        
        var replay = default(T);
        var response = new QuickResponse<GetReplayResponse<T>>();

        if (PRequest.WWWSuccess(www))
        {
            string data = www.text;

            // spool deserialization off to another thread, allows UI to continue updating in the meantime
            Thread t = new Thread(() =>
            {
                response = PRequest.FastProcessUnityThread<GetReplayResponse<T>>(data);
            });
            t.Start();

            // wait for thread to finish
            while (t.ThreadState == ThreadState.Running)
                yield return null;

            if(response.success)
                replay = response.ResponseObject.challenge["replay"];
        }
        else
        {
            response = new QuickResponse<GetReplayResponse<T>> {success = false, errorcode = 1};
        }

        var resp = new PResponse {errorcode = response.errorcode, success = response.success};

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
    public Dictionary<string, T> challenge;
}

public class UpdateChallengeResponse<T> : ResponseBase
{
    public T challenge;
}

public class FindChallengeResponse<T> : ResponseBase
{
    public T profile;
}

class ReplaySentResponse : ResponseBase
{
    public bool challenge;
}