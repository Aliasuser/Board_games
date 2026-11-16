using System.Collections;
using UnityEngine;

public class SetActiveScript : MonoBehaviour
{

    public GameObject window2;
    

    public void ToggleActiveAfterDelay(float delay)
    {
        StartCoroutine(ToggleActiveCoroutine(delay));
    }

    private IEnumerator ToggleActiveCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        window2.SetActive(!window2.activeSelf);
        gameObject.SetActive(!gameObject.activeSelf);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
