using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnTouch : MonoBehaviour
{
    [Header("Settings")]
    public string sceneName = "schena 2";
    public bool requireTag = true;
    public string requiredTag = "Player";

    [Header("Scene Transition")]
    public SceneTransition sceneTransition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!requireTag || other.CompareTag(requiredTag))
        {
            if (sceneTransition != null)
            {
                sceneTransition.StartSceneTransition();
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}