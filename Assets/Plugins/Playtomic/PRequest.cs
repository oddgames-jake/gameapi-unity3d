using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class PRequest
{
	public static Dictionary<String, PResponse> Requests = new Dictionary<String, PResponse>();
	private static string APIURL;
	private static string PUBLICKEY;
	private static string PRIVATEKEY;
	
	public static void Initialise(string publickey, string privatekey, string apiurl)
	{
		if(!apiurl.EndsWith ("/")) 
		{
			apiurl += "/";
		}
		
		APIURL = apiurl + "v1?publickey=" + publickey;
		PRIVATEKEY = privatekey;
		PUBLICKEY = publickey;
	}
	
	public static WWW Prepare(string section, string action, Dictionary<string,object> postdata = null)
	{
		if(postdata == null)
		{
			postdata = new Dictionary<string,object>();
		}
		else 
		{
			
			postdata.Remove ("publickey");
			postdata.Remove ("section");
			postdata.Remove ("action");
		}
		
		postdata.Add ("publickey", PUBLICKEY);
		postdata.Add ("section", section);
		postdata.Add ("action", action);
	
        var json = LitJson.JsonMapper.ToJson(postdata);

		var post = new WWWForm();
		post.AddField("data", PEncode.Base64(json));
		post.AddField("hash", PEncode.MD5 (json + PRIVATEKEY));
		
		
		
		return new WWW(APIURL, post);
	}

    public static WWW LegacyPrepare(string section, string action, Dictionary<string, object> postdata = null)
    {
        if (postdata == null)
        {
            postdata = new Dictionary<string, object>();
        }
        else
        {

            postdata.Remove("publickey");
            postdata.Remove("section");
            postdata.Remove("action");
        }

        postdata.Add("publickey", PUBLICKEY);
        postdata.Add("section", section);
        postdata.Add("action", action);

        var json = PJSON.JsonEncode(postdata);

        var post = new WWWForm();
        post.AddField("data", PEncode.Base64(json));
        post.AddField("hash", PEncode.MD5(json + PRIVATEKEY));



        return new WWW(APIURL, post);
    }
	
	public static PResponse Process(WWW www)
	{
		if(www == null)
			return PResponse.GeneralError(1);
		
		if (www.error != null)
			return PResponse.GeneralError(www.error);

		if (string.IsNullOrEmpty(www.text))
			return PResponse.Error(1);

		var results = (Dictionary<string,object>)PJSON.JsonDecode(www.text);
		
		if(!results.ContainsKey("success") || !results.ContainsKey("errorcode"))
			return PResponse.GeneralError(1);
				
		var response = new PResponse();
		response.success = (bool)results["success"];
		response.errorcode = (int)(double)results["errorcode"];
		response.json = results;
		return response;
	}

    public static QuickResponse<T> FastProcess<T>(WWW www) where T : ResponseBase
    {
        var response = new QuickResponse<T>();

        if (!PRequest.WWWSuccess(www))
        {
            response.errorcode = 1;
            response.success = false;
            return response;
        }

        var results = LitJson.JsonMapper.ToObject<T>(www.text);
        response.success = results.success;
        response.errorcode = results.errorcode;

        response.ResponseObject = results;
        return response;
    }

    public static bool WWWSuccess(WWW www)
    {
        if (www == null)
            return false;

        if (www.error != null)
            return false;

        if (string.IsNullOrEmpty(www.text))
            return false;

        return true;
    }

    /// <summary>
    /// Processes some returned text from a www. Technically not threadsafe, but in current 
    /// usage context is 100 % safe. Works around UNitys limitations on only main thread 
    /// accessing Unity stuff, as such error checking on the WWW MUST BE DONE BEFORE CALLING 
    /// THIS using PRequest.WWWSuccess.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static QuickResponse<T> FastProcessUnityThread<T>(string data) where T : ResponseBase
    {
        var response = new QuickResponse<T>();

        var results = LitJson.JsonMapper.ToObject<T>(data);
        response.success = results.success;
        response.errorcode = results.errorcode;

        response.ResponseObject = results;
        return response;
    }
}