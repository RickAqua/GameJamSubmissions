using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine;

public class PoleSwitchAuthoring : MonoBehaviour
{
    public GameObject[] triggeredGameObjects;

    class PoleSwitchBaker : Baker<PoleSwitchAuthoring>
    {
        public override void Bake(PoleSwitchAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<TriggerableTag>(entity);
            var buffer = AddBuffer<TriggerListComponent>(entity);
            foreach (var triggeredGameObject in authoring.triggeredGameObjects)
            {
                // Get the Entity associated with the GameObject
                buffer.Add(new TriggerListComponent {Value = GetEntity(triggeredGameObject, TransformUsageFlags.Dynamic)});
            }
        }
    }
}

public struct TriggerableTag : IComponentData
{
}
