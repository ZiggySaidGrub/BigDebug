using System;
using BepInEx.Configuration;
using BigDebug;

public static class TimeFormat
{
    public static string Format(float time)
    {
        string ampm = "AM";
        int hour = (int) Math.Floor(time);

        float minuteDecimal = time - hour;
        int minute = (int) Math.Floor(minuteDecimal * 60);

        if (BigDebug.BigDebug.BigConfig.twelveHourClock.Value)
        {
            if (hour >= 12) ampm = "PM";
            hour %= 12;
            if (hour == 0) hour = 12;
        }

        return $"{hour}:{minute:D2}{(BigDebug.BigDebug.BigConfig.twelveHourClock.Value ? $" {ampm}" : "")}";
    }
}