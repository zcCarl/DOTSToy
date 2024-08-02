using Common;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class SpawnerAuthoring : MonoBehaviour
    {
        private class SpawnerBaker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                // 创建 BlobBuilder
                BlobBuilder builder = new BlobBuilder(Allocator.Temp);
                ref CharacterConfigBlob configBlob = ref builder.ConstructRoot<CharacterConfigBlob>();
                var spawner = new CharacterSpawner()
                {
                    MaxWaitCount = 20,
                    CurrentWaitIndex = -1,
                    CurrentWaitTime = 0,
                    CurrentWaitCount = 0,
                };
                // 分配并初始化 BlobArray
                BlobBuilderArray<CharacterConfig> waitQueueArray = builder.Allocate(ref configBlob.WaitQueue, spawner.MaxWaitCount);
                // 创建 BlobAssetReference 并分配给组件
                spawner.WaitQueueBlob = builder.CreateBlobAssetReference<CharacterConfigBlob>(Allocator.Persistent);
                AddComponent(entity, spawner);
                // 释放 BlobBuilder
                builder.Dispose();
            }
        }
    }
}