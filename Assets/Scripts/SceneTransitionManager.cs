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

    private void Start()
    {
        // 처음 씬이 켜졌을 때 검은 화면에서 서서히 밝아지도록 설정
        if (fadeGroup != null)
        {
            fadeGroup.alpha = 1f;
            StartCoroutine(Fade(0f)); // 0 = 완전 투명
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
        // 1. 화면 서서히 어둡게 (Fade Out)
        yield return StartCoroutine(Fade(1f)); // 1 = 완전 불투명(검은색)

        // 2. 비동기 씬 로드 (게임이 멈추는 것을 방지)
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

        // 3. 씬 로드 완료 후 화면 서서히 밝게 (Fade In)
        yield return StartCoroutine(Fade(0f));
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