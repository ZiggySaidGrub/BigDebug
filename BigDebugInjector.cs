using HarmonyLib;
using UnityEngine;

namespace BigDebug;

public class BigDebugInjector : MonoBehaviour
{
    public PlayerCharacter pc;
    public BigFly flier;
    public BigUI UI;
    void Awake()
    {
        pc = GetComponent<PlayerCharacter>();
        flier = gameObject.AddComponent<BigFly>();
        flier.pc = pc;
        gameObject.AddComponent<BigTeleporter>();
        UI = gameObject.AddComponent<BigUI>();
    }
}

[HarmonyPatch(typeof(PlayerCharacter))]
public class PCInjectPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerCharacter.Start))]
    internal static void Start(PlayerCharacter __instance)
    {
        if (!__instance.isLocalPlayer) return;

        BigDebugInjector injector = __instance.gameObject.AddComponent<BigDebugInjector>();
    }
}