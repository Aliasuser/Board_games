
using UnityEngine;

public class ChooseCharacterScript : MonoBehaviour
{

    public GameObject[] characters;
    int characterIndex;

    public GameObject inputField;
    string characterName;
    public int playerCount = 3;
    public SceneChangerScript sceneChangerScript;

   
    void Awake()
    {
        characterIndex = 0;
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }
    }
    
    public void NextCharacter()
    {
        characters[characterIndex].SetActive(false);
        characterIndex++;
        if(characterIndex == characters.Length)
        {
            characterIndex = 0;
        }
        characters[characterIndex].SetActive(true);
    }

    public void PreviousCharacter()
    {
        characters[characterIndex].SetActive(false);
        characterIndex--;
        if (characterIndex == -1)
        {
            characterIndex = characters.Length-1;
        }
        characters[characterIndex].SetActive(true);
    }
}
