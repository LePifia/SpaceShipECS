using System.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public partial struct PlayerSystem : ISystem
{
    private EntityManager _entityManager;

    private Entity playerEntity;
    private Entity inputEntityM;

    private PlayerComponent playerComponent;
    private InputComponent inputComponent;

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;
        
        // Ensure singleton entities are present
        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out playerEntity) ||
            !SystemAPI.TryGetSingletonEntity<InputComponent>(out inputEntityM))
        {
            Debug.LogWarning("PlayerComponent or InputComponent singleton entity is missing.");
            return;
        }

        playerComponent = _entityManager.GetComponentData<PlayerComponent>(playerEntity);
        inputComponent = _entityManager.GetComponentData<InputComponent>(inputEntityM);

        Move(ref state);
        Shoot(ref state);
    }

    private void Move(ref SystemState state)
    {
        if (!_entityManager.HasComponent<LocalTransform>(playerEntity))
        {
            Debug.LogWarning("Player entity does not have a LocalTransform component.");
            return;
        }

        // Moving The Player
        LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(playerEntity);
        playerTransform.Position += new float3(inputComponent.Movement * playerComponent.MoveSpeed * SystemAPI.Time.DeltaTime, 0f);

        // Look Towards Mouse
        Vector2 dir = (Vector2)inputComponent.MousePos - (Vector2)Camera.main.WorldToScreenPoint(playerTransform.Position);
        float angle = math.degrees(math.atan2(dir.y, dir.x));
        playerTransform.Rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        _entityManager.SetComponentData(playerEntity, playerTransform);
    }

    private void Shoot(ref SystemState state)
    {
        if (inputComponent.Shoot){
            for (int i = 0; i < playerComponent.NumOfBulletsToSpawn; i++){
                EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);

                Entity bulletEntity = _entityManager.Instantiate(playerComponent.BulletPrefab);

                ECB.AddComponent(bulletEntity, new BulletComponent{

                    speed = 25f,
                    size = 0.25f,
                    damage = 10f

                });

                ECB.AddComponent(bulletEntity, new BulletLifeTimeComponent{
                    RemainingLifetime = 1.5f
                });


                LocalTransform bulletTransform = _entityManager.GetComponentData<LocalTransform>(bulletEntity);
                LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(playerEntity);


                bulletTransform.Rotation = playerTransform.Rotation;


                float randomOffset = UnityEngine.Random.Range(-playerComponent.BulletSpread, playerComponent.BulletSpread);
                bulletTransform.Position = playerTransform.Position + (playerTransform.Right() * 1.5f) + (bulletTransform.Up() * randomOffset);

                ECB.SetComponent(bulletEntity, bulletTransform);
                ECB.Playback(_entityManager);

                ECB.Dispose();
            }
        }
    }
}