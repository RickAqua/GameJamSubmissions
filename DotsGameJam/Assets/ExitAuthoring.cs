using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ExitAuthoring : MonoBehaviour
{
    public int nextLevel;
    public int currentLevel;
    class ExitBaker : Baker<ExitAuthoring>
    {
        public override void Bake(ExitAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ExitTag{Value = authoring.nextLevel});
            AddComponent(entity, new CurrentLevelTag{Value = authoring.currentLevel});
        }
    }
}

public struct CurrentLevelTag : IComponentData
{
    public int Value;
}

public struct ExitTag : IComponentData
{
    public int Value;
}
