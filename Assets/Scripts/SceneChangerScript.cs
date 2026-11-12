using System.Collections;
using UnityEngine;

public class SceneChangerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ExitGame()
    {
        StartCoroutine(Exit());
    }

    // Update is called once per frame
    IEnumerator Exit()
    {
        yield return new WaitForSeconds(0.8f);
        Application.Quit();

        if (UnityEditor.EditorApplication.isPlaying) UnityEditor.EditorApplication.isPlaying = false;
        else
            Application.Quit();
    }
}
