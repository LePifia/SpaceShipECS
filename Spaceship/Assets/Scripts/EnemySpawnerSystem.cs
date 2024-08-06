using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public partial struct EnemySpawnerSystem : ISystem
{
    private EntityManager entityManager;
    private Entity enemySpawnerEntity;
    private EnemySpawnerComponent enemySpawnerComponent;
    private Entity playerEntity;

    private Unity.Mathematics.Random random;
[BurstCompile]    
public void OnCreate(ref SystemState state){
        random = Unity.Mathematics.Random.CreateFromIndex((uint) enemySpawnerComponent.GetHashCode());
    }


[BurstCompile]
    public void OnUpdate(ref SystemState state) { 

        entityManager = state.EntityManager;
        enemySpawnerEntity = SystemAPI.GetSingletonEntity<EnemySpawnerComponent>();
        enemySpawnerComponent = entityManager.GetComponentData<EnemySpawnerComponent>(enemySpawnerEntity);

        playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();

        SpawnEnemies(ref state);

    }

    private void SpawnEnemies (ref SystemState state) {

        //Timer Behaviour
        enemySpawnerComponent.currentTimeBeforeNextSpawn -= SystemAPI.Time.DeltaTime;

        

        if (enemySpawnerComponent.currentTimeBeforeNextSpawn <= 0f){
            for (int i = 0; i < enemySpawnerComponent.numOfEnemiesPerSecond; i++){

                EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);
                Entity enemyEntity = entityManager.Instantiate(enemySpawnerComponent.enemyPrefabToSpawn);

                LocalTransform enemyTransform = entityManager.GetComponentData<LocalTransform>(enemyEntity);
                LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);

                //Random Spawn Point
                float minDistanceSquared = enemySpawnerComponent.minimunDistanceFromPlayer * enemySpawnerComponent.minimunDistanceFromPlayer;
                float2 randomOffset = random.NextFloat2Direction() * random.NextFloat(enemySpawnerComponent.minimunDistanceFromPlayer, enemySpawnerComponent.enemySpawnRadious);
                float2 playerPosition = new float2(playerTransform.Position.x, playerTransform.Position.y);
                float2 spawnPosition = playerPosition + randomOffset;
                float distanceSquared = math.lengthsq(spawnPosition - playerPosition);

                if (distanceSquared > minDistanceSquared){
                    spawnPosition = playerPosition + math.normalize(randomOffset) * math.sqrt(minDistanceSquared);
                }

                enemyTransform.Position = new float3 (spawnPosition.x, spawnPosition.y, 0f);

                //spawnLook direction
                float3 direction = math.normalize(playerTransform.Position - enemyTransform.Position);
                float angle = math.atan2(direction.x, direction.y);
                angle += math.radians(90);

                quaternion lookRot = quaternion.AxisAngle(new float3 (0,0,1), angle);
                enemyTransform.Rotation = lookRot;

                ECB.SetComponent(enemyEntity, enemyTransform);

                ECB.AddComponent(enemyEntity, new EnemyComponent{
                    currentHealth = 100f,
                    enemySpeed = 1.25f

                });
                

                ECB.Playback(entityManager);
                ECB.Dispose();

            }

            //Increment number of enemies per second
            int desiredEnemiesPerWave = enemySpawnerComponent.numOfEnemiesPerSecond + enemySpawnerComponent.numOfEnemiesIncrementAmount;
            int enemiesPerWave = math.min(desiredEnemiesPerWave, enemySpawnerComponent.maxNumOfEnemiesPerSecond);
            enemySpawnerComponent.numOfEnemiesPerSecond = enemiesPerWave;

            enemySpawnerComponent.currentTimeBeforeNextSpawn = enemySpawnerComponent.timeBeforeNextSpawn;
        }

        entityManager.SetComponentData(enemySpawnerEntity,enemySpawnerComponent);


    }
}
