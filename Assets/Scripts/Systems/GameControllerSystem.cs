using Common;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [BurstCompile]
    public partial struct GameControllerSystem : ISystem
    {
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var c in SystemAPI.Query<RefRO<Controller>>())
            {
                //修改角色的目标位置
                foreach (var target in SystemAPI.Query<RefRW<MoveTargetComponent>>())
                {
                    target.ValueRW.TargetPosition = c.ValueRO.targetPoint;
                }
            }
        }
    }
}