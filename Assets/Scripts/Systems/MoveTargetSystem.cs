using Common;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    public partial struct MoveTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (transform, moveTarget) in SystemAPI.Query<RefRW<LocalTransform>,RefRO<MoveTargetComponent>>())
            {
                var targetPosition = moveTarget.ValueRO.TargetPosition;
                var moveSpeed = moveTarget.ValueRO.MoveSpeed;
                var stopDistance = moveTarget.ValueRO.StopDistance;
                var rotationSpeed = moveTarget.ValueRO.RotationSpeed;
                var currentPosition = transform.ValueRO.Position;
                var direction = targetPosition - currentPosition;
                var distance = math.length(direction);
                if (distance > stopDistance)
                {
                    var moveDirection = direction / distance;
                    //先转身，再移动
                    var targetRotation = quaternion.LookRotationSafe(moveDirection, math.up());
                    var currentRotation = transform.ValueRO.Rotation;
                    var newRotation = math.slerp(currentRotation, targetRotation, rotationSpeed * deltaTime);
                    if (math.angle(currentRotation, newRotation) > 0.01f)
                    {

                        transform.ValueRW.Rotation = newRotation;
                    }
                    else 
                    {
                        var moveDistance = math.min(moveSpeed * deltaTime, distance);
                        var newPosition = currentPosition + moveDirection * moveDistance;
                        transform.ValueRW.Position = newPosition;
                    }
                }
            }
        }
    }
}