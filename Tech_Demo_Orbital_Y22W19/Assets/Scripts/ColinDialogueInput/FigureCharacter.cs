using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Dialogues/Characters")]
public class FigureCharacter : ScriptableObject
{
    
    public string CharName;
    public Sprite Angry, Disgusted, Elated, Frightened, Happy, Neutral, Sad, Surprised;
    // Start is called before the first frame update

    public Sprite GetEmotion(string emotion)
    {
        switch (emotion)
        {
            case "Angry":
                return Angry;
            case "Disgusted":
                return Disgusted;
            case "Elated":
                return Elated;
            case "Frightened":
                return Frightened;
            case "Happy":
                return Happy;
            case "Neutral":
                return Neutral;
            case "Sad":
                return Sad;
            case "Surprised":
                return Surprised;
        }
        return null; 
    }
}
