using System.Runtime.Intrinsics.X86;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace BigDebug;

[HarmonyPatch(typeof(PlayerMover))]
public class BigFly : MonoBehaviour
{
    // [HarmonyPrefix]
    // [HarmonyPatch(nameof(PlayerMover.Update))]
    // internal static bool PMUpdate(PlayerMover __instance)
    // {
    //     if (!__instance.playerNetworking.isLocalPlayer) return true;

    //     BigFly flier = __instance.pc.GetComponent<BigFly>();
    //     if (flier == null) return true;

    //     return !bigFlying;
    // }

    private void Update()
    {
        if (Input.GetKeyDown(BigDebug.BigConfig.flightKey.Value))
        {
            bigFlying = !bigFlying;
            if (bigFlying)
            {
                pc.rb.useGravity = false;
                pc.rb.detectCollisions = false;
                pc.faller.ignoreFalling = true;
                pc.mover.bypassUpdate = true;
                pc.mover.ignoreAirbourneVelocity = true;
            } else
            {
                pc.rb.useGravity = true;
                pc.rb.detectCollisions = true;
                pc.faller.ignoreFalling = false;
                pc.mover.bypassUpdate = false;
                pc.mover.ignoreAirbourneVelocity = false;
                pc.faller.ClearNextFall();
            }
        }

        if (bigFlying) HandleFlightMovement();
    }

    public PlayerCharacter pc;
    public bool bigFlying = false;
    public void HandleFlightMovement()
    {
        if (pc.texter.isLocalPlayerTextChatting) return;

        Transform mainCamTransform = Camera.main.transform;
        Rewired.Player rewiredPlayer = pc.inputPlayer;
		float num = BigDebug.BigConfig.moveSpeed.Value;
		if (rewiredPlayer.GetButton(15)) // CONTROLS_SPRINT
		{
			num *= BigDebug.BigConfig.fastMoveMultiplier.Value;
		}
		Vector3 val = Vector3.zero;

        val += mainCamTransform.forward * rewiredPlayer.GetAxis(8); // forward/backwards axis
        val += mainCamTransform.right * rewiredPlayer.GetAxis(9); // right/left axis

		if (rewiredPlayer.GetButton(16)) // CONTROLS_JUMP
		{
			val += Vector3.up;
		}
		if (rewiredPlayer.GetButton(20)) // CONTROLS_CROUCH
		{
			val += Vector3.down;
		}
        Rigidbody rb = pc.rb;
        rb.velocity = Vector3.zero;
        rb.MovePosition(rb.position + val * num * Time.deltaTime);
        pc.transform.position = rb.position;
    }
}