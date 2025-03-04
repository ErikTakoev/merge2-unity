using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct Test : IComponentData
{
    public float Value;
}

public partial struct TestSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (transform, test) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Test>>())
        {
            Debug.Log($"Transform: {transform.ValueRW.Position}, Test: {test.ValueRW.Value}");
        }
    }
}
