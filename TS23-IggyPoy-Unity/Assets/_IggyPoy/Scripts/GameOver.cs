using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameOver : MonoBehaviour
{
    [SerializeField] private PairOfThings[] thingsToSpawn;
    [SerializeField] private float minDelay = 0f;
    [SerializeField] private float maxDelay = 1f;
    [SerializeField] private float maxHeight = 3f;
    [SerializeField] private float maxHorDistribution = 5f;

    private RandomEssentials rnd;

    private void Start()
    {
        rnd = new RandomEssentials();
        Invoke(nameof(SpawnCycle), 0f);
    }

    private void SpawnCycle()
    {
        PairOfThings toSpawn = thingsToSpawn[ rnd.GetRandomInt(0, thingsToSpawn.Length) ];
        Vector3 location = transform.position + new Vector3(rnd.GetRandomFloat(-maxHorDistribution, maxHorDistribution), rnd.GetRandomFloat(0, maxHeight), rnd.GetRandomFloat(-maxHorDistribution, maxHorDistribution));
        Quaternion rotation = Quaternion.Euler( new Vector3(0, rnd.GetRandomFloat(0.0f, 360.0f), 0) );
        Destroy (Instantiate(toSpawn.flash, location, rotation, transform), 5);
        Destroy (Instantiate(toSpawn.hit, location, rotation, transform), 5);

        Invoke(nameof(SpawnCycle), rnd.GetRandomFloat(minDelay, maxDelay));
    }
}


[Serializable]
public struct PairOfThings
{
    public GameObject hit;
    public GameObject flash;
}