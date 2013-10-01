using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class PPlayerProfiles
{
    private const string SECTION = "playerprofiles";
    private const string UPDATE = "update";
    private const string PING = "ping";
    private const string CREATE = "create";
    private const string LOAD = "load";

    /// <summary>
    /// Updates the Servers copy of PlayerProfile
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="callback"></param>
    public void Update(PlayerProfile profile, Action<PlayerProfile, PResponse> callback)
    {
        Update<PlayerProfile>(profile, callback);
    }

    /// <summary>
    /// Updates the serves copy of a Generic Profile
    /// </summary>
    /// <typeparam name="T"> Type of Profile (T : PlayerProfile)</typeparam>
    /// <param name="profile"></param>
    /// <param name="callback"></param>
    public void Update<T>(T profile, Action<T, PResponse> callback) where T : PlayerProfile, new()
    {
        var postdata = (Dictionary<string, object>)profile;
       
        Playtomic.API.StartCoroutine(SendProfileActionRequest<T>(UPDATE, postdata, callback));
    }

    /// <summary>
    /// Loads a players profile from server
    /// </summary>
    /// <param name="playerid"></param>
    /// <param name="callback"></param>
    public void Load(string playerid, Action<PlayerProfile,PResponse> callback)
    {
        Load<PlayerProfile>(playerid, callback);
    }

    /// <summary>
    /// Loads a profile of Type T
    /// </summary>
    /// <typeparam name="T">The profile type (T : PlayerProfile)</typeparam>
    /// <param name="playerid"></param>
    /// <param name="callback"></param>
    public void Load<T>(string playerid, Action<T, PResponse> callback) where T : PlayerProfile, new()
    {
        var postdata = new Dictionary<string, object>
        {
            {"playerid", playerid}
        };
        Playtomic.API.StartCoroutine(SendProfileActionRequest<T>(LOAD, postdata, callback));
    }

    IEnumerator SendProfileActionRequest<T>(string action, Dictionary<string, object> postdata, Action<T, PResponse> callback)
        where T : PlayerProfile, new()
    {
        var www = PRequest.Prepare(SECTION, action, postdata);
        yield return www;

        var response = default(QuickResponse<ProfileResponse<T>>);
        var profile = default(T);

        if (PRequest.WWWSuccess(www))
        {
            string data = www.text;

            Thread t = new Thread(() =>
                {
                    response = PRequest.FastProcessUnityThread<ProfileResponse<T>>(data);
                });
            t.Start();

            // wait for thread to finish
            while (t.ThreadState == ThreadState.Running)
                yield return null;

            if (response.success)
                profile = response.ResponseObject.profile;
        }
        else
        {
            response = new QuickResponse<ProfileResponse<T>> {success = false, errorcode = 1};
        }

        var resp = new PResponse {success = response.success, errorcode = response.errorcode};

        callback(profile, resp);
    }

    /// <summary>
    /// Tell the server this player is still online
    /// </summary>
    /// <param name="playerid"></param>
    public void Ping(string playerid)
    {
        var postdata = new Dictionary<string, object>
        {
            {"playerid", playerid}
        };
        Playtomic.API.StartCoroutine(SendPing(postdata,0));
    }

    private IEnumerator SendPing(Dictionary<string, object> postdata, int attempts)
    {
        while (attempts > 0)
        {
            var www = PRequest.Prepare(SECTION, PING, postdata);
            yield return www;

            // no point threading this one due to tiny size of response
            var response = PRequest.FastProcess<ResponseBase>(www);

            // if successful exit
            if (response.success)
                break;

            attempts--;
        }
    }

    /// <summary>
    /// Creates a new PlayerProfile on the server
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="callback"></param>
    public void Create(PlayerProfile profile, Action<PlayerProfile,PResponse> callback)
    {
        Create<PlayerProfile>(profile, callback);
    }

    /// <summary>
    /// Creates a player profile on the server
    /// </summary>
    /// <typeparam name="T"> Type of profile (T : PlayerProfile) </typeparam>
    /// <param name="profile"></param>
    /// <param name="callback"></param>
    public void Create<T>(T profile, Action<T, PResponse> callback) where T : PlayerProfile, new()
    {
        var postdata = (Dictionary<string,object>)profile;

        Playtomic.API.StartCoroutine(SendProfileActionRequest<T>(CREATE, postdata, callback));
    }
}

public class ProfileResponse<T> : ResponseBase
{
    public T profile;
}