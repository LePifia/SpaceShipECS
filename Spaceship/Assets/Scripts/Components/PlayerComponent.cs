using Unity.Entities;
using UnityEngine;

public struct PlayerComponent : IComponentData
{
    public float MoveSpeed;
    public Entity BulletPrefab;
    public int NumOfBulletsToSpawn;
    public float BulletSpread;
}
