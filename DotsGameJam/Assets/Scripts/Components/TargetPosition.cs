using Unity.Entities;
using Unity.Mathematics;

// These are the targets for the LocalTransform in 1 second
public struct TargetPosition : IComponentData
{
    public float3 Value;
}

public struct TargetRotation : IComponentData
{
    public float3 Value;
}

public struct TargetPositionRotation : IComponentData
{
    public float3 Position;
    public float3 Rotation;
    public float Scale;
    public float Time;
}

public struct TargetSize : IComponentData
{
    public float Value;
}

public struct PositionChange : IBufferElementData
{
    public bool IsTarget;
    public float3 Change;
}

public struct RotationChange : IBufferElementData
{
    public bool IsTarget;
    public quaternion Change;
}

public struct SizeChange : IBufferElementData
{
    public bool IsTarget;
    public float Change;
}