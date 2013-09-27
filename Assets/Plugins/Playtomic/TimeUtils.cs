using System;

public class TimeUtils
{
    public static int ToUnixTime(DateTime input)
    {
        return (int)(input.ToUniversalTime() - 
            new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }

    public static DateTime FromUnixTime(object timestamp)
    {
        double dbl;
        if (double.TryParse(timestamp.ToString(), out dbl))
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) +
                TimeSpan.FromSeconds(dbl)).ToLocalTime();
        }
        return DateTime.Now;
    }
}
