using Common;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    public partial struct SpawnerCharacterSystem : ISystem
    {
        private EntityCommandBuffer ecb;
        public void OnCreate(ref SystemState state)
        {
        }
        
        public void OnUpdate(ref SystemState state)
        {
            // 创建一个 EntityCommandBuffer
            
            var  ecb = new EntityCommandBuffer(Allocator.TempJob);
            foreach (var ( enqueue,entity) in SystemAPI.Query<RefRO<SpawnerEnqueue>>().WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
                foreach (var spawner in SystemAPI.Query<RefRW<CharacterSpawner>>())
                {
                    int index = -1;
                    if (spawner.ValueRO.CurrentWaitIndex + spawner.ValueRO.CurrentWaitCount >= spawner.ValueRO.MaxWaitCount)
                    {
                        index = spawner.ValueRO.CurrentWaitCount -
                                (spawner.ValueRO.MaxWaitCount - spawner.ValueRO.CurrentWaitIndex);
                    }
                    else
                    {
                        index = spawner.ValueRO.CurrentWaitIndex + spawner.ValueRO.CurrentWaitCount + 1;
                    }

                    if (index < 0 || index > spawner.ValueRO.MaxWaitCount -1)
                    {
                        Debug.LogError("队列溢出");
                        continue;
                    }

                    if (!spawner.ValueRW.WaitQueueBlob.IsCreated)
                    {
                        continue;
                    }
                    
                    Debug.Log("添加到队列:" + index + " " + enqueue.ValueRO.Element.Name);
                    ref var waitQueue = ref spawner.ValueRO.WaitQueueBlob.Value.WaitQueue;
                    waitQueue[index] = enqueue.ValueRO.Element;
                }
                
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            foreach (var spawner in SystemAPI.Query<RefRW<CharacterSpawner>>())
            {
                if (!spawner.ValueRO.Running || !spawner.ValueRW.WaitQueueBlob.IsCreated)
                {
                    continue;
                }

                ref var waitQueue = ref spawner.ValueRO.WaitQueueBlob.Value.WaitQueue;
                if (spawner.ValueRO.CurrentWaitCount == 0)
                {
                    Debug.Log("生产结束");
                    spawner.ValueRW.Running = false;
                    continue;
                }

                if (spawner.ValueRO.CurrentWaitIndex == -1)
                {
                    spawner.ValueRW.CurrentWaitIndex = 0;
                    var characterConfig = waitQueue[0];
                    spawner.ValueRW.CurrentWaitTime = characterConfig.WaitTime;
                }

                if (spawner.ValueRO.CurrentWaitTime > 0)
                {
                    spawner.ValueRW.CurrentWaitTime -= SystemAPI.Time.DeltaTime;
                    if (spawner.ValueRO.CurrentWaitTime <= 0)
                    {
                        spawner.ValueRW.CurrentWaitIndex++;
                        Debug.Log("开始生产下一个:" + spawner.ValueRW.CurrentWaitIndex);
                        if (spawner.ValueRO.CurrentWaitIndex >= waitQueue.Length)
                        {
                            spawner.ValueRW.CurrentWaitIndex = 0;
                        }

                        var characterConfig = waitQueue[spawner.ValueRO.CurrentWaitIndex];
                        spawner.ValueRW.CurrentWaitTime = characterConfig.WaitTime;
                        spawner.ValueRW.CurrentWaitCount--;
                        if (spawner.ValueRO.CurrentWaitCount == 0)
                        {
                            spawner.ValueRW.Running = false;
                            spawner.ValueRW.CurrentWaitIndex = -1;
                            spawner.ValueRW.CurrentWaitCount = 0;
                            spawner.ValueRW.CurrentWaitTime = 0;
                        }
                    }
                }
            }
        }
    }
}