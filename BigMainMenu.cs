using System.Text;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BigDebug;

[HarmonyPatch(typeof(VersionDisplay))]
internal static class BigMainMenu
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(VersionDisplay.Awake))]
    internal static void Awake(VersionDisplay __instance)
    {
        __instance.textField.text += $"\nBig Debug v{MyPluginInfo.PLUGIN_VERSION}";
    }
}