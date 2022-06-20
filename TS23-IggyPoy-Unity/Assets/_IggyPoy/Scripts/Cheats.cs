using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    [MenuItem("📈 Cheats 🍕/Resources 💲/Get 1.000 Resources 🤑")]
    public static void GetResources()
    {
        if (Application.isPlaying)
        {
            GameManager.instance.gameData.resources += 1000;
        }
        else
        {
            Debug.LogError("Not in play mode.");
        }
    }
    [MenuItem("📈 Cheats 🍕/Resources 💲/Remove all resources 0️⃣")]
    public static void RemoveAllResources()
    {
        if (Application.isPlaying)
        {
            GameManager.instance.gameData.resources = 0;
        }
        else
        {
            Debug.LogError("Not in play mode.");
        }
    }
    
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Standard Unit 😐", false, 0)]
    public static void SpawnStandard()
    {
        SpawnUnit(0);
    }
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Ranged Unit 🏹", false, 1)]
    public static void SpawnRanged()
    {
        SpawnUnit(1);
    }
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Tank Unit 🦖", false, 2)]
    public static void SpawnTank()
    {
        SpawnUnit(2);
    }
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Healer Unit 👨🏽‍⚕️", false, 3)]
    public static void SpawnHealer()
    {
        SpawnUnit(3);
    }
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Smart Unit 🧠", false, 4)]
    public static void SpawnSmart()
    {
        SpawnUnit(4);
    }

    private static void SpawnUnit(int i)
    {
        if (Application.isPlaying)
        {
            GameObject unit = GameManager.instance.unitsSpawner.unitsToSpawn[i].unit;
            Debug.Log($"CHEAT: Spawning unit {unit.gameObject.name}");
            GameManager.instance.unitsSpawner.SpawnUnit(unit);
        }
        else
        {
            Debug.LogError("Not in play mode.");
        }
    }
}