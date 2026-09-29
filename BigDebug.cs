using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;

namespace BigDebug;

[BepInDependency(DearImGuiInjection.Metadata.GUID)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class BigDebug : BasePlugin
{
    internal static new ManualLogSource Log;
    internal static BigConfig BigConfig;

    public override void Load()
    {
        // Plugin startup logic
        Log = base.Log;
        BigConfig = new BigConfig(Config);

        new Harmony("horse.smots.bigdebug").PatchAll();

        RegisterTypes();

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
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
