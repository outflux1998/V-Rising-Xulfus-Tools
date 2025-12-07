using Unity.Entities;
using Unity.Collections;
using ProjectM;
using RetroCamera.ESP;

namespace RetroCamera.Utilities
{
    internal static class DumpLocalAbilities
    {
        private static EntityAbilityInput? lastInput = null;
        private static AbilityBar_Client? lastClient = null;
        private static AbilityBar_Shared? lastShared = null;

        public static void Run(Entity player)
        {
            var entityManager = EntityList.GetClientWorld().EntityManager;

            // -------- EntityAbilityInput --------
            if (entityManager.HasComponent<EntityAbilityInput>(player))
            {
                var input = entityManager.GetComponentData<EntityAbilityInput>(player);

                if (!lastInput.HasValue || input.ActiveCastGroup.Index != lastInput.Value.ActiveCastGroup.Index)
                {
                    Plugin.LogInstance.LogInfo($"[EntityAbilityInput] ActiveCastGroup: {input.ActiveCastGroup.Index}");
                }

                if (!lastInput.HasValue || input.CastInput != lastInput.Value.CastInput)
                {
                    Plugin.LogInstance.LogInfo($"[EntityAbilityInput] CastInput: {input.CastInput}");
                }

                lastInput = input;
            }

            // -------- AbilityBar_Client --------
            if (entityManager.HasComponent<AbilityBar_Client>(player))
            {
                var client = entityManager.GetComponentData<AbilityBar_Client>(player);

                if (!lastClient.HasValue || client.ClientCastGroupNetworkId != lastClient.Value.ClientCastGroupNetworkId)
                {
                    Plugin.LogInstance.LogInfo($"[AbilityBar_Client] ClientCastGroupNetworkId: {client.ClientCastGroupNetworkId}");
                }

                if (!lastClient.HasValue || client.AimPreviewInstance.Index != lastClient.Value.AimPreviewInstance.Index)
                {
                    Plugin.LogInstance.LogInfo($"[AbilityBar_Client] AimPreviewInstance: {client.AimPreviewInstance.Index}");
                }

                if (!lastClient.HasValue || client.IsSimulating != lastClient.Value.IsSimulating)
                {
                    Plugin.LogInstance.LogInfo($"[AbilityBar_Client] IsSimulating: {client.IsSimulating}");
                }

                lastClient = client;
            }

            // -------- AbilityBar_Shared --------
            if (entityManager.HasComponent<AbilityBar_Shared>(player))
            {
                var shared = entityManager.GetComponentData<AbilityBar_Shared>(player);

                if (!lastShared.HasValue || shared.CastGroup._Entity.Index != lastShared.Value.CastGroup._Entity.Index)
                {
                    Plugin.LogInstance.LogInfo($"[AbilityBar_Shared] CastGroup: {shared.CastGroup._Entity.Index}");
                }

                if (!lastShared.HasValue || shared.CastAbility._Entity.Index != lastShared.Value.CastAbility._Entity.Index)
                {
                    Plugin.LogInstance.LogInfo($"[AbilityBar_Shared] CastAbility: {shared.CastAbility._Entity.Index}");
                }

                if (!lastShared.HasValue || shared.SyncedIsCasting != lastShared.Value.SyncedIsCasting)
                {
                    Plugin.LogInstance.LogInfo($"[AbilityBar_Shared] IsCasting: {shared.SyncedIsCasting}");
                }

                if (!lastShared.HasValue || shared.CastAbilityPrefabGuid != lastShared.Value.CastAbilityPrefabGuid)
                {
                    Plugin.LogInstance.LogInfo($"[AbilityBar_Shared] CastAbilityPrefabGuid: {shared.CastAbilityPrefabGuid}");
                }

                lastShared = shared;
            }
        }
    }
}
