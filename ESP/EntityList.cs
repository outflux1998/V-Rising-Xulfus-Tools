using Epic.OnlineServices.Presence;
using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.Network;
using Stunlock.Core;
using Stunlock.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Transforms;
using UnityEngine.EventSystems;

namespace RetroCamera.ESP
{
    internal class EntityList
    {
        static public List<Entity> Players;
        static public List<Entity> BloodCarriers;
        static public List<Entity> Containers;
        static public List<Entity> Mountables;

        static public Entity LocalPlayer;
        static public String LocalPlayerName;

        public static World GetClientWorld()
        {
            foreach (var world in World.All)
            {
                if (world.Name.ToLower().Contains("client"))
                {
                    return world;
                }
            }
            return null;
        }

        static public NativeArray<Entity> GetEntities(World playerWorld)
        {
            var entityManager = playerWorld.EntityManager;
            var query = entityManager.CreateEntityQuery(new EntityQueryDesc
            {
                None = Array.Empty<ComponentType>(),
                Any = Array.Empty<ComponentType>(),
                All = Array.Empty<ComponentType>()
            });

            var result = query.ToEntityArray(Allocator.TempJob);


            return result;
        }

        public static List<Entity> GetPlayers(World playerWorld)
        {
            var players = new List<Entity>();
            var entityManager = playerWorld.EntityManager;

            // Pega o LocalPlayer e seu time
            Team localPlayerTeam = default;
            bool hasLocalTeam = false;

            if (LocalPlayer != Entity.Null && entityManager.HasComponent<Team>(LocalPlayer))
            {
                localPlayerTeam = entityManager.GetComponentData<Team>(LocalPlayer);
                hasLocalTeam = true;
            }

            // Query para pegar todos os players
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<CharacterHUD>(),
                ComponentType.ReadOnly<Health>(),
                ComponentType.ReadOnly<Equipment>(),
                ComponentType.ReadOnly<Blood>(),
                ComponentType.ReadOnly<LocalToWorld>()
            );

            var result = query.ToEntityArray(Unity.Collections.Allocator.TempJob);

            foreach (var entity in result)
            {
                if (!entityManager.Exists(entity)) continue;

                // Verifica se o player tem componente de Time
                if (entityManager.HasComponent<Team>(entity) && hasLocalTeam)
                {
                    var entityTeam = entityManager.GetComponentData<Team>(entity);

                    if (entityTeam.Value == localPlayerTeam.Value)
                    {
                        // Mesmo time, ignora
                        continue;
                    }
                }

                // Chamada para verificar se o player está em counter
                //CounterDetector.CheckForCounter(entity);

                players.Add(entity);
            }

            result.Dispose();
            query.Dispose();

            return players;
        }



        public static List<Entity> GetBloodCarriers(World playerWorld, float minQuality = 95f)
        {
            var bloodCarriers = new List<Entity>();
            var entityManager = playerWorld.EntityManager;


            var query = entityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
            ComponentType.ReadOnly<EntityCategory>(),
            ComponentType.ReadOnly<BloodConsumeSource>(),
            ComponentType.ReadOnly<LocalToWorld>()
        }
            });

            var result = query.ToEntityArray(Allocator.TempJob);

            foreach (var entity in result)
            {
                if (!entityManager.Exists(entity)) continue;

                var category = entityManager.GetComponentData<EntityCategory>(entity);
                if (category.MainCategory != MainEntityCategory.Unit) continue;
                if (category.UnitCategory == UnitCategory.PlayerVampire) continue;

                var bloodSource = entityManager.GetComponentData<BloodConsumeSource>(entity);
                if (bloodSource.BloodQuality < minQuality) continue;

                bloodCarriers.Add(entity);
            }

            result.Dispose();
            query.Dispose();

            return bloodCarriers;
        }



        public static List<Entity> GetContainers(World playerWorld)
        {
            var containers = new List<Entity>();
            var entityManager = playerWorld.EntityManager;

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<InventoryOwner>(),
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadOnly<PrefabGUID>()
            );

            var entities = query.ToEntityArray(Allocator.TempJob);
            var guids = query.ToComponentDataArray<PrefabGUID>(Allocator.TempJob);

            try
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    int guid = guids[i].GuidHash;

                    if (!PrefabNameTranslator.GuidToName.ContainsKey(guid))
                        continue;

                    containers.Add(entities[i]);
                }
            }
            finally
            {
                entities.Dispose();
                guids.Dispose();
            }

            return containers;
        }



        public static void UpdatePlayers()
        {
            var world = GetClientWorld();
            if (world == null) return;

            Players = GetPlayers(world);

        }

        public static void UpdateContainers()
        {
            var world = GetClientWorld();
            if (world == null) return;

            Containers = GetContainers(world);

        }

        public static void UpdateBloodCarriers()
        {
            var world = GetClientWorld();
            if (world == null) return;

            BloodCarriers = GetBloodCarriers(world);

        }

        static public void GetAll()
        {
            var world = GetClientWorld();
            if (world == null) return;

            Players = GetPlayers(world);
            BloodCarriers = GetBloodCarriers(world);
            Containers = GetContainers(world);
        }

        public static Entity GetLocalPlayer()
        {
            var clientWorld = WorldUtility.FindClientWorld();
            if (clientWorld == null)
            {
                Core.Log.LogWarning("Client World not available.");
                return Entity.Null;
            }

            var entityManager = clientWorld.EntityManager;

            var localCharQuery = entityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { ComponentType.ReadOnly<ProjectM.Network.LocalCharacter>() },
                Options = EntityQueryOptions.IncludeDisabledEntities
            });

            NativeArray<Entity> locals = localCharQuery.ToEntityArray(Allocator.Temp);

            Entity localPlayer = Entity.Null;

            foreach (var candidate in locals)
            {
                if (entityManager.Exists(candidate))
                {
                    localPlayer = candidate;
                    break; // First valid match
                }
            }



            locals.Dispose();

            LocalPlayer = localPlayer;
            LocalPlayerName = entityManager.GetComponentData<CharacterHUD>(localPlayer).Name.ToString();
            return localPlayer;
        }

        public static void LogCastInfo(Entity entity)
        {
            var entityManager = GetClientWorld().EntityManager;
            Core.Log.LogInfo($"[CAST] ===== Logando informações de CAST para Entity {entity.Index} =====");

            if (entityManager.HasComponent<EntityAbilityInput>(entity))
            {
                var input = entityManager.GetComponentData<EntityAbilityInput>(entity);
                Core.Log.LogInfo($"[CAST] QueueAbilityUntil: {input.QueueAbilityUntil}");
                Core.Log.LogInfo($"[CAST] PrepareCastGroup: {input.PrepareCastGroup.Index}");
                Core.Log.LogInfo($"[CAST] ActiveCastGroup: {input.ActiveCastGroup.Index}");
                Core.Log.LogInfo($"[CAST] QueuedCastGroup: {input.QueuedCastGroup.Index}");
                Core.Log.LogInfo($"[CAST] CastInput: {input.CastInput}");
                Core.Log.LogInfo($"[CAST] HasMoveInput: {input.HasMoveInput}");
                Core.Log.LogInfo($"[CAST] Interrupt: {input.Interrupt}");
                Core.Log.LogInfo($"[CAST] InterruptByPrepare: {input.InterruptByPrepare}");
            }
            else
            {
                Core.Log.LogInfo($"[CAST] ProjectM_EntityAbilityInput: Não encontrado.");
            }

            if (entityManager.HasComponent<CastSequenceBuffer>(entity))
            {
                var sequence = entityManager.GetComponentData<CastSequenceBuffer>(entity);
                Core.Log.LogInfo($"[CAST] CastSequence ID: {sequence.CastSequence.Id.Index}");
            }
            else
            {
                Core.Log.LogInfo($"[CAST] ProjectM_CastSequenceBuffer: Não encontrado.");
            }

            if (entityManager.HasComponent<GlobalCooldown>(entity))
            {
                var gcd = entityManager.GetComponentData<GlobalCooldown>(entity);
                Core.Log.LogInfo($"[CAST] GlobalCooldown: {gcd.Value}");
            }
            else
            {
                Core.Log.LogInfo($"[CAST] ProjectM_GlobalCooldown: Não encontrado.");
            }

            if (entityManager.HasComponent<AbilityBar_Client>(entity))
            {
                var client = entityManager.GetComponentData<AbilityBar_Client>(entity);
                Core.Log.LogInfo($"[CAST] ClientCastGroupNetworkId: {client.ClientCastGroupNetworkId}");
                Core.Log.LogInfo($"[CAST] ClientCastStartedCounter: {client.ClientCastStartedCounter}");
                Core.Log.LogInfo($"[CAST] ClientInterruptCounter: {client.ClientInterruptCounter}");
                Core.Log.LogInfo($"[CAST] HandledServerInterruptCounter: {client.HandledServerInterruptCounter}");
                Core.Log.LogInfo($"[CAST] IsSimulating: {client.IsSimulating}");
            }
            else
            {
                Core.Log.LogInfo($"[CAST] ProjectM_AbilityBar_Client: Não encontrado.");
            }

            Entity castAbilityEntity = Entity.Null;

            if (entityManager.HasComponent<AbilityBar_Shared>(entity))
            {
                var shared = entityManager.GetComponentData<AbilityBar_Shared>(entity);
                Core.Log.LogInfo($"[CAST] GlobalCooldown: {shared.GlobalCooldown}");
                Core.Log.LogInfo($"[CAST] CastStartTime: {shared.CastStartTime}");
                Core.Log.LogInfo($"[CAST] CastTime: {shared.CastTime}");
                Core.Log.LogInfo($"[CAST] PostCastTime: {shared.PostCastTime}");
                Core.Log.LogInfo($"[CAST] InterruptTypes: {shared.InterruptTypes}");
                Core.Log.LogInfo($"[CAST] CooldownOnInterrupt: {shared.CooldownOnInterrupt}");
                Core.Log.LogInfo($"[CAST] SyncedIsCasting: {shared.SyncedIsCasting}");
                Core.Log.LogInfo($"[CAST] IsChargeUp: {shared.IsChargeUp}");
                Core.Log.LogInfo($"[CAST] CastGroup: {shared.CastGroup._Entity.Index}");
                Core.Log.LogInfo($"[CAST] CastAbility: {shared.CastAbility._Entity.Index}");

                castAbilityEntity = shared.CastAbility._Entity;
            }
            else
            {
                Core.Log.LogInfo($"[CAST] ProjectM_AbilityBar_Shared: Não encontrado.");
            }

            if (entityManager.HasComponent<EntityAimData>(entity))
            {
                var aim = entityManager.GetComponentData<EntityAimData>(entity);
                Core.Log.LogInfo($"[CAST] AimPosition: {aim.AimPosition}");
                Core.Log.LogInfo($"[CAST] AimPositionPlane: {aim.AimPositionPlane}");
                Core.Log.LogInfo($"[CAST] ProjectileAimPosition: {aim.ProjectileAimPosition}");
            }
            else
            {
                Core.Log.LogInfo($"[CAST] ProjectM_EntityAimData: Não encontrado.");
            }

            // ✅ Verificação final: CastAbility tem PrefabGUID?
            if (castAbilityEntity != Entity.Null && entityManager.HasComponent<PrefabGUID>(castAbilityEntity))
            {
                var prefabGuid = entityManager.GetComponentData<PrefabGUID>(castAbilityEntity);
                Core.Log.LogInfo($"[CAST] CastAbility PrefabGUID: {prefabGuid}");
            }
            else if (castAbilityEntity != Entity.Null)
            {
                Core.Log.LogInfo($"[CAST] CastAbility não possui PrefabGUID.");
            }

            Core.Log.LogInfo($"[CAST] ===== Fim do log de CAST para Entity {entity.Index} =====");
        }

        public static void LogCastInput(Entity entity)
        {
            var entityManager = GetClientWorld().EntityManager;

            Core.Log.LogInfo($"[CAST_INPUT] ===== Logando CastInput para Entity {entity.Index} =====");

            if (entityManager.HasComponent<EntityAbilityInput>(entity))
            {
                var input = entityManager.GetComponentData<EntityAbilityInput>(entity);
                Core.Log.LogInfo($"[CAST_INPUT] CastInput: {(int)input.CastInput}");
            }
            else
            {
                //Core.Log.LogInfo($"[CAST_INPUT] EntityAbilityInput não encontrado para Entity {entity.Index}.");
            }

            //Core.Log.LogInfo($"[CAST_INPUT] ===== Fim do log para Entity {entity.Index} =====");
        }
    }
}
