using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct CubeFloorInitializeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CubeFloorProperties>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        var cubeFloorProperties = SystemAPI.GetSingletonEntity<CubeFloorProperties>();
        var cubeFloor = SystemAPI.GetAspect<CubeFloorAspect>(cubeFloorProperties);

        var spacingX = cubeFloor.SpacingX;
        var spacingZ = cubeFloor.SpacingZ;
        var sizeX = cubeFloor.SizeX;
        var sizeZ = cubeFloor.SizeZ;

        var cubeFirstX = cubeFloor.Position.x - sizeX / 2 * spacingX;
        var cubeFirstZ = cubeFloor.Position.z - sizeZ / 2 * spacingZ;
        var cubeY = cubeFloor.Position.y;

        var random = new Unity.Mathematics.Random(112);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        for (var x = 0; x < cubeFloor.SizeX; x++)
        for (var z = 0; z < cubeFloor.SizeZ; z++)
        {
            var newCube = ecb.Instantiate(cubeFloor.Prefab);
            var newCubeTransform = new LocalTransform
            {
                Position = new float3(cubeFirstX + x * spacingX, cubeY, cubeFirstZ + z * spacingZ),
                Rotation = cubeFloor.Rotation,
                Scale = cubeFloor.Scale
            };
            ecb.SetComponent(newCube, newCubeTransform);
  //          ecb.AddComponent(newCube, new Movement { Value = math.up() * random.NextFloat(-1f, 1f) });
            ecb.AddBuffer<PositionChange>(newCube);
            ecb.AddBuffer<RotationChange>(newCube);
            ecb.AddBuffer<SizeChange>(newCube);
            ecb.AddComponent(newCube, new TargetPosition{Value = newCubeTransform.Position});
        }

        ecb.Playback(state.EntityManager);
    }
}