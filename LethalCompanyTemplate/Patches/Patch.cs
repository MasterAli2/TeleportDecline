using HarmonyLib;
using UnityEngine.InputSystem;

namespace TeleportDecline.Patches
{
    internal class Patch
    {
        [HarmonyPatch(typeof(ShipTeleporter), "PressTeleportButtonClientRpc")]
        [HarmonyPostfix]
        static void PressTeleportButtonClientRpcPostfix(ShipTeleporter __instance)
        {
            if (StartOfRound.Instance.mapScreen.targetedPlayer != StartOfRound.Instance.localPlayerController || 
                StartOfRound.Instance.localPlayerController.isPlayerDead ||
                __instance.isInverseTeleporter)
                return;

            HUDManager.Instance.DisplayTip("Teleporting!", "Press " + TeleportDeclineInput.instance.DeclineKey.GetBindingDisplayString().Split("|")[0] + "to stop teleport");

            TeleportDeclineBase.instance.isTeleporting = true;
            TeleportDeclineBase.instance.teleporter = __instance;
        }

        [HarmonyPatch(typeof(ShipTeleporter), "SetPlayerTeleporterId")]
        [HarmonyPostfix]
        static void SetPlayerTeleporterIdPostfix(ShipTeleporter __instance, ref int teleporterId)
        {
            if (TeleportDeclineBase.instance.teleporter != __instance) return;

            TeleportDeclineBase.instance.isTeleporting = false;
        }
    }
}
