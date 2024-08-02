using Common;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Authoring
{
    public class CharacterAuthoring : MonoBehaviour
    {
        private class CharacterBaker : Baker<CharacterAuthoring>
        {
            public override void Bake(CharacterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var position = authoring.transform.position;
                AddComponent(entity,new Character());
                AddComponent(entity,new MoveTargetComponent()
                {
                    MoveSpeed = 400,
                    StopDistance = 0,
                    RotationSpeed = 10,
                    TargetPosition = new float3(position.x,position.y,position.z),
                });
            }
        }
    }
}