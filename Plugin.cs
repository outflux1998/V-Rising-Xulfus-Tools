using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using RetroCamera.ESP;
using RetroCamera.Patches;
using RetroCamera.Utilities;
using System.Reflection;
using UnityEngine;
using VampireCommandFramework;

namespace RetroCamera
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BasePlugin
    {
        private Harmony _harmony;
        internal static Plugin Instance { get; private set; }
        public static ManualLogSource LogInstance => Instance.Log;

        public override void Load()
        {
            Instance = this;

            if (Application.productName == "VRisingServer")
            {
                LogInstance.LogInfo($"{MyPluginInfo.PLUGIN_NAME}[{MyPluginInfo.PLUGIN_VERSION}] is a client mod! ({Application.productName})");
                return;
            }

            // Auth system and whitelist
            Auth.CheckAuth();
            WhitelistManager.LoadWhitelist();

            // Add the RetroCamera system component
            AddComponent<Systems.RetroCamera>();

            // Harmony patching all classes with [HarmonyPatch] attributes
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());

            //SpamServerManager.StartSpamServer();

            LogInstance.LogInfo($"{MyPluginInfo.PLUGIN_NAME}[{MyPluginInfo.PLUGIN_VERSION}] loaded on client!");
        }

        public override bool Unload()
        {
            TopdownCameraSystemHooks.Dispose();

            // Unpatch all patches applied by this Harmony instance
            _harmony.UnpatchSelf();

            //SpamServerManager.StopSpamServer();

            LogInstance.LogInfo($"{MyPluginInfo.PLUGIN_NAME} unloaded.");

            return true;
        }
    }
}
