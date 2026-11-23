using UnityEngine;
using System.IO;
using System;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    public GameObject[] playerPrefabs;
    int characterIndex;
    public GameObject spawnPoint;
    int[] otherPlayers;
    int otherPlayersIndex;
    private const string playerNameTxt = "playernames";


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        GameObject mainCharacter = Instantiate(playerPrefabs[characterIndex], spawnPoint.transform.position, Quaternion.identity);
        mainCharacter.GetComponent<NameScript>().SetPlayerName(PlayerPrefs.GetString("PlayerName", "PlayerName"));

        otherPlayers = new int[PlayerPrefs.GetInt("PlayerCount")];
        string[] nameArray = ReadLinesFromFile(playerNameTxt);

        for(int i= 0; i<otherPlayers.Length-1; i++)
        {
            spawnPoint.transform.position += new Vector3(0.2f, 0, 0.8f);
            otherPlayersIndex = UnityEngine.Random.Range(0, playerPrefabs.Length);
            GameObject character = Instantiate(playerPrefabs[otherPlayersIndex], spawnPoint.transform.position, Quaternion.identity);
            character.GetComponent<NameScript>().SetPlayerName(nameArray[UnityEngine.Random.Range(0, nameArray.Length)]);
        }
    }

    string[] ReadLinesFromFile(string playerNameTxt)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(playerNameTxt);
        if(textAsset != null)
        {
            return textAsset.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
           
        }
        else
        {
            Debug.LogError("File not found: " + playerNameTxt);
            return new string[0];
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
