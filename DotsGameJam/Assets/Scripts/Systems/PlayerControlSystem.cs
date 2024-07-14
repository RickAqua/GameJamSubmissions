using System.Diagnostics.CodeAnalysis;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable CS0414 // Field is assigned but its value is never used

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(BeforePhysicsSystemGroup))]
    public partial struct PlayerControlSystem : ISystem
    {
        private float rotation;

        private const float Speed = 5f;
        private const float RotationSpeed = 2f;
        private const float ProximityRadius = 2f;

        private EntityQuery triggerableQuery;
        private EntityQuery playerQuery;
        private EntityQuery exitQuery;

        public void OnCreate(ref SystemState state)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            playerQuery = entityManager.CreateEntityQuery(typeof(PlayerTag), typeof(LocalTransform));
            exitQuery = entityManager.CreateEntityQuery(typeof(ExitTag), typeof(LocalTransform));
            triggerableQuery = entityManager.CreateEntityQuery(typeof(TriggerableTag), typeof(LocalTransform));
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.WorldUnmanaged.EntityManager;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            if (playerQuery.TryGetSingleton(out LocalTransform playerTransform))
            {
                if (NextLevel(ref state, entityManager, playerTransform))
                {
                    Action(ref state, entityManager, ecb, playerTransform);
                    Movement(ref state, entityManager, ecb);
                }
            }
        }

        private bool NextLevel(ref SystemState state, EntityManager entityManager, LocalTransform playerTransform)
        {
            var exitEntity = exitQuery.GetSingletonEntity();
            if (playerTransform.Position.y < -20)
            {
                var currentLevel = entityManager.GetComponentData<CurrentLevelTag>(exitEntity);
                SceneManager.LoadScene(currentLevel.Value);
                return false;
            }
            
            var exitTransform = entityManager.GetComponentData<LocalTransform>(exitEntity);
            var distance = math.distance(playerTransform.Position, exitTransform.Position);
            if (distance <= ProximityRadius)
            {
                var exitTag = entityManager.GetComponentData<ExitTag>(exitQuery.GetSingletonEntity());

                SceneManager.LoadScene(exitTag.Value);
                return false;
            }

            return true;
        }

        private void Action(ref SystemState state, EntityManager entityManager, EntityCommandBuffer ecb,
            LocalTransform playerTransform)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKey(KeyCode.E))
            {
                var targetPosition = playerTransform.Position;

                NativeArray<Entity> triggerableEntities = triggerableQuery.ToEntityArray(Allocator.TempJob);
                foreach (var entity in triggerableEntities)
                {
                    // Get Triggerable Transforms
                    var transform = entityManager.GetComponentData<LocalTransform>(entity);
                    var distance = math.distance(transform.Position, targetPosition);
                    if (distance <= ProximityRadius) Activate(entityManager, ecb, entity);
                }

                triggerableEntities.Dispose();

                ecb.Playback(entityManager);
            }
        }

        private void Activate(EntityManager entityManager, EntityCommandBuffer ecb, Entity entity)
        {
            ecb.AddComponent<Activated>(entity);
            if (entityManager.HasBuffer<TriggerListComponent>(entity))
            {
                var buffer = entityManager.GetBuffer<TriggerListComponent>(entity, true);
                if (buffer.Length > 0)
                {
                    foreach (var triggerListComponent in buffer)
                    {
                        Activate(entityManager, ecb, triggerListComponent.Value);
                    }
                }
            }
        }

        [BurstCompile]
        private void Movement(ref SystemState state, EntityManager entityManager, EntityCommandBuffer ecb)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            var rotateLeft = Input.GetKey(KeyCode.A);
            var rotateRight = Input.GetKey(KeyCode.D);
            var forward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            var backwards = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            var left = Input.GetKey(KeyCode.LeftArrow);
            var right = Input.GetKey(KeyCode.RightArrow);

            rotation += (rotateLeft ? (rotateRight ? 0 : -RotationSpeed) : (rotateRight ? RotationSpeed : 0)) *
                        deltaTime;
            var z = forward ? (backwards ? 0 : Speed) : (backwards ? -Speed : 0);
            var x = right ? (left ? 0 : Speed) : (left ? -Speed : 0);

            new PlayerMovementJob
            {
                DeltaTime = deltaTime,
                Rotation = rotation,
                Z = z,
                X = x
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct PlayerMovementJob : IJobEntity
    {
        public float DeltaTime;
        public float Rotation;
        public float Z;
        public float X;

        [BurstCompile]
        private void Execute(PlayerMovementAspect entity, [EntityIndexInQuery] int _)
        {
            entity.Move(Rotation, Z, X, DeltaTime);
        }
    } 

    public readonly partial struct PlayerMovementAspect : IAspect
    {
        private readonly RefRO<PlayerTag> _tag;
        private readonly RefRW<LocalTransform> _localTransform;

        [SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable")]
        public void Move(float rotation, float z, float x, float deltaTime)
        {
            var localTransform = _localTransform;
            localTransform.ValueRW.Rotation = quaternion.RotateY(rotation);
            localTransform.ValueRW.Position =
                localTransform.ValueRO.Position + localTransform.ValueRO.Forward() * z * deltaTime +
                localTransform.ValueRO.Right() * x * deltaTime;
        }
    }
}