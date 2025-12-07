using Unity.Entities;

namespace RetroCamera.Patches
{
    public static class TryGetCurrentAbilityEntitiesPatch
    {
        // Método Postfix que intercepta o resultado
        public static void Postfix(
            ref bool __result,
            ref EntityManager entityManager,
            ref Entity groupEntity,
            ref Entity ability)
        {
            if (__result)
            {
                Plugin.LogInstance.LogInfo($"[Patch] TryGetCurrentAbilityEntities: SUCCESS! Group={groupEntity.Index}, Ability={ability.Index}");
            }
            else
            {
                Plugin.LogInstance.LogInfo($"[Patch] TryGetCurrentAbilityEntities: FAILED for Group={groupEntity.Index}");
            }
        }
    }
}
