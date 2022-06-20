using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    [MenuItem("📈 Cheats 🍕/Get Resources 🤑💲")]
    public static void UnlockAllLevels()
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
}