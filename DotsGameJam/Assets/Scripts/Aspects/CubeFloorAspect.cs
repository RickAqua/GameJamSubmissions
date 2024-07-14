using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct CubeFloorAspect : IAspect
{
    public readonly Entity Entity;

    private readonly RefRO<LocalTransform> _localTransform;
    private readonly RefRO<CubeFloorProperties> _properties;
    public float3 Position => _localTransform.ValueRO.Position;
    public quaternion Rotation => _localTransform.ValueRO.Rotation;
    public float Scale => _localTransform.ValueRO.Scale;
    
    public float SizeX => _properties.ValueRO.SizeX;
    public float SizeZ => _properties.ValueRO.SizeZ;
    public float SpacingX => _properties.ValueRO.SpacingX;
    public float SpacingZ => _properties.ValueRO.SpacingZ;
    public Entity Prefab => _properties.ValueRO.Prefab;

}