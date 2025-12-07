
using UnityEngine;
using Unity.Entities;
using ProjectM;
using Unity.Transforms;
using System.Collections.Generic;
using Unity.Mathematics;
using Stunlock.Core;
using RetroCamera.Utilities;

namespace RetroCamera.ESP
{

    internal class Render
    {

        public static void DrawPlayers()
        {
            if (!Auth.IsAuthorized) return;

            var world = EntityList.GetClientWorld();
            if (world == null) return;

            var entityManager = world.EntityManager;

            if (EntityList.Players == null || EntityList.Players.Count == 0)
                return;

            if (Camera.main == null) return;

            int drawn = 0;
            int maxDrawn = 20;

            foreach (var player in EntityList.Players)
            {
                if (drawn >= maxDrawn) break;
                if (!entityManager.Exists(player)) continue;

                if (!entityManager.HasComponent<CharacterHUD>(player) || !entityManager.HasComponent<LocalToWorld>(player))
                    continue;

                var hud = entityManager.GetComponentData<CharacterHUD>(player);
                string name = hud.Name.IsEmpty ? "Player" : hud.Name.ToString();

                if (EntityList.LocalPlayer != Entity.Null)
                {
                    if (string.Equals(name, EntityList.LocalPlayerName))
                        continue;
                }

                if (WhitelistManager.WhitelistNames.Contains(name))
                    continue;

                var translation = entityManager.GetComponentData<LocalToWorld>(player);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(translation.Position);

                if (screenPos.z < 0 ||
                    screenPos.x < 0 || screenPos.x > Screen.width ||
                    screenPos.y < 0 || screenPos.y > Screen.height)
                    continue;

                int level = 0;

                if (entityManager.HasComponent<Equipment>(player))
                {
                    var equipment = entityManager.GetComponentData<Equipment>(player);
                    level = (int)(equipment.ArmorLevel.Value + equipment.WeaponLevel.Value + equipment.SpellLevel.Value);
                }

                float currentHp = 0f;
                float maxHp = 0f;

                if (entityManager.HasComponent<Health>(player))
                {
                    var health = entityManager.GetComponentData<Health>(player);
                    currentHp = health.Value;
                    maxHp = health.MaxHealth;
                }

                Color orange = new Color(1f, 0.5f, 0f);

                var lines = new List<(string, Color)>
                {
                    ($"HP: {currentHp}", Color.green),
                    (name, Color.cyan),
                    ($"Level: {level}", orange)
                };

                Primitives.DrawStringMultiline(new Vector2(screenPos.x, screenPos.y - 40), lines);

                drawn++;
            }
        }





        public static void DrawBloodCarriers()
        {
            if (!Auth.IsAuthorized) return;

            Primitives.Init();

            var world = EntityList.GetClientWorld();
            if (world == null) return;

            var entityManager = world.EntityManager;

            if (EntityList.BloodCarriers == null || EntityList.BloodCarriers.Count == 0)
                return;

            if (Camera.main == null)
                return;

            foreach (var entity in EntityList.BloodCarriers)
            {
                if (!entityManager.Exists(entity)) continue;

                var transform = entityManager.GetComponentData<LocalToWorld>(entity);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.Position);

                if (screenPos.z < 0 ||
                    screenPos.x < 0 || screenPos.x > Screen.width ||
                    screenPos.y < 0 || screenPos.y > Screen.height)
                    continue;

                var bloodSource = entityManager.GetComponentData<BloodConsumeSource>(entity);

                int bloodTypeHash = bloodSource.UnitBloodType._Value.GuidHash;

                string bloodType = BloodType.GetBloodTypeName(bloodTypeHash);

                string label = $"{bloodType} {bloodSource.BloodQuality:0.0}%";

                Primitives.DrawString(new Vector2(screenPos.x, screenPos.y), label, Color.red);
            }
        }



        public static void DrawContainers()
        {
            if (!Auth.IsAuthorized) return;

            Primitives.Init();

            var world = EntityList.GetClientWorld();
            if (world == null) return;

            var entityManager = world.EntityManager;

            if (EntityList.Containers == null || EntityList.Containers.Count == 0)
                return;

            if (Camera.main == null)
                return;

            foreach (var container in EntityList.Containers)
            {

                if (!entityManager.Exists(container)) continue;

                var translation = entityManager.GetComponentData<LocalToWorld>(container);
                Vector3 worldPos = translation.Position;
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

                if (screenPos.z < 0 || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height)
                    continue;

                var prefabGuid = entityManager.GetComponentData<PrefabGUID>(container);
                string label = PrefabNameTranslator.GetName(prefabGuid.GuidHash);

                Vector2 boxSize = new Vector2(90f, 70f);
                Vector2 screen2D = new Vector2(screenPos.x, screenPos.y);

                // Eleva a caixa na tela (como o DrawBox inverte o Y, aqui é +)
                screen2D.y += 100f;

                // Centraliza a caixa
                Vector2 boxPosition = screen2D - (boxSize / 2f);

                // Renderiza
                Primitives.DrawBox(boxPosition, boxSize, Color.yellow, invert: true);
                Primitives.DrawString(screen2D, label, Color.yellow, true);

            }
        }





    }
}
