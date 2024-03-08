using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

using UnityEngine.InputSystem;
using UnityEngine;

using LethalCompanyInputUtils;
using LethalCompanyInputUtils.Api;

using System;
namespace TeleportDecline
{
    [BepInPlugin(modGUID, modName, modVersion)]

    [BepInDependency(LethalCompanyInputUtilsPlugin.ModId, BepInDependency.DependencyFlags.HardDependency)]
    public class TeleportDeclineBase : BaseUnityPlugin
    {
        public const string modGUID = "MasterAli2.TeleportDecline";
        public const string modName = "Teleport Decline";
        public const string modVersion = "0.0.1";

        private readonly Harmony harmony = new Harmony(modGUID);
        internal ManualLogSource? mls;

        public static TeleportDeclineBase? instance;

        public bool isTeleporting = false;
        public bool declineTeleport = false;

        public bool wasInElevator;
        public bool wasInHangarShipRoom;
        public bool wasInsideFactory;
        public float oldAverageVelocity;
        public Vector3 oldVelocityLastFrame;

        public AudioClip? declieneAudio;

        void Awake()
        {

            if (instance == null)
            {
                instance = this;
            }

            mls = this.Logger;

            TeleportDeclineInputClass.instance.DeclineKey.performed += DeclineTeleport;
            ApplyPatches();

            mls.LogInfo(modName + "Loaded!");

        }

        void ApplyPatches()
        {
            harmony.PatchAll(typeof(TeleportDeclineBase));
            harmony.PatchAll(typeof(Patches.Patch));
        }


        public void DeclineTeleport(InputAction.CallbackContext context)
        {
            if (declieneAudio == null) declieneAudio = UnityEngine.Object.FindObjectOfType<Terminal>().leaveTerminalSFX;
            if (!context.performed || !isTeleporting) return;

            declineTeleport = true;
            StartOfRound.Instance.localPlayerController.beamUpParticle.Stop();

            HUDManager.Instance.tipsPanelBody.text = "Declining teleport...";

            StartOfRound.Instance.localPlayerController.movementAudio.PlayOneShot(declieneAudio);
            WalkieTalkie.TransmitOneShotAudio(StartOfRound.Instance.localPlayerController.movementAudio, declieneAudio);

        }
    }


    public class TeleportDeclineInputClass : LcInputActions
    {
        public static TeleportDeclineInputClass instance = new();

        [InputAction("<Keyboard>/h", Name = "Decline Teleport")]
        public InputAction DeclineKey { get; set; }


    }
}
