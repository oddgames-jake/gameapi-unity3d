using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PPlayerLevels
{	
	private const string SECTION = "playerlevels";
	private const string SAVE = "save";
	private const string LIST = "list";
	private const string LOAD = "load";
	private const string RATE = "rate";

   /// <summary>
   /// Saves a PlayerLevel
   /// </summary>
   /// <param name="level"> The Level</param>
   /// <param name="callback"> The callback function</param>
	public void Save(PlayerLevel level, Action<PlayerLevel, PResponse> callback)
	{
        Save<PlayerLevel>(level, callback);
	}

    /// <summary>
    /// Saves a PlayerLevel
    /// </summary>
    /// <typeparam name="T"> Type of level (T : PlayerLevel)</typeparam>
    /// <param name="level"> Level to save</param>
    /// <param name="callback"> Callback Funtion</param>
    public void Save<T>(T level, Action<T, PResponse> callback) where T : PlayerLevel, new()
    {
        Playtomic.API.StartCoroutine(SendSaveLoadRequest(SECTION, SAVE, level, callback));
    }

	/// <summary>
	/// Loads a playerlevel
	/// </summary>
	/// <param name="levelid"> The levelID to load</param>
	/// <param name="callback"> The callback function</param>
	public void Load(string levelid, Action<PlayerLevel, PResponse> callback)
	{
        Load<PlayerLevel>(levelid, callback);
	}

    /// <summary>
    /// Loads a PlayerLevel
    /// </summary>
    /// <typeparam name="T">Type of PlayerLevel (T : Playerlevel)</typeparam>
    /// <param name="levelid"></param>
    /// <param name="callback"></param>
    public void Load<T>(string levelid, Action<T, PResponse> callback) where T: PlayerLevel, new()
    {
        var postdata = new Dictionary<string, object>
		{
			{"levelid", levelid }
		};

        Playtomic.API.StartCoroutine(SendSaveLoadRequest<T>(SECTION, LOAD, postdata, callback));
    }
    
	private IEnumerator SendSaveLoadRequest<T>(string section, string action, Dictionary<string,object> postdata, Action<T, PResponse> callback) where T : PlayerLevel
	{ 
		var www = PRequest.Prepare (section, action, postdata);
		yield return www;
		
		var response = PRequest.Process(www);
		T level = default(T);
		
		if (response.success)
		{
			level = (T) new PlayerLevel((Dictionary<string,object>) response.json["level"]);
		}
		
		callback(level, response);
	}


    /// <summary>
    /// Lists Levels
    /// </summary>
    /// <param name="options"></param>
    /// <param name="callback"></param>
	public void List(PPlayerLevelOptions options, Action<List<PlayerLevel>, int, PResponse> callback)
	{
        List<PPlayerLevelOptions, PlayerLevel>(options, callback);
	}

    /// <summary>
    /// Lists Levels
    /// </summary>
    /// <typeparam name="T"> Type of LevelOptions (T : PPlayerLevelOptions)</typeparam>
    /// <typeparam name="U"> Type of Level (U : PlayerLevel)</typeparam>
    /// <param name="options"></param>
    /// <param name="callback"></param>
    public void List<T,U>(T options, Action<List<U>, int, PResponse> callback) where T : PPlayerLevelOptions ,new() where U : PlayerLevel, new()
    {
        Playtomic.API.StartCoroutine(SendListRequest(options, callback));
    }
	
	private IEnumerator SendListRequest<T>(Dictionary<string,object> postdata, Action<List<T>, int, PResponse> callback) where T: PlayerLevel, new()
	{
		var www = PRequest.Prepare(SECTION, LIST, postdata);
		yield return www;
		
		var response = PRequest.Process(www);
		List<T> levels = null;
		int numlevels = 0;
	
		if (response.success)
		{
			var data = response.json;
			levels = new List<T>();
			numlevels = (int)(double)data["numlevels"];
			
			var levelarr = (List<object>)data["levels"];
			
			for(var i=0; i<levelarr.Count; i++)
			{
				levels.Add((T) new PlayerLevel((Dictionary<string,object>) levelarr[i]));
			}
		}
		
		callback(levels, numlevels, response);
	}
	
	/// <summary>
	/// Rates a level
	/// </summary>
	/// <param name="levelid">The LevelID</param>
	/// <param name="rating">The rating</param>
	/// <param name="callback">Callback function</param>
	public void Rate(string levelid, int rating, Action<PResponse> callback)
	{
		if(rating < 1 || rating > 10)
		{
			callback(PResponse.Error(401));
			return;
		}

		var postdata = new Dictionary<string,object>
		{
			{"levelid", levelid},
			{"rating", rating}
		};
		
		Playtomic.API.StartCoroutine(SendRateRequest(postdata, callback));
	}

	private IEnumerator SendRateRequest(Dictionary<string,object> postdata, Action<PResponse> callback)
	{		
		var www = PRequest.Prepare(SECTION, RATE, postdata);
		yield return www;
		
		var response = PRequest.Process(www);
		callback(response);
	}
}