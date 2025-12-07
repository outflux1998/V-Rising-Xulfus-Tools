using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Unity.Entities;
using System.Reflection;

namespace RetroCamera.Patches
{
    [BepInPlugin("com.retro.camera", "RetroCamera", "1.4.3")]
    internal class Plugin : BasePlugin
    {
        private Harmony _harmony;
        internal static Plugin Instance { get; private set; }
        public static ManualLogSource LogInstance => Instance.Log;

        public override void Load()
        {
            Instance = this;

            _harmony = new Harmony("com.retro.camera");

            // Patch manual do TryGetCurrentAbilityEntities
            var method = AccessTools.Method(
                typeof(ProjectM.AbilityStartCastingSystem_Client),
                "TryGetCurrentAbilityEntities"
            );

            if (method != null)
            {
                LogInstance.LogInfo("[Patch] Found method: TryGetCurrentAbilityEntities");
                _harmony.Patch(method, postfix: new HarmonyMethod(typeof(TryGetCurrentAbilityEntitiesPatch).GetMethod(nameof(TryGetCurrentAbilityEntitiesPatch.Postfix))));
            }
            else
            {
                LogInstance.LogWarning("[Patch] Failed to find TryGetCurrentAbilityEntities");
            }

            LogInstance.LogInfo("RetroCamera[1.4.3] loaded!");
        }

        public override bool Unload()
        {
            _harmony.UnpatchSelf();
            return true;
        }
    }
}
