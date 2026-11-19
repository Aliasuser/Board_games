using System.IO;
using UnityEngine;

public class SaveLoadScript : MonoBehaviour
{
    public string saveFileName = "playerData.json";

    [SerializeField]
    public class GameData
    {
        public int characterIndex;
        public string character;
        public int playerCount;

    }

    private GameData gameData = new GameData();
  
    public void SavePlayerData(int characterIndex, string characterName, int playerCount)
    {
        gameData.characterIndex = characterIndex;
        gameData.character = characterName;
        gameData.playerCount = playerCount;
        string json = JsonUtility.ToJson(gameData);
        File.WriteAllText(Application.persistentDataPath + "/" + saveFileName, json);
        Debug.Log("Player data saved to " + Application.persistentDataPath + "/" + saveFileName);
    }

    public void LoadPlayerData()
    {
        string filePath = Application.persistentDataPath + "/" + saveFileName;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Player data loaded from " + filePath);
            Debug.Log("Character Index: " + gameData.characterIndex + ", Character Name: " + gameData.character + ", Player Count: " + gameData.playerCount);

        }
        else
        {
            Debug.LogWarning("Save file not found at " + filePath);
        }
    }
}
