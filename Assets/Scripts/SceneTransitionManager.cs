using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance { get; private set; }

    [Header("페이드 UI 연결")]
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
 
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        fadeGroup.blocksRaycasts = true;

        yield return StartCoroutine(Fade(1f));

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(Fade(0f));

        fadeGroup.blocksRaycasts = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeGroup == null) yield break;

        while (!Mathf.Approximately(fadeGroup.alpha, targetAlpha))
        {
            fadeGroup.alpha = Mathf.MoveTowards(fadeGroup.alpha, targetAlpha, Time.deltaTime / fadeDuration);
            yield return null;
        }
    }
}