using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneChangerScript : MonoBehaviour
{
    [Tooltip("Small delay so the click sound can be heard before the scene changes.")]
    public float loadDelay = 0.2f;


    public SaveLoadScript saveLoadScript;
    public FadeScript fadeScript;


    void Awake()
    {
        if (fadeScript == null) fadeScript = FindFirstObjectByType<FadeScript>();
        if (saveLoadScript == null) saveLoadScript = FindFirstObjectByType<SaveLoadScript>();
    }

    public void CloseGame()
    {
        StartCoroutine(Delay("quit", -1, "", -1));
    }

    public IEnumerator Delay(string action, int charIndex, string playerName, int playerCount)
    {
        if(string.Equals(action, "quit", StringComparison.OrdinalIgnoreCase))
        {
            yield return new WaitForSeconds(0.5f);
            //PlayerPrefs.DeleteAll();

            
            if (UnityEditor.EditorApplication.isPlaying) 
                UnityEditor.EditorApplication.isPlaying = false;
            else
            {
                yield return fadeScript.FadeOut(0.5f);
                Application.Quit();
            }
                
            

        }
        else if(string.Equals(action, "play", StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeOut(0.5f);
            saveLoadScript.SavePlayerData(charIndex, playerName, playerCount);
            SceneManager.LoadScene(2, LoadSceneMode.Single);
        }else if(string.Equals(action, "menu", StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeOut(0.5f);
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
        else if (string.Equals(action, "settings", StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeOut(0.5f);
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
        else if (string.Equals(action, "leaderboard", StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeOut(0.5f);
            SceneManager.LoadScene(3, LoadSceneMode.Single);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAfterDelay(sceneName));
    }

    public void ToMainMenu()
    {
        StartCoroutine(Delay("menu", -1, "", -1));
    }

    public void ToSettings()
    {
        StartCoroutine(Delay("settings", -1, "", -1));
    }

    public void ToLeaderboard()
    {
        StartCoroutine(Delay("leaderboard", -1, "", -1));
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

        /*if (UnityEditor.EditorApplication.isPlaying)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }*/

        Application.Quit();
    }
}
