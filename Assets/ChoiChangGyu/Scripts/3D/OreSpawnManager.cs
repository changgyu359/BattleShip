using UnityEngine;
using System.Collections.Generic;

public class OreSpawnManager : MonoBehaviour
{
    [SerializeField]
    private Ore baseOrePrefab;
    [SerializeField]
    private Transform[] spawnPoint;
    [SerializeField]
    private OreSO[] spawnOreData;
    private float spawnInterval = 30f;

    private List<Ore> oreList= new List<Ore>();

    private void Start()
    {
        for(int i = 0; i < spawnPoint.Length; i++)
        {
            Ore ore = Instantiate(baseOrePrefab, spawnPoint[i].position, Quaternion.identity);
            OreSO oreData = spawnOreData[Random.Range(0, spawnOreData.Length)];
            ore.Setup(oreData);
            oreList.Add(ore);
        }

        InvokeRepeating(nameof(SpawnOre),spawnInterval,spawnInterval);
    }


    private void SpawnOre()
    {
        

        
        for(int i = 0;i<oreList.Count;i++)
        {
            if (!oreList[i].gameObject.activeSelf)
            {
                OreSO oreData = spawnOreData[Random.Range(0, spawnOreData.Length)];
                oreList[i].Setup(oreData);
            }
        }


    }
}
