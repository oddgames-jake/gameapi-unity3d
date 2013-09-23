using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PPlayerProfiles
{
    private const string SECTION = "playerprofiles";
    private const string UPDATE = "update";
    private const string PING = "ping";
    private const string CREATE = "create";
    private const string LOAD = "load";

    public void Update(PlayerProfile profile, Action<PlayerProfile, PResponse> callback)
    {
        var postdata = (Dictionary<string, object>)profile;
       
        Playtomic.API.StartCoroutine(SendProfileActionRequest(SECTION, UPDATE, postdata, callback));
    }

    public void Load(string playerid, Action<PlayerProfile,PResponse> callback)
    {
        var postdata = new Dictionary<string, object>();
        postdata.Add("playerid", playerid);
        Playtomic.API.StartCoroutine(SendProfileActionRequest(SECTION, LOAD, postdata, callback));
    }

    IEnumerator SendProfileActionRequest(string section, string action, Dictionary<string, object> postdata, Action<PlayerProfile, PResponse> callback)
    {
        var www = PRequest.Prepare(section, action, postdata);
        yield return www;

        var response = PRequest.Process(www);
        PlayerProfile profile = null;

        if(response.success)
            profile = new PlayerProfile((Dictionary<string,object>)response.json["profile"]);
        callback(profile, response);
    }

    public void Ping(string playerid)
    {
        var postdata = new Dictionary<string, object>();
        postdata.Add("playerid", playerid);
        Playtomic.API.StartCoroutine(SendPing(postdata));
    }

    IEnumerator SendPing(Dictionary<string, object> postdata)
    {
        var www = PRequest.Prepare(SECTION, PING, postdata);
        yield return www;
        var response = PRequest.Process(www);
    }

    public void Create(string UID, PlayerProfile profile, Action<PlayerProfile,PResponse> callback)
    {
        var postdata = new Dictionary<string, object>();
        postdata.Add("playerid", UID);
        postdata.Add("playername", profile.Name);
        postdata.Add("elo", 1400);
        postdata.Add("playtime", 1);
        postdata.Add("responsetime", 1);

        Playtomic.API.StartCoroutine(SendProfileActionRequest(SECTION, CREATE, postdata, callback));
    }
}
