using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CubeFloorPrefab : MonoBehaviour
{
    public class CubeFloorPrefabBaker : Baker<CubeFloorPrefab>
    {
        public override void Bake(CubeFloorPrefab authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
        }
    }
}
