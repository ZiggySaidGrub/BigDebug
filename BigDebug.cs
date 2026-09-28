using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using ImGuiNET;

namespace BigDebug;

[BepInDependency(DearImGuiInjection.Metadata.GUID)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class BigDebug : BasePlugin
{
    internal static new ManualLogSource Log;

    public override void Load()
    {
        // Plugin startup logic
        Log = base.Log;
        BindConfigs();

        new Harmony("horse.smots.bigdebug").PatchAll();

        RegisterTypes();

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public void BindConfigs()
    {
        BigFly.flightKey =          Config.Bind("Bindings", "FlightKey", KeyCode.F11, "The key to toggle flying");
        BigFly.moveSpeed =          Config.Bind("Flight", "FlySpeed", 10f, "How fast you move when flying");
        BigFly.fastMoveMultiplier = Config.Bind("Flight", "FlightSpeedMultipler", 2.5f, "How much faster you go when sprinting while flying");
        
        BigUI.menuKey =             Config.Bind("Bindings", "MenuKey", KeyCode.F10, "The key to toggle the debug menu");
        BigUI.unlockCursorKey =     Config.Bind("Bindings", "UnlockCursorKey", KeyCode.Delete, "The key to toggle your cursor being unlocked");

        BigUI.twelveHourClock =     Config.Bind("Time", "TwelveHourClock", true, "If true, tells time using a twelve hour clock format");
    }

    private readonly Type[] typesToRegister =
    {
        typeof(BigDebugInjector),
        typeof(BigFly),
        typeof(BigTeleporter),
        typeof(BigUI),
    };
    public void RegisterTypes()
    {
        foreach (Type type in typesToRegister)
        {
            ClassInjector.RegisterTypeInIl2Cpp(type);
        }
    }
}
