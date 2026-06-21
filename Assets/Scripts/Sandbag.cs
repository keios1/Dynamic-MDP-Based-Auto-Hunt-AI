using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandbag : MonoBehaviour
{
    [Header("»÷µå¹é ¼³Á¤")]
    public int maxHits = 3;
    private int currentHits = 0;

    private void OnMouseDown()
    {
        if (PlayerStats.instance == null) return;

        currentHits++;
        Debug.Log($"Sandbag Hit : {currentHits} / {maxHits}");

        if(currentHits >= maxHits)
        {
            PlayerStats.instance.LevelUp();
            int currentLevel = PlayerStats.instance.level;

            Debug.Log($"level up -> {currentLevel}");

            currentHits = 0;   
        }
        
    }
}
