using UnityEngine;
using Unity.Entities;

public class TestAuthoring : MonoBehaviour
{
    public float Value;

    class Baker : Baker<TestAuthoring>
    {
        public override void Bake(TestAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Test { Value = authoring.Value });

        }
    }
}