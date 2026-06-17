using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewRegion", menuName = "MDP/RegionData")]
public class RegionDataSO : ScriptableObject
{
    public string regionName;
    public enum RegionType {Village, Shop, HuntingZone, BossArea }
    public RegionType regionType;

    [Header("전투 밸런스")]
    public int ATK; // need attack power
    public int Exp; // need Exp
    public int hpPenalty; // need damage
    public int missionCount; // hunting count
}
