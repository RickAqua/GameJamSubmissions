using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Systems;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(BeforePhysicsSystemGroup))]
[UpdateAfter(typeof(PlayerControlSystem))]
public partial struct MoveToTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        new MoveToTargetJob
        {
            DeltaTime = deltaTime
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct MoveToTargetJob : IJobEntity
{
    public float DeltaTime;

    [BurstCompile]
    private void Execute(MoveToTargetAspect entity, [EntityIndexInQuery] int _)
    {
        entity.Move(DeltaTime);
    }
}

public readonly partial struct MoveToTargetAspect : IAspect
{
    private readonly RefRW<LocalTransform> _localTransform;
    private readonly RefRW<TargetPositionRotation> _target;

    public bool Move(float deltaTime)
    {
        var time = _target.ValueRO.Time;
        var currentScale = _localTransform.ValueRO.Scale;
        var currentPosition = _localTransform.ValueRO.Position;
        var currentRotation = math.Euler(_localTransform.ValueRO.Rotation);
        var targetScale = _target.ValueRO.Scale;
        var targetPosition = _target.ValueRO.Position;
        var targetRotation = _target.ValueRO.Rotation;

        if (time > 0)
        {
            _localTransform.ValueRW.Scale = math.lerp(currentScale, targetScale, deltaTime / time);
            _localTransform.ValueRW.Position = math.lerp(currentPosition, targetPosition, deltaTime / time);
            _localTransform.ValueRW.Rotation =
                quaternion.Euler(math.lerp(currentRotation, targetRotation, deltaTime / time));

            _target.ValueRW.Time -= deltaTime;
            return false;
        }

        _localTransform.ValueRW.Scale =  targetScale;
        _localTransform.ValueRW.Position = targetPosition;
        _localTransform.ValueRW.Rotation = quaternion.Euler(targetRotation);
        return true;
    }
}