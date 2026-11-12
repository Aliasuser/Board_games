using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerScript : MonoBehaviour
{
    [Tooltip("Small delay so the click sound can be heard before the scene changes.")]
    public float loadDelay = 0.2f;

    /// <summary>Loads a scene by name, e.g. "Settings" or "MainMenu". Can be used from a Button OnClick.</summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAfterDelay(sceneName));
    }

    public void ExitGame()
    {
        StartCoroutine(Exit());
    }

    IEnumerator LoadAfterDelay(string sceneName)
    {
        yield return new WaitForSecondsRealtime(loadDelay);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator Exit()
    {
        yield return new WaitForSeconds(0.8f);

        /*#if UNITY_EDITOR
                // UnityEditor only exists inside the editor, so it must be wrapped,
                // otherwise building the game fails.
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif*/

        if (UnityEditor.EditorApplication.isPlaying)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }
}
