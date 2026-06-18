using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MDPManager : MonoBehaviour
{
    public static MDPManager instance{  get; private set; }

    [Header("사냥터 데이터 세팅 난이도 순서")]
    public RegionDataSO[] regions;

    [Header("MDP 파라미터")]
    public float gamma = 0.9f;
    public float theta = 1e-4f;

    public int statesCount;
    public float[,] reward; //reward 
    public float[,] prob; // prob

    public float[] V;
    public int[] policy;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return; 
        }

        statesCount = regions.Length;
        reward = new float [statesCount,statesCount];
        prob = new float [statesCount,statesCount];
        V = new float [statesCount];
        policy = new int[statesCount];
    }

    public void UpdateTutorialPath(int playerAtk, int playerHp) // 플레이어 체력이 변할 때마다 호출 하여 최적의 경로 계산
    {
        RecalculateData(playerAtk, playerHp);

        System.Array.Clear(V, 0, V.Length);
        for(int i =0; i<statesCount; i++)
        {
            for(int j=0; j<statesCount; j++)
            {
                if (reward[i,j] >= -50f)
                {
                    policy[i] = j;
                    break;
                }
            }
        }
        bool isStable = false;
        int maxSafetyIter = 1000;
        int iter = 0;

        while (!isStable&&iter < maxSafetyIter)
        {
            iter++;
            PolicyEvaluation();
            isStable = PolicyImprovement();
        }

        Debug.Log($"{iter} 회 반복 후 최적 동선 수렴");

        // TODO : UI매니저 호출 하여 갱신된 polciy 배열을 바탕으로 가이드 텍스트 출력
    }

    private void RecalculateData(int playerAtk, int playerHp) // 플레이어 체력 및 공격력을 가지고 지역 데이터와 비교하여 가치 계산
    {
        for(int i =0; i<statesCount; i++) // 2차원 배열에서 제자리 걸음에 대한 확률 0, 보상 -100f으로 설정 ,i는 현재 위치
        {
            for (int j = 0; j<statesCount; j++) // j는 이동하기로 한 목표위치
            {
                if (i == j && i != statesCount - 1)
                {
                    reward[i, j] = -100f;
                    prob[i, j] = 0;
                    continue;
                }
                RegionDataSO target = regions[j]; // 현재 위치를 가져옴.

                if(target.regionType == RegionDataSO.RegionType.Village || target.regionType == RegionDataSO.RegionType.Shop)
                {
                    // 비전투구역 : 실패확률 없음, 고정 보상 부여
                    prob[i, j] = 1.0f;
                    reward[i, j] = target.regionType == RegionDataSO.RegionType.Village ? 5.0f : 2.0f; // 마을은 회복할 수 있으니 확정 보상 5, 상점은 강해질순있지만 상황에 따라 다르기에 2
                }else{
                    // 전투 구역 (HuntingZone or BossArea)

                    // 1. 승률 연산: 내 공격력 / 권장 공격력 (최소 10% ~ 최대 100%) -> 순수 공격력으로만 승률계산
                    float winRate = Mathf.Clamp((float)playerAtk / Mathf.Max(1, target.ATK), 0.1f, 1.0f);
                    prob[i, j] = winRate;

                    // 2. 기대 손실(HP) 및 기대 보상(Exp) 계산
                    float expectedHpLoss = target.hpPenalty * (1.0f - winRate);
                    float expectedReward = (target.Exp * winRate) - (expectedHpLoss * 2.0f); // 경험치 보단 체력보전을 위해 가중치 2배

                    if (playerHp - expectedHpLoss <= 0) // 확실한 패배 일 경우 
                    {
                        expectedReward = -10000f; // 차단
                    }

                    reward[i, j] = expectedReward;
                }
            }
        }

        
    }

    private void PolicyEvaluation() // 정책 계산
    {
        while (true)
        {
            float delta = 0.0f; // 오차 변수
            float[] newV = (float[])V.Clone();

            for(int i = 0; i<statesCount; i++)
            {
                int j = policy[i];
                if (reward[i, j] <= -100f) continue;

                //벨만 기대방정식
                float expectedValue = reward[i, j] + prob[i, j] * (gamma * V[j]) + (1.0f - prob[i, j]) * (gamma * V[i]);

                newV[i] = expectedValue;
                delta = Mathf.Max(delta, Mathf.Abs(newV[i] - V[i]));
            }

            V = newV;
            if (delta < theta) break; // 안정화됐다면
        }
    }

    private bool PolicyImprovement() // 정책 개선
    {
        bool policyStable = true;

        for(int i =0; i<statesCount; i++)
        {
            int oldAction = policy[i];
            float bestValue = -99999f;
            int bestAction = oldAction;

            for(int j=0; j<statesCount; j++)
            {
                if (reward[i, j] <= -100f) continue; // 제자리 걸음 방지

                float expectedValue = reward[i, j] + prob[i, j] * (gamma * V[j]) + (1.0f - prob[i, j]) * (gamma * V[i]); // 벨만 방정식

                if(expectedValue > bestValue) // 더 높을 경우
                {
                    bestValue = expectedValue;
                    bestAction = j;
                }
            }

            policy[i] = bestAction; // 최고 정책 저장
            if(oldAction!= bestAction) // 정책변경 시 아직 안정되지 않음
            {
                policyStable = false;
            }

        }

        return policyStable; // bool 값 리턴
    }

}
