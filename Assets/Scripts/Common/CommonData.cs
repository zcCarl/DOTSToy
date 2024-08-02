using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Common
{
    
    public struct CharacterConfig
    {
        public int Id;
        public float WaitTime;
        public FixedString32Bytes Name;
    }
    // Blob 结构体定义
    public struct CharacterConfigBlob
    {
        public BlobArray<CharacterConfig> WaitQueue;
    }
    [BurstCompile]
    public struct CharacterSpawner:IComponentData
    {
        public BlobAssetReference<CharacterConfigBlob> WaitQueueBlob;
        //当前生产的角色在队列中的下标
        public int CurrentWaitIndex;
        //当前生产的角色等待的时间
        public float CurrentWaitTime;
        //在当前生产序列后还有多少个等待者
        public int CurrentWaitCount;
        //最大生产等待数量
        public int MaxWaitCount;
        public bool Running;

    }
    [BurstCompile]
    public struct SpawnerEnqueue :IComponentData
    {
        public CharacterConfig Element;
    }
    
    
    [BurstCompile]
    struct Character:IComponentData
    {
        
    }

    [BurstCompile]
    public struct Controller:IComponentData
    {
        public float3 targetPoint;
    }
    [BurstCompile]
    public struct MoveTargetComponent : IComponentData
    {
        public float3 TargetPosition;
        public float MoveSpeed;
        public float StopDistance;
        public float RotationSpeed;
    }
    
}
