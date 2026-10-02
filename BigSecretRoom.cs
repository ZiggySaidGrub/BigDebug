using UnityEngine;
using HarmonyLib;

namespace BigDebug;

[HarmonyPatch(typeof(SecretZoneController))]
public class BigSecretRoom
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(SecretZoneController.Awake))]
    internal static bool Awake(SecretZoneController __instance)
    {
        BaseManager.instance.devSettings.secretZoneActive = true;
        return false;
    }
}