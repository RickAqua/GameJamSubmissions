using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MovingPlatformAuthoring : MonoBehaviour
{
    public float scale;
    public float time;
    public GameObject target;

    class MovingPlatformBaker : Baker<MovingPlatformAuthoring>
    {
        public override void Bake(MovingPlatformAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var targetInfo = authoring.target.transform;
            AddComponent(entity, new MovingPlatform
            {
                Rotation = targetInfo.rotation.eulerAngles,
                Position = targetInfo.position,
                Scale = authoring.scale,
                Time = authoring.time
            });
        }
    }
}

public struct MovingPlatform : IComponentData
{
    public float3 Position;
    public float3 Rotation;
    public float Scale;
    public float Time;
}

[BurstCompile]
public partial struct MovingPlatformSystem : ISystem
{
    private EntityQuery activatedPlatforms;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MovingPlatform>();
        state.RequireForUpdate<Activated>();
        var entityManager = state.EntityManager;
        activatedPlatforms = entityManager.CreateEntityQuery(typeof(MovingPlatform), typeof(Activated));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var entityManager = state.EntityManager;

        NativeArray<Entity> entityArray = activatedPlatforms.ToEntityArray(Allocator.TempJob);
        foreach (var entity in entityArray)
        {
            // Get Triggerable Transforms
            var movingPlatformInfo = entityManager.GetComponentData<MovingPlatform>(entity);
            ecb.AddComponent(entity, new TargetPositionRotation
            {
                Rotation = movingPlatformInfo.Rotation,
                Position = movingPlatformInfo.Position,
                Scale = movingPlatformInfo.Scale,
                Time = movingPlatformInfo.Time
            });
            ecb.RemoveComponent<MovingPlatform>(entity);
        }
        entityArray.Dispose();

        ecb.Playback(entityManager);
    }
}
