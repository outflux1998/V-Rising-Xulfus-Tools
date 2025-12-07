using Unity.Entities;
using UnityEngine;

public static class ECSUtilities
{
    public static void DumpAllComponents(EntityManager entityManager, Entity localPlayer)
    {
        if (!entityManager.Exists(localPlayer))
        {
            Debug.LogError("[ComponentDump] Local player entity does not exist.");
            return;
        }

        var componentTypes = entityManager.GetComponentTypes(localPlayer);

        Debug.Log($"[ComponentDump] Local player has {componentTypes.Length} components:");

        for (int i = 0; i < componentTypes.Length; i++)
        {
            var componentType = componentTypes[i];
            int typeIndex = componentType.TypeIndex;  // Unique index assigned by Unity's TypeManager
            string typeName = componentType.GetManagedType()?.FullName ?? componentType.ToString();

            Debug.Log($"[{i}] TypeIndex: {typeIndex} | Component: {typeName}");
        }

        componentTypes.Dispose();
    }
}
