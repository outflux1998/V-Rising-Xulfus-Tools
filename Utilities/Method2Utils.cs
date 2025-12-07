//using Unity.Entities;
//using Unity.Transforms;
//using Unity.Mathematics;
//using RetroCamera.ESP;

//namespace RetroCamera.Utilities
//{
//    internal static class Method2Utils
//    {
//        public static void RotatePlayer180()
//        {
//            var world = EntityList.GetClientWorld();
//            if (world == null) return;

//            var em = world.EntityManager;
//            var localPlayer = EntityList.LocalPlayer;

//            if (!em.Exists(localPlayer)) return;

//            if (!em.HasComponent<Rotation>(localPlayer)) return;

//            var currentRotation = em.GetComponentData<Rotation>(localPlayer).Value;

//            // Convert quaternion to euler
//            float3 euler = math.degrees(math.eulerAngles(currentRotation));

//            // Add 180° on Y axis
//            euler.y = (euler.y + 180f) % 360f;

//            // Convert back to quaternion
//            quaternion newRotation = quaternion.Euler(math.radians(euler));

//            em.SetComponentData(localPlayer, new Rotation { Value = newRotation });

//            Core.Log.LogInfo($"[Method2Utils] Rotated local player 180° to Y angle: {euler.y}°");
//        }
//    }
//}
