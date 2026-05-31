using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnTouch : MonoBehaviour
{
    [Header("Target Scene")]
    [Tooltip("Выберите сцену для перехода")]
    public SceneName targetScene;

    [Header("Settings")]
    public bool requireTag = true;
    public string requiredTag = "Player";

    [Header("Transition Type")]
    public TransitionType transitionType = TransitionType.OneWay;

    public enum SceneName
    {
        schema_1,
        schema_2,
        schema_3,
        schema_4,
        schema_5
    }

    public enum TransitionType
    {
        OneWay,      // Только вперед, нельзя вернуться
        TwoWay,      // Можно вернуться обратно
        OneWayFrom   // Только если пришел с определенной сцены
    }

    [Header("OneWayFrom Settings (если выбран этот тип)")]
    public SceneName allowedFromScene;  // С какой сцены можно перейти

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!requireTag || other.CompareTag(requiredTag))
        {
            // Проверяем, можно ли перейти
            if (CanTransition())
            {
                string sceneToLoad = GetSceneName(targetScene);
                Debug.Log($"Переход на сцену: {sceneToLoad}");
                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                Debug.Log("Переход запрещен!");
            }
        }
    }

    private bool CanTransition()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        switch (transitionType)
        {
            case TransitionType.OneWay:
                // Всегда можно перейти
                return true;

            case TransitionType.TwoWay:
                // Всегда можно перейти
                return true;

            case TransitionType.OneWayFrom:
                // Можно перейти только с разрешенной сцены
                return currentScene == GetSceneName(allowedFromScene);

            default:
                return true;
        }
    }

    private string GetSceneName(SceneName scene)
    {
        switch (scene)
        {
            case SceneName.schema_1: return "schema 1";
            case SceneName.schema_2: return "schema 2";
            case SceneName.schema_3: return "schema 3";
            case SceneName.schema_4: return "schema 4";
            case SceneName.schema_5: return "schema 5";
            default: return "schema 1";
        }
    }
}