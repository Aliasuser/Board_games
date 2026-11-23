using TMPro;
using UnityEngine;

public class NameScript : MonoBehaviour
{

    TextMeshPro tMP;

    void Awake()
    {
        tMP = transform.Find("NameField").gameObject.GetComponent<TextMeshPro>();
    }

    public void SetPlayerName()
    {
        tMP.text = name;
        tMP.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
    }
}
