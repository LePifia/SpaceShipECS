using Unity.Entities;

public struct EnemySpawnerComponent : IComponentData
{
    public Entity enemyPrefabToSpawn;
    public int numOfEnemiesPerSecond;
    public int numOfEnemiesIncrementAmount;
    public int maxNumOfEnemiesPerSecond;
    public float enemySpawnRadious;
    public float minimunDistanceFromPlayer;

    public float timeBeforeNextSpawn;
    public float currentTimeBeforeNextSpawn;


}
