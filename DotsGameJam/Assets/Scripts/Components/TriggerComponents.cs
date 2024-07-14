using Unity.Entities;

namespace Components
{
    public struct TriggerListComponent : IBufferElementData
    {
        public Entity Value;
    }

    public struct Activated : IComponentData
    {
    }
}