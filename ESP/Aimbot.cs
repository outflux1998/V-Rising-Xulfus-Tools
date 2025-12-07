using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using ProjectM;
using RetroCamera.Configuration;
using RetroCamera.Utilities;
using Stunlock.Core;

namespace RetroCamera.ESP
{
    public static class Aimbot
    {
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        private static readonly Dictionary<Entity, Vector3> lastPositions = new();
        private static readonly Dictionary<Entity, Vector3> smoothedVelocities = new();

        private static readonly Dictionary<WeaponCategory, float> projectileSpeeds = new()
        {
            { WeaponCategory.Sword, 43f },
            { WeaponCategory.Axes, 43f },
            { WeaponCategory.Mace, 43f },
            { WeaponCategory.Spear, 43f },
            { WeaponCategory.Reaper, 43f },
            { WeaponCategory.Greatsword, 43f },
            { WeaponCategory.Whip, 43f },
            { WeaponCategory.Slashers, 43f },
            { WeaponCategory.Claws, 43f },
            { WeaponCategory.Twinblade, 43f },
            { WeaponCategory.Crossbow, 44f },
            { WeaponCategory.Longbow, 44f },
            { WeaponCategory.Daggers, 35f },
            { WeaponCategory.Pistols, 43f }
        };

        internal static Toggle AimbotEnabled;

        public static bool tempPrediction = false;

        public static void Init()
        {
            AimbotEnabled = OptionsManager.AddToggle("Aimbot", "Enables Aimbot", true);
        }

        private static bool IsAimbotKeyPressed(WeaponCategory category)
        {
            return Settings._spellAimbotKey.IsKeyDown() || IsWeaponAimbotKeyPressed(category);
        }

        private static bool IsWeaponAimbotKeyPressed(WeaponCategory category)
        {
            return category switch
            {
                WeaponCategory.Sword => Settings.WeaponAimbotSwordKey.IsKeyDown(),
                WeaponCategory.Axes => Settings.WeaponAimbotAxesKey.IsKeyDown(),
                WeaponCategory.Mace => Settings.WeaponAimbotMaceKey.IsKeyDown(),
                WeaponCategory.Spear => Settings.WeaponAimbotSpearKey.IsKeyDown(),
                WeaponCategory.Reaper => Settings.WeaponAimbotReaperKey.IsKeyDown(),
                WeaponCategory.Greatsword => Settings.WeaponAimbotGreatswordKey.IsKeyDown(),
                WeaponCategory.Whip => Settings.WeaponAimbotWhipKey.IsKeyDown(),
                WeaponCategory.Slashers => Settings.WeaponAimbotSlashersKey.IsKeyDown(),
                WeaponCategory.Claws => Settings.WeaponAimbotClawsKey.IsKeyDown(),
                WeaponCategory.Twinblade => Settings.WeaponAimbotTwinbladeKey.IsKeyDown(),
                WeaponCategory.Crossbow => Settings.WeaponAimbotCrossbowKey.IsKeyDown(),
                WeaponCategory.Longbow => Settings.WeaponAimbotLongbowKey.IsKeyDown(),
                WeaponCategory.Daggers => Settings.WeaponAimbotDaggersKey.IsKeyDown(),
                WeaponCategory.Pistols => Settings.WeaponAimbotPistolsKey.IsKeyDown(),
                _ => false,
            };
        }

        private static float GetProjectileSpeed(WeaponCategory category)
        {
            return projectileSpeeds.TryGetValue(category, out var speed) ? speed : 43f;
        }

        public static void Run()
        {
            if (Time.time < CounterDetector.SuppressAimbotUntilTime) return;
            if (!AimbotEnabled?.Value ?? true) return;

            var world = EntityList.GetClientWorld();
            if (world == null) return;

            var em = world.EntityManager;

            if (EntityList.Players == null || EntityList.Players.Count == 0) return;
            if (Camera.main == null) return;

            var localPlayer = EntityList.LocalPlayer;
            var weaponCategory = WeaponHelper.GetEquippedWeaponCategory(localPlayer, em);

            bool spellKeyDown = Settings._spellAimbotKey.IsKeyDown();
            bool weaponKeyDown = weaponCategory.HasValue && IsWeaponAimbotKeyPressed(weaponCategory.Value);

            if (!spellKeyDown && !weaponKeyDown) return;

            float smoothingFactor = 0.65f;
            bool predictionEnabled = true;
            float projSpeed = 45f;

            if (weaponKeyDown && weaponCategory.HasValue)
            {
                if (!IsWeaponAimbotToggleEnabled(weaponCategory.Value)) return;

                smoothingFactor = GetWeaponAimbotSmoothing(weaponCategory.Value);
                predictionEnabled = IsWeaponPredictionEnabled(weaponCategory.Value);
                projSpeed = GetProjectileSpeed(weaponCategory.Value);
            }

            float closestDistance = float.MaxValue;
            Entity? closestTarget = null;
            Vector2 mousePos = Input.mousePosition;
            Vector2? targetScreenPos = null;

            foreach (var player in EntityList.Players)
            {
                if (!em.HasComponent<LocalToWorld>(player) ||
                    !em.HasComponent<CharacterHUD>(player) ||
                    !em.HasComponent<Movement>(player)) continue;

                var hud = em.GetComponentData<CharacterHUD>(player);
                string name = hud.Name.ToString();

                if (EntityList.LocalPlayer != Entity.Null && name == EntityList.LocalPlayerName) continue;

                var ltw = em.GetComponentData<LocalToWorld>(player);
                Vector3 currentPos = ltw.Position;

                var movement = em.GetComponentData<Movement>(player);
                Vector2 moveInput = movement.MoveInput;
                float speed = movement.Speed.Value;

                Vector3 predictedOffset = Vector3.zero;
                Vector3 estimatedVelocity = Vector3.zero;

                if (tempPrediction)
                {
                    predictedOffset = new Vector3(999f, 999f, 999f);
                }
                else if (predictionEnabled)
                {
                    if (moveInput != Vector2.zero)
                    {
                        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
                        predictedOffset = inputDir * speed * (Vector3.Distance(Camera.main.transform.position, currentPos) / projSpeed);
                    }

                    if (lastPositions.TryGetValue(player, out var lastPos))
                    {
                        var rawVelocity = (currentPos - lastPos) / Time.deltaTime;
                        if (smoothedVelocities.TryGetValue(player, out var previousVelocity))
                            estimatedVelocity = Vector3.Lerp(previousVelocity, rawVelocity, 1f - smoothingFactor);
                        else
                            estimatedVelocity = rawVelocity;
                        smoothedVelocities[player] = estimatedVelocity;
                    }
                    lastPositions[player] = currentPos;
                }

                Vector3 predictedPos = currentPos + estimatedVelocity * (Vector3.Distance(Camera.main.transform.position, currentPos) / projSpeed) + predictedOffset;
                Vector3 screenPos = Camera.main.WorldToScreenPoint(predictedPos);

                if (screenPos.z < 0 || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height) continue;

                float distanceToMouse = Vector2.Distance(new Vector2(screenPos.x, screenPos.y), mousePos);
                if (distanceToMouse > 155f) continue;

                if (distanceToMouse < closestDistance)
                {
                    closestDistance = distanceToMouse;
                    closestTarget = player;
                    targetScreenPos = new Vector2(screenPos.x, Screen.height - screenPos.y);
                }
            }

            if (closestTarget.HasValue && targetScreenPos.HasValue)
            {
                SetCursorPos((int)targetScreenPos.Value.x, (int)targetScreenPos.Value.y);
            }

            if (tempPrediction) tempPrediction = false;
        }


        private static bool IsWeaponAimbotToggleEnabled(WeaponCategory category)
        {
            return category switch
            {
                WeaponCategory.Sword => Settings.AimbotSwordToggle.Value,
                WeaponCategory.Axes => Settings.AimbotAxesToggle.Value,
                WeaponCategory.Mace => Settings.AimbotMaceToggle.Value,
                WeaponCategory.Spear => Settings.AimbotSpearToggle.Value,
                WeaponCategory.Reaper => Settings.AimbotReaperToggle.Value,
                WeaponCategory.Greatsword => Settings.AimbotGreatswordToggle.Value,
                WeaponCategory.Whip => Settings.AimbotWhipToggle.Value,
                WeaponCategory.Slashers => Settings.AimbotSlashersToggle.Value,
                WeaponCategory.Claws => Settings.AimbotClawsToggle.Value,
                WeaponCategory.Twinblade => Settings.AimbotTwinbladeToggle.Value,
                WeaponCategory.Crossbow => Settings.AimbotCrossbowToggle.Value,
                WeaponCategory.Longbow => Settings.AimbotLongbowToggle.Value,
                WeaponCategory.Daggers => Settings.AimbotDaggersToggle.Value,
                WeaponCategory.Pistols => Settings.AimbotPistolsToggle.Value,
                _ => false,
            };
        }

        private static bool IsWeaponPredictionEnabled(WeaponCategory category)
        {
            return category switch
            {
                WeaponCategory.Sword => Settings.AimbotSwordPredictionToggle.Value,
                WeaponCategory.Axes => Settings.AimbotAxesPredictionToggle.Value,
                WeaponCategory.Mace => Settings.AimbotMacePredictionToggle.Value,
                WeaponCategory.Spear => Settings.AimbotSpearPredictionToggle.Value,
                WeaponCategory.Reaper => Settings.AimbotReaperPredictionToggle.Value,
                WeaponCategory.Greatsword => Settings.AimbotGreatswordPredictionToggle.Value,
                WeaponCategory.Whip => Settings.AimbotWhipPredictionToggle.Value,
                WeaponCategory.Slashers => Settings.AimbotSlashersPredictionToggle.Value,
                WeaponCategory.Claws => Settings.AimbotClawsPredictionToggle.Value,
                WeaponCategory.Twinblade => Settings.AimbotTwinbladePredictionToggle.Value,
                WeaponCategory.Crossbow => Settings.AimbotCrossbowPredictionToggle.Value,
                WeaponCategory.Longbow => Settings.AimbotLongbowPredictionToggle.Value,
                WeaponCategory.Daggers => Settings.AimbotDaggersPredictionToggle.Value,
                WeaponCategory.Pistols => Settings.AimbotPistolsPredictionToggle.Value,
                _ => false,
            };
        }

        private static float GetWeaponAimbotSmoothing(WeaponCategory category)
        {
            return category switch
            {
                WeaponCategory.Sword => Settings.AimbotSwordSmoothing.Value,
                WeaponCategory.Axes => Settings.AimbotAxesSmoothing.Value,
                WeaponCategory.Mace => Settings.AimbotMaceSmoothing.Value,
                WeaponCategory.Spear => Settings.AimbotSpearSmoothing.Value,
                WeaponCategory.Reaper => Settings.AimbotReaperSmoothing.Value,
                WeaponCategory.Greatsword => Settings.AimbotGreatswordSmoothing.Value,
                WeaponCategory.Whip => Settings.AimbotWhipSmoothing.Value,
                WeaponCategory.Slashers => Settings.AimbotSlashersSmoothing.Value,
                WeaponCategory.Claws => Settings.AimbotClawsSmoothing.Value,
                WeaponCategory.Twinblade => Settings.AimbotTwinbladeSmoothing.Value,
                WeaponCategory.Crossbow => Settings.AimbotCrossbowSmoothing.Value,
                WeaponCategory.Longbow => Settings.AimbotLongbowSmoothing.Value,
                WeaponCategory.Daggers => Settings.AimbotDaggersSmoothing.Value,
                WeaponCategory.Pistols => Settings.AimbotPistolsSmoothing.Value,
                _ => 0.65f,
            };
        }
    }
}