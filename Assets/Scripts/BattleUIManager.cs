using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleUIManager : MonoBehaviour // 옵저버패턴
{
    [Header("UI 연결")]
    public TextMeshProUGUI txtPlayerStatus;
    public TextMeshProUGUI txtGuidePath;

    private void Start()
    {
        if (PlayerStats.instance != null)
        {
            PlayerStats.instance.OnStatsChanged += RefreshTutorialUI;
        }

        Invoke("RefreshTutorialUI", 0.1f);
    }

    public void RefreshTutorialUI()
    {
        if (MDPManager.instance == null || PlayerStats.instance == null) return;

        int realAtk = PlayerStats.instance.atk;
        int realHp = PlayerStats.instance.hp;

        int playerRegionIndex = PlayerStats.instance.currentRegionIndex;

        MDPManager.instance.UpdateTutorialPath(realAtk, realHp);

        txtPlayerStatus.text = $"[Status]\n ATK : {realAtk} / HP : {realHp}";
        int nextBestRegionIndex = MDPManager.instance.policy[playerRegionIndex];

        string currentPath = MDPManager.instance.regions[playerRegionIndex].regionName;
        string nextPath = MDPManager.instance.regions[nextBestRegionIndex].regionName;

        txtGuidePath.text = $"[Currnet Location]\n {currentPath}\n[Recommended Target]\n Move to {nextPath}";
    }

    private void OnDestroy()
    {
        if (PlayerStats.instance != null)
        {
            PlayerStats.instance.OnStatsChanged -= RefreshTutorialUI;
        }
    }
}