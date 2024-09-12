using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

using UnityEngine.InputSystem;
using UnityEngine;

using LethalCompanyInputUtils;
using LethalCompanyInputUtils.Api;

namespace TeleportDecline
{

    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
    public class TeleportDeclineBase : BaseUnityPlugin
    {
        public const string GUID = "MasterAli2.TeleportDecline";
        public const string NAME = "Teleport Decline";
        public const string VERSION = "1.0.2";
        public const string AUTHOR = "MasterAli2";

        private readonly Harmony harmony = new Harmony(GUID);
        internal ManualLogSource mls;

        public static TeleportDeclineBase instance;

        public bool isTeleporting = false;
        public ShipTeleporter teleporter;

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
            if (declieneAudio == null) declieneAudio = UnityEngine.Object.FindObjectOfType<Terminal>().leaveTerminalSFX;

            mls.LogInfo("Stopping teleport!");

            StartOfRound.Instance.localPlayerController.beamUpParticle.Stop();

            HUDManager.Instance.tipsPanelBody.text = "Declining teleport...";

            StartOfRound.Instance.localPlayerController.movementAudio.PlayOneShot(declieneAudio);
            WalkieTalkie.TransmitOneShotAudio(StartOfRound.Instance.localPlayerController.movementAudio, declieneAudio);

            isTeleporting = false;
            teleporter.StopCoroutine(teleporter.beamUpPlayerCoroutine);
        }
    }

    public class TeleportDeclineInputClass : LcInputActions
    {
        public static TeleportDeclineInputClass instance = new();

        [InputAction("<Keyboard>/h", Name = "Decline Teleport")]
        public InputAction DeclineKey { get; set; }
    }
}
