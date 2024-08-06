
using UnityEngine;
using Unity.Entities;
public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject enemyPrefabToSpawn;
    public int numOfEnemiesPerSecond = 50;
    public int numOfEnemiesIncrementAmount = 2;
    public int maxNumOfEnemiesPerSecond = 200;
    public float enemySpawnRadious = 40f;
    public float minimunDistanceFromPlayer = 2f;

    public float timeBeforeNextSpawn = 1f;

    public class EnemySpawnerAuthoringBaker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            Entity enemySpawnerEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(enemySpawnerEntity, new EnemySpawnerComponent{
                enemyPrefabToSpawn = GetEntity(authoring.enemyPrefabToSpawn, TransformUsageFlags.None),

                numOfEnemiesPerSecond = authoring.numOfEnemiesPerSecond,
                numOfEnemiesIncrementAmount = authoring.numOfEnemiesIncrementAmount,

                maxNumOfEnemiesPerSecond = authoring.maxNumOfEnemiesPerSecond,
                enemySpawnRadious = authoring.enemySpawnRadious,
                minimunDistanceFromPlayer = authoring.minimunDistanceFromPlayer,
                timeBeforeNextSpawn = authoring.timeBeforeNextSpawn,


            });
        }
    }

}
