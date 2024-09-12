using HarmonyLib;
using UnityEngine.InputSystem;

namespace TeleportDecline.Patches
{
    internal class Patch
    {
        [HarmonyPatch(typeof(ShipTeleporter), "PressTeleportButtonClientRpc")]
        [HarmonyPostfix]
        static void beamUpPlayerPrefix(ShipTeleporter __instance)
        {
            if (StartOfRound.Instance.mapScreen.targetedPlayer != StartOfRound.Instance.localPlayerController || 
                StartOfRound.Instance.localPlayerController.isPlayerDead ||
                __instance.isInverseTeleporter)
                return;

            HUDManager.Instance.DisplayTip("Teleporting!", "Press " + TeleportDeclineInputClass.instance.DeclineKey.GetBindingDisplayString().Split("|")[0] + "to stop teleport");
            TeleportDeclineBase.instance.isTeleporting = true;
            TeleportDeclineBase.instance.teleporter = __instance;
        }
    }
}
