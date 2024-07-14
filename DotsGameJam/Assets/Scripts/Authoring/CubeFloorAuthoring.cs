using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CubeFloorAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public int sizeX;
    public int sizeZ;
    public float spacingX;
    public float spacingZ;
}

public class CubeFloorBaker : Baker<CubeFloorAuthoring>
{
    public override void Bake(CubeFloorAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new CubeFloorProperties
        {
            Prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
            SizeX = authoring.sizeX,
            SizeZ = authoring.sizeZ,
            SpacingX = authoring.spacingZ,
            SpacingZ = authoring.spacingZ
        });
    }
}

public struct CubeFloorProperties : IComponentData
{
    public Entity Prefab;
    public float SizeX;
    public float SizeZ;
    public float SpacingX;
    public float SpacingZ;
}