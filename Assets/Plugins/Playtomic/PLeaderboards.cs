using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class PLeaderboards
{
	private const string SECTION = "leaderboards";
	private const string SAVEANDLIST = "saveandlist";
	private const string SAVE = "save";
	private const string LIST = "list";
	
	/**
	 * Saves a player's score
	 * @param	score	PlayerScore	The PlayerScore object 
	 * @param	callback	Action<PResponse> Your callback method
	 */
	public void Save(PlayerScore score, Action<PResponse> callback)
	{
		Save<PlayerScore>(score,callback);
	}
	
	public void Save<T>(T score, Action<PResponse> callback) where T : PlayerScore
	{
		Playtomic.API.StartCoroutine(SendSaveRequest(SECTION, SAVE, score, callback));
	}

	public void SaveAndList(PlayerScore score, Action<List<PlayerScore>, int, PResponse> callback)
	{
		SaveAndList<PlayerScore>(score,callback);
	}
	
	public void SaveAndList<T>(T score, Action<List<T>, int, PResponse> callback) where T : PlayerScore
	{
		Playtomic.API.StartCoroutine(SendListRequest(SECTION, SAVEANDLIST, score, callback));
	}

    public void GetSurroundingScores<T>(T score, int above, int below, Action<List<T>, int, PResponse> callback) where T : PlayerScore
    {
        var postdata = score;
        postdata.Add("afterlength", below);
        postdata.Add("above", above);
        Playtomic.API.StartCoroutine(SendListRequest(SECTION, SAVEANDLIST, score, callback));
    }

    public void GetReplay<T>(T score, Action<List<T>, int, PResponse> callback) where T : PlayerScore
    {
        Playtomic.API.StartCoroutine(SendListRequest(SECTION, "getreplay", score, callback));
    }

    public void GetRivalReplays<T>(T score, int pointDifference, int returnAmount, Action<List<T>, int, PResponse> callback) where T : PlayerScore
    {
        var postdata = new Dictionary<string, object>();
        postdata.Add("points", score.points);
        postdata.Add("table", "France");
        postdata.Add("pointdifference", pointDifference);
        postdata.Add("returnamount", returnAmount);
        Playtomic.API.StartCoroutine(SendListRequest(SECTION, "getrivalreplays", postdata, callback));
    }

	/**
	 * Lists scores
	 * @param	options	Dictionary<string,object>	The listing options
	 * @param	callback	Action<List<PlayerScore>, int, PResponse>	Your callback function
	 */
	
	public void List(PLeaderboardOptions options, Action<List<PlayerScore>, int, PResponse> callback)
	{
		List<PlayerScore>(options,callback);		
	}
	
	public void List<T>(PLeaderboardOptions options, Action<List<T>, int, PResponse> callback)  where T : PlayerScore
	{	
		Playtomic.API.StartCoroutine(SendListRequest<T>(SECTION, LIST, options, callback));
	}
	
	private IEnumerator SendSaveRequest(string section, string action, Dictionary<string,object> postdata, Action<PResponse> callback)
	{ 
		WWW www = PRequest.Prepare (section, action, postdata);
		yield return www;
		
		PResponse response = PRequest.Process(www);
		callback(response);
	}
	
	
	private IEnumerator SendListRequest<T>(string section, string action, Dictionary<string,object> postdata, Action<List<T>, int, PResponse> callback) where T : PlayerScore
	{ 
		WWW www = PRequest.Prepare (section, action, postdata);
	
		yield return www;
	
		var response = PRequest.Process(www);
		var data = response.json;
		
		List<T> scores = new List<T>();
		
		int numscores = 0;

		if (response.success)
		{
	
			if (data.ContainsKey("numscores"))
			{
			
				int.TryParse(data["numscores"].ToString(), out numscores);
				
			}
			
			if (data.ContainsKey("scores"))
			{
	
				if (data["scores"] is IList)
				{

					foreach(IDictionary score in (IList) data["scores"])
					{
						
						scores.Add((T) Activator.CreateInstance(typeof(T), new object[] { score }));
						
					}
					
				}
				
			}
			
		}
		
		callback(scores, numscores, response);
	}

}
