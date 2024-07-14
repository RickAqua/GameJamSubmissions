using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Systems;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(BeforePhysicsSystemGroup))]
[UpdateAfter(typeof(PlayerControlSystem))]
public partial struct MovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        new MovementPositionJob
        {
            DeltaTime = deltaTime
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct MovementPositionJob : IJobEntity
{
    public float DeltaTime;

    [BurstCompile]
    private void Execute(MovementPositionAspect entity, [EntityIndexInQuery] int _)
    {
        entity.Move(DeltaTime);
    }
}

public readonly partial struct MovementPositionAspect : IAspect
{
    private readonly RefRW<LocalTransform> _localTransform;
    private readonly RefRW<TargetPosition> _target;
    private readonly DynamicBuffer<PositionChange> _positionChanges;

    public void Move(float deltaTime)
    {
        var positionChanges = _positionChanges;
        var currentTargetPosition = _target.ValueRO.Value;
        var newTargetPosition = currentTargetPosition;
        var overrideTargetPosition = float3.zero;
        var overrideTP = false;

        foreach (var change in positionChanges)
        {
            if (change.IsTarget)
            {
                overrideTP = true;
                overrideTargetPosition = change.Change;
            }
            else
            {
                newTargetPosition += change.Change;
            }
        }
        _target.ValueRW.Value = overrideTP ? overrideTargetPosition : newTargetPosition;

        _localTransform.ValueRW.Position = math.lerp(_localTransform.ValueRO.Position,
            _target.ValueRO.Value, deltaTime);
        positionChanges.Clear();
        
    }
}