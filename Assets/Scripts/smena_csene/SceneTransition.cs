using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public float fadeInDuration = 3f;
    public float fadeOutDuration = 3f;
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Плавная кривая
    public string sceneToLoad = "schena 2";

    [Header("UI References")]
    public CanvasGroup fadeCanvasGroup;
    public Image fadeImage;
    public Text loadingText;

    private bool isTransitioning = false;

    void Start()
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.interactable = false;
            fadeCanvasGroup.blocksRaycasts = false;
        }

        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
    }

    public void StartSceneTransition()
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionCoroutine());
        }
    }

    private IEnumerator TransitionCoroutine()
    {
        isTransitioning = true;

        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(true);
            loadingText.text = "Загрузка";
            loadingText.color = Color.white;
            loadingText.fontSize = 48;
            loadingText.alignment = TextAnchor.MiddleCenter;
            StartCoroutine(LoadingTextAnimation());
        }

        // === ПЛАВНОЕ ЗАТУХАНИЕ по кривой ===
        float elapsedTime = 0;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeInDuration;
            float alpha = fadeCurve.Evaluate(t); // Используем кривую для плавности

            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = alpha;
            }

            yield return null;
        }

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1;
        }

        yield return new WaitForSeconds(0.2f);

        // Загружаем сцену
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // === ПЛАВНОЕ РАССТУХАНИЕ по кривой ===
        elapsedTime = 0;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeOutDuration;
            float alpha = 1 - fadeCurve.Evaluate(t);

            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = alpha;
            }

            yield return null;
        }

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0;
        }

        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(false);
        }

        isTransitioning = false;
    }

    private IEnumerator LoadingTextAnimation()
    {
        int dots = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.4f);
            if (loadingText != null)
            {
                dots = (dots % 3) + 1;
                loadingText.text = "Загрузка" + new string('.', dots);
            }
        }
    }
}