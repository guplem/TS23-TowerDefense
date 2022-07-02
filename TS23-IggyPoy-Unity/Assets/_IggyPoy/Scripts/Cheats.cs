using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
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
    }    [MenuItem("📈 Cheats 🍕/Resources 💲/Get 100.000 Resources 💸")]
    public static void GetALotOfResources()
    {
        if (Application.isPlaying)
        {
            GameManager.instance.gameData.resources += 100000;
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
        SpawnUnit(0, 1);
    }
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Ranged Unit 🏹", false, 1)]
    public static void SpawnRanged()
    {
        SpawnUnit(1, 1);
    }
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Tank Unit 🦖", false, 2)]
    public static void SpawnTank()
    {
        SpawnUnit(2, 1);
    }
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Healer Unit 👨🏽‍⚕️", false, 3)]
    public static void SpawnHealer()
    {
        SpawnUnit(3, 1);
    }
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Smart Unit 🧠", false, 4)]
    public static void SpawnSmart()
    {
        SpawnUnit(4, 1);
    }
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Standard Unit 😐 (x5)", false, 0)]
    public static void SpawnMultipleStandard()
    {
        SpawnUnit(0, 5);
    }
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Ranged Unit 🏹 (x5)", false, 1)]
    public static void SpawnMultipleRanged()
    {
        SpawnUnit(1, 5);
    }
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Tank Unit 🦖 (x5)", false, 2)]
    public static void SpawnMultipleTank()
    {
        SpawnUnit(2, 5);
    }
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Healer Unit 👨🏽‍⚕️ (x5)", false, 3)]
    public static void SpawnMultipleHealer()
    {
        SpawnUnit(3, 5);
    }
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Smart Unit 🧠 (x5)", false, 4)]
    public static void SpawnMultipleSmart()
    {
        SpawnUnit(4, 5);
    }
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Mega horda 🏋 (5x10)", false, 50)]
    public static void SpawnMultipleAll()
    {
        SpawnUnit(0, 10);
        SpawnUnit(1, 10);
        SpawnUnit(2, 10);
        SpawnUnit(3, 10);
        SpawnUnit(4, 10);
    }
    [MenuItem("📈 Cheats 🍕/Spawn 🆕/Stress Test 🥵 (5x50)", false, 50)]
    public static void SpawnMultipleAll()
    {
        SpawnUnit(0, 50);
        SpawnUnit(1, 50);
        SpawnUnit(2, 50);
        SpawnUnit(3, 50);
        SpawnUnit(4, 50);
    }

    private static void SpawnUnit(int i, int quantity)
    {
        if (Application.isPlaying)
        {
            GameObject unit = GameManager.instance.unitsSpawner.unitsToSpawn[i].unit;
            Debug.Log($"CHEAT: Spawning unit {unit.gameObject.name}");
            GameManager.instance.unitsSpawner.SpawnUnit(unit, quantity);
        }
        else
        {
            Debug.LogError("Not in play mode.");
        }
    }
}
#endif