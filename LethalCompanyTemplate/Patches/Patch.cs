using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace TeleportDecline.Patches
{
    internal class Patch
    {
        [HarmonyPatch(typeof(ShipTeleporter), "beamUpPlayer", MethodType.Enumerator)]
        [HarmonyPrefix]
        static void beamUpPlayerPrefix(ShipTeleporter __instance)
        {
            if (StartOfRound.Instance.mapScreen.targetedPlayer != StartOfRound.Instance.localPlayerController || 
                StartOfRound.Instance.localPlayerController.isPlayerDead)
                return;

            HUDManager.Instance.DisplayTip("Teleporting!", "Press " + TeleportDeclineInputClass.instance.DeclineKey.GetBindingDisplayString().Split("|")[0] + "to stop teleport");
            TeleportDeclineBase.instance.isTeleporting = true;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "TeleportPlayer")]
        [HarmonyPrefix]
        static bool TeleportPlayerPostfix(PlayerControllerB __instance)
        {
            TeleportDeclineBase Base = TeleportDeclineBase.instance;
            if (__instance == StartOfRound.Instance.localPlayerController && 
                Base.declineTeleport && 
                Base.isTeleporting && 
                !StartOfRound.Instance.localPlayerController.isPlayerDead)
            {
                Base.isTeleporting = false;
                Base.declineTeleport = false;

                __instance.isInElevator = Base.wasInElevator;
                __instance.isInHangarShipRoom = Base.wasInHangarShipRoom;
                __instance.isInsideFactory = Base.wasInsideFactory;
                __instance.averageVelocity = Base.oldAverageVelocity;
                __instance.velocityLastFrame = Base.oldVelocityLastFrame;

                return false;
            }

            Base.isTeleporting = false;
            Base.declineTeleport = false;
            return true;
        }


        [HarmonyPatch(typeof(PlayerControllerB), "DropAllHeldItems")]
        [HarmonyPrefix]
        static bool DropAllHeldItemsPrefix(PlayerControllerB __instance)
        {
            if (__instance == StartOfRound.Instance.localPlayerController &&
                TeleportDeclineBase.instance.declineTeleport &&
                TeleportDeclineBase.instance.isTeleporting &&
                !StartOfRound.Instance.localPlayerController.isPlayerDead)
            {
                TeleportDeclineBase Base = TeleportDeclineBase.instance;

                Base.wasInElevator = __instance.isInElevator;
                Base.wasInHangarShipRoom = __instance.isInHangarShipRoom;
                Base.wasInsideFactory = __instance.isInsideFactory;
                Base.oldAverageVelocity = __instance.averageVelocity;
                Base.oldVelocityLastFrame = __instance.velocityLastFrame;

                return false;
            }
            return true;
        }

    }
}
