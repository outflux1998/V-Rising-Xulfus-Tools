//using UnityEngine;
//using Unity.Entities;
//using ProjectM;
//using Unity.Transforms;

//namespace RetroCamera.ESP
//{
//    public static class CounterCheckHelper
//    {
//        private static readonly float MaxFovPixels = 155f;

//        public static void CheckCountersNearMouse()
//        {
//            var world = EntityList.GetClientWorld();
//            if (world == null) return;

//            var em = world.EntityManager;

//            if (EntityList.Players == null || EntityList.Players.Count == 0) return;
//            if (Camera.main == null) return;

//            Vector2 mousePos = Input.mousePosition;

//            foreach (var player in EntityList.Players)
//            {
//                if (!em.Exists(player)) continue;
//                if (!em.HasComponent<LocalToWorld>(player) ||
//                    !em.HasComponent<CharacterHUD>(player)) continue;

//                var ltw = em.GetComponentData<LocalToWorld>(player);
//                Vector3 worldPos = ltw.Position;

//                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

//                // Verifica se está visível na tela
//                if (screenPos.z <= 0 ||
//                    screenPos.x < 0 || screenPos.x > Screen.width ||
//                    screenPos.y < 0 || screenPos.y > Screen.height) continue;

//                float distanceToMouse = Vector2.Distance(new Vector2(screenPos.x, screenPos.y), mousePos);

//                if (distanceToMouse <= MaxFovPixels)
//                {
//                    // ✅ Só chama se estiver perto do mouse
//                    CounterDetector.CheckForCounter(player);
//                }
//            }
//        }
//    }
//}
