using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

using UnityEngine.InputSystem;
using LethalCompanyInputUtils.Api;

using StaticNetcodeLib;
using Unity.Netcode;

namespace TeleportDecline
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(StaticNetcodeLib.StaticNetcodeLib.Guid, BepInDependency.DependencyFlags.HardDependency)]
    public class TeleportDeclineBase : BaseUnityPlugin
    {
        public const string GUID = "MasterAli2.TeleportDecline";
        public const string NAME = "Teleport Decline";
        public const string VERSION = "1.1.0";
        public const string AUTHOR = "MasterAli2";

        private readonly Harmony harmony = new Harmony(GUID);
        internal ManualLogSource mls;

        public static TeleportDeclineBase instance;

        public bool isTeleporting = false;
        public ShipTeleporter teleporter;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            mls = this.Logger;

            TeleportDeclineInput.instance.DeclineKey.performed += DeclineTeleport;
            ApplyPatches();

            mls.LogInfo($"{GUID} v{VERSION} has loaded!");
        }

        void ApplyPatches()
        {
            mls.LogInfo("Patching...");
            harmony.PatchAll(typeof(Patches.Patch));
        }

        public void DeclineTeleport(InputAction.CallbackContext context)
        {
            if (!context.performed || !isTeleporting) return;

            mls.LogInfo("Stopping teleport!");

            StartOfRound.Instance.localPlayerController.beamUpParticle.Stop();
            HUDManager.Instance.tipsPanelBody.text = "Declining teleport...";

            
            teleporter.StopCoroutine(teleporter.beamUpPlayerCoroutine);

            TeleportDeclineNetcode.DeclineTeleportClientRpc();
            isTeleporting = false;
        }
    }

    public class TeleportDeclineInput : LcInputActions
    {
        public static TeleportDeclineInput instance = new();

        [InputAction("<Keyboard>/h", Name = "Decline Teleport")]
        public InputAction DeclineKey { get; set; }
    }

    [StaticNetcode]
    public static class TeleportDeclineNetcode
    {
        [ClientRpc]
        public static void DeclineTeleportClientRpc()
        {
            if (TeleportDeclineBase.instance.isTeleporting || !StartOfRound.Instance.localPlayerController.isInHangarShipRoom) return;
            
            HUDManager.Instance.DisplayTip("Teleport Decline", "That teleport got declined");
        }
    }
}
