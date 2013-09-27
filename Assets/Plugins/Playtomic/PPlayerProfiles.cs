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

    public void Update(PlayerProfile profile, Action<PlayerProfile, PResponse> callback)
    {
        Update<PlayerProfile>(profile, callback);
    }

    public void Update<T>(T profile, Action<T, PResponse> callback) where T : PlayerProfile, new()
    {
        var postdata = (Dictionary<string, object>)profile;
       
        Playtomic.API.StartCoroutine(SendProfileActionRequest<T>(SECTION, UPDATE, postdata, callback));
    }

    public void Load(string playerid, Action<PlayerProfile,PResponse> callback)
    {
        Load<PlayerProfile>(playerid, callback);
    }

    public void Load<T>(string playerid, Action<T, PResponse> callback)
    {
        var postdata = new Dictionary<string, object>
        {
            {"playerid", playerid}
        };
        Playtomic.API.StartCoroutine(SendProfileActionRequest<T>(SECTION, LOAD, postdata, callback));
    }

    IEnumerator SendProfileActionRequest<T>(string section, string action, Dictionary<string, object> postdata, Action<T, PResponse> callback)
    {
        var www = PRequest.Prepare(section, action, postdata);
        yield return www;

        string data = www.text;

        var response = default(QuickResponse<ProfileResponse<T>>);

        Thread t = new System.Threading.Thread(new ThreadStart(() =>
            {
                response = PRequest.FastProcessThreadsafe<ProfileResponse<T>>(data);
            }));
        t.Start();

        //wait for thread to finish
        while (t.ThreadState == ThreadState.Running)
        { 
            yield return null;
        }

        PResponse resp = new PResponse();
        resp.success = response.success;
        resp.errorcode = response.errorcode;

        callback(response.ResponseObject.profile, resp);
    }

    public void Ping(string playerid)
    {
        var postdata = new Dictionary<string, object>
        {
            {"playerid", playerid}
        };
        Playtomic.API.StartCoroutine(SendPing(postdata));
    }

    IEnumerator SendPing(Dictionary<string, object> postdata)
    {
        var www = PRequest.Prepare(SECTION, PING, postdata);
        yield return www;
        // no point threading this one due to minute size of response
        var response = PRequest.FastProcess<ResponseBase>(www);
        // If ping unsuccessful resend
        if (!response.success)
            SendPing(postdata);
    }

    public void Create(string UID, PlayerProfile profile, Action<PlayerProfile,PResponse> callback)
    {
        var postdata = new Dictionary<string, object>
        {
            {"playerid", UID},
            {"playername", profile.name},
            {"elo", 1400},
            {"playtime", 1},
            {"responsetime", 1}
        };

        Playtomic.API.StartCoroutine(SendProfileActionRequest(SECTION, CREATE, postdata, callback));
    }
}

public class ProfileResponse<T> : ResponseBase
{
    public T profile;
}