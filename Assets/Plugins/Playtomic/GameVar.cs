using System;
using System.Collections.Generic;

public class GameVar : PDictionary
{
	
	public GameVar(): base() {}
	
	public GameVar(Dictionary<string,object> data): base(data) {}
	
	public string name
	{
		get { return GetString ("name"); }
		set { SetProperty ("name", value); }
	}

	public string value
	{
		get { return GetString ("value"); }
		set { SetProperty ("value", value); }
	}

    /// <summary>
    /// Attempts to pull a Generic Value out of GameVar
    /// </summary>
    /// <typeparam name="T">Type of Value to retrieve</typeparam>
    /// <param name="defaultValue"> Default value to return should it fail</param>
    /// <returns> The variable if successful or the provided defaultValue</returns>
    public T GetValue<T>(T defaultValue)
    {
        if (typeof(T).IsAssignableFrom(this["value"].GetType()))
        {
            return (T) this["value"];
        }

        return defaultValue;
    }

    /// <summary>
    /// Attempts to pull a Generic Value out of GameVar
    /// </summary>
    /// <typeparam name="T"> Type of Value to retrieve</typeparam>
    /// <param name="defaultValue"> The Default to retun in error cases</param>
    /// <param name="returnValue"> The output. is == defaultValue if failed</param>
    /// <returns> Succes/Fail of retrieval of var</returns>
    public bool GetValue<T>(T defaultValue, out T returnValue)
    {
        if (typeof(T).IsAssignableFrom(this["value"].GetType()))
        {
            returnValue = (T) this["value"];
            return true;
        }

        returnValue = default(T);
        return false;
    }
}