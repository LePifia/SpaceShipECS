using System.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public partial struct EnemySystem : ISystem
{
    private EntityManager entityManager;
    private Entity playerEntity;

[BurstCompile]
    public void OnUpdate(ref SystemState state){

        entityManager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);

        NativeArray<Entity> allEntities = entityManager.GetAllEntities();

        foreach (Entity entity in allEntities){
            if (entityManager.HasComponent<EnemyComponent>(entity)){

                //MoveTowards Player
                LocalTransform enemyTransform = entityManager.GetComponentData<LocalTransform>(entity);
                EnemyComponent enemyComponent = entityManager.GetComponentData<EnemyComponent>(entity);
                float3 moveDirection = math.normalize(playerTransform.Position - enemyTransform.Position);

                enemyTransform.Position += enemyComponent.enemySpeed * SystemAPI.Time.DeltaTime * moveDirection;

                

                //Look at Player
                 float3 direction = math.normalize(playerTransform.Position - enemyTransform.Position);
                float angle = math.atan2(direction.y, direction.x); // Calcular el ángulo
                angle += math.PI; // Ajustar el ángulo para que mire en dirección opuesta

                quaternion lookRot = quaternion.RotateZ(angle); // Crear la rotación en el plano Z

                enemyTransform.Rotation = lookRot;

                entityManager.SetComponentData(entity, enemyTransform);
            }


        }

        allEntities.Dispose();
    }
}
