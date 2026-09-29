using System.IO;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace BigDebug;

public class BigConfig
{
    public ConfigFile Config;

    public string WarpsFolder;

    public ConfigEntry<KeyCode> flightKey;
    public ConfigEntry<float> moveSpeed;
    public ConfigEntry<float> fastMoveMultiplier;

    public ConfigEntry<KeyCode> menuKey;
    public ConfigEntry<KeyCode> unlockCursorKey;

    public ConfigEntry<bool> twelveHourClock;

    internal BigConfig(ConfigFile config)
    {
        Config = config;

        BindConfigs();

        WarpsFolder = Path.Join(Paths.ConfigPath, "BigDebugWarps/");
        if (!Directory.Exists(WarpsFolder))
        {
            Directory.CreateDirectory(WarpsFolder);
        }
    }

    public void BindConfigs()
    {
        flightKey =              Config.Bind("Bindings", "FlightKey", KeyCode.F11, "The key to toggle flying");
        moveSpeed =              Config.Bind("Flight", "FlySpeed", 10f, "How fast you move when flying");
        fastMoveMultiplier =     Config.Bind("Flight", "FlightSpeedMultipler", 2.5f, "How much faster you go when sprinting while flying");
        
        menuKey =                 Config.Bind("Bindings", "MenuKey", KeyCode.F10, "The key to toggle the debug menu");
        unlockCursorKey =         Config.Bind("Bindings", "UnlockCursorKey", KeyCode.Delete, "The key to toggle your cursor being unlocked");

        twelveHourClock =    Config.Bind("Time", "TwelveHourClock", true, "If true, tells time using a twelve hour clock format");
    }
}