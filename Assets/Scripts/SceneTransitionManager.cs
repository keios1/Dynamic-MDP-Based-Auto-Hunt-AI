using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 로딩을 위해 필요
using UnityEngine.UI;              // UI 조작을 위해 필요

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance { get; private set; }

    [Header("페이드 UI 연결")]
    // 캔버스 그룹을 사용하면 이미지나 패널의 투명도(Alpha)를 한 번에 조절할 수 있습니다.
    public CanvasGroup fadeGroup;
    public float fadeDuration = 1.0f; // 페이드에 걸리는 시간 (초)

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 버튼 OnClick 등에서 이 함수를 호출하여 씬을 이동합니다.
    /// 예: SceneTransitionManager.instance.ChangeScene("BattleScene");
    /// </summary>
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        // 페이드 도중에는 유저가 다른 버튼을 누르지 못하게 터치 차단
        fadeGroup.blocksRaycasts = true;

        // 1. 화면 서서히 어둡게 (Fade Out)
        yield return StartCoroutine(Fade(1f));

        // 2. 비동기 씬 로드
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

        // 🚨 [핵심 추가] 배틀 씬이 로드된 후, 새로운 씬의 오브젝트들과 카메라가 
        // 완전히 자리를 잡고 준비할 때까지 0.1초~0.2초 정도 숨을 고르게 합니다.
        // 이 코드가 없으면 씬이 켜지는 순간의 버벅임 때문에 Fade In 연출이 통째로 씹히게 됩니다.
        yield return new WaitForSeconds(0.1f);

        // 3. 씬 로드 완료 후 화면 서서히 밝게 (Fade In)
        yield return StartCoroutine(Fade(0f));

        // 페이드 인이 완전히 끝나면 다시 터치 허용
        fadeGroup.blocksRaycasts = false;
    }

    // 투명도를 목표 수치(targetAlpha)까지 부드럽게 조절하는 코루틴
    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeGroup == null) yield break;

        // 알파값이 목표치에 도달할 때까지 반복
        while (!Mathf.Approximately(fadeGroup.alpha, targetAlpha))
        {
            // Time.deltaTime을 활용해 프레임에 상관없이 일정한 속도로 알파값 변경
            fadeGroup.alpha = Mathf.MoveTowards(fadeGroup.alpha, targetAlpha, Time.deltaTime / fadeDuration);
            yield return null;
        }
    }
}