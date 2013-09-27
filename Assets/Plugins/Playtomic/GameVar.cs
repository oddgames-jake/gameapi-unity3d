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

    public T GetValue<T>(T defaultValue)
    {
        if (typeof(T).IsAssignableFrom(this["value"].GetType()))
        {
            return (T) this["value"];
        }

        return defaultValue;
    }

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