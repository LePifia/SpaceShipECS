using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;
using Unity.Physics;
using Unity.Entities.UniversalDelegates;

[BurstCompile]
public partial struct BulletSystem : ISystem
{
    [BurstCompile]
    private void OnUpdate(ref SystemState state) {

        EntityManager entityManager = state.EntityManager;
        NativeArray<Entity> allEntities = entityManager.GetAllEntities();
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach (Entity entity in allEntities) {

            if (entityManager.HasComponent<BulletComponent>(entity) && entityManager.HasComponent<BulletLifeTimeComponent>(entity)) {

                //MoveTheBullet
                LocalTransform bulletTranfsform = entityManager.GetComponentData<LocalTransform>(entity);
                BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(entity);

                bulletTranfsform.Position += bulletComponent.speed * SystemAPI.Time.DeltaTime * bulletTranfsform.Right();
                entityManager.SetComponentData(entity, bulletTranfsform);

                //Timer
                BulletLifeTimeComponent bulletLifeTimeComponent = entityManager.GetComponentData<BulletLifeTimeComponent>(entity);
                bulletLifeTimeComponent.RemainingLifetime -= SystemAPI.Time.DeltaTime;

                if (bulletLifeTimeComponent.RemainingLifetime <= 0f){
                    entityManager.DestroyEntity(entity);
                    continue;
                }
                entityManager.SetComponentData(entity, bulletLifeTimeComponent);


                //Physics
                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                float3 point1 = new float3(bulletTranfsform.Position - bulletTranfsform.Right() * 0.05f);
                float3 point2 = new float3(bulletTranfsform.Position + bulletTranfsform.Right() * 0.05f);

                uint layerMask = LayerMaskHelper.GetLayerMaskFromTwoLayers(CollisionLayer.Wall, CollisionLayer.Enemy);

                physicsWorldSingleton.CapsuleCastAll(point1, point2, bulletComponent.size/2, float3.zero, 1f, ref hits, new CollisionFilter{

                    BelongsTo = (uint)CollisionLayer.Default,
                    CollidesWith = layerMask
                });

                if (hits.Length > 0){

                    for (int i = 0;  i< hits.Length; i++){
                        Entity hitEntity = hits[i].Entity;

                        if (entityManager.HasComponent<EnemyComponent>(hitEntity)){
                            EnemyComponent enemyComponent = entityManager.GetComponentData<EnemyComponent>(hitEntity);
                            enemyComponent.currentHealth -= bulletComponent.damage;
                            entityManager.SetComponentData(hitEntity,enemyComponent);

                            if (enemyComponent.currentHealth <= 0f){
                                entityManager.DestroyEntity(hitEntity);
                            }
                        }
                    }

                    entityManager.DestroyEntity(entity);
                }

                hits.Dispose();
            }
        }
     }
}
