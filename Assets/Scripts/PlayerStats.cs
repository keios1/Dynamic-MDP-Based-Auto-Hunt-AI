using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance { get; private set; } // singleton

    [Header("실제 플레이어 스탯")]
    public int atk = 10;
    public int hp = 100;

    public int currentRegionIndex =0;

    public System.Action OnStatsChanged;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void ModifyStats(int atkChange, int hpChange) // damage and atk change
    {
        atk += atkChange;
        hp += hpChange;

        hp = Mathf.Max(0, hp);

        if (OnStatsChanged != null)
        {
            OnStatsChanged.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RegionTrigger trigger = collision.GetComponent<RegionTrigger>();

        if (trigger != null)
        {
            if(trigger.myRegionIndex != currentRegionIndex)
            {
                currentRegionIndex = trigger.myRegionIndex;
                Debug.Log($"New Section : {currentRegionIndex}");

                if(OnStatsChanged != null)
                {
                    OnStatsChanged.Invoke();
                }
            }
            
        }
    }
}
