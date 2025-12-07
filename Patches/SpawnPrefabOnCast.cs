using HarmonyLib;
using ProjectM;
using Unity.Entities;
using Unity.Collections;
using System;
using System.Reflection;

namespace RetroCamera.Patches
{
    [HarmonyPatch]
    internal static class AbilityStartCastingSystem_ServerPatch
    {
        static MethodBase TargetMethod()
        {
            var type = typeof(AbilityStartCastingSystem_Server);
            var method = AccessTools.Method(
                type,
                "SpawnPrefabOnCast",
                new Type[] {
                    typeof(SystemState).MakeByRefType(),
                    typeof(Entity),
                    typeof(AbilityStartCastingSystem_Server.Input).MakeByRefType(),
                    typeof(AbilityBar_Shared).MakeByRefType(),
                    typeof(NativeArray<AbilitySpawnPrefabOnCast>)
                }
            );

            Plugin.LogInstance.LogInfo(method != null
                ? $"[Patch] Found method: {method.Name}"
                : "[Patch] Method not found!");

            return method;
        }

        static void Prefix(
            ref SystemState state,
            Entity character,
            ref AbilityStartCastingSystem_Server.Input input,
            ref AbilityBar_Shared abilityBar,
            NativeArray<AbilitySpawnPrefabOnCast> spawnPrefabs
        )
        {
            Plugin.LogInstance.LogInfo($"[Patch] SpawnPrefabOnCast called for entity {character.Index}");
        }
    }
}
