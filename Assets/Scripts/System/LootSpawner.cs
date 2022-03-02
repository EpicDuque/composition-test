using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [SerializeField] private List<Loot> lootPool;
    
    [Space(10f)]
    [SerializeField] private Transform location;
    [SerializeField] private int amount = 1;
    [SerializeField] private float spawnMinForce = 1f;
    [SerializeField] private float spawnMaxForce = 3f;

    public int Amount
    {
        get => amount;
        set => amount = value;
    }

    public List<Loot> LootPool => lootPool;

    public void SpawnLoot()
    {
        for (var i = 0; i < amount; i++)
        {
            var randomInt = Random.Range(0, lootPool.Count);
            var item = lootPool[randomInt];

            var spawned = Instantiate(item, location.position, Quaternion.identity);

            var randomForce = new Vector3(
                Random.Range(spawnMinForce, spawnMaxForce), 
                Random.Range(spawnMinForce, spawnMaxForce), 
                Random.Range(spawnMinForce, spawnMaxForce));
                
            spawned.Rigidbody.AddForce(randomForce, ForceMode.Impulse);
        }
    }
}
