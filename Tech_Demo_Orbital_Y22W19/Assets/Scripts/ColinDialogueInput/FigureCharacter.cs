using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Dialogues/Characters")]
public class FigureCharacter : ScriptableObject
{
    //may find a use one day
    public enum EmotionType
    {
        Angry, 
        Disgusted, 
        Elated, 
        Happy, 
        Neutral, 
        Sad, 
        Surprised
    }

    //public static Dictionary<string, FigureCharacter> DictionaryBase;
    public string CharName;
    public Sprite Angry, Disgusted, Elated, Happy, Neutral, Sad, Surprised;
    // Start is called before the first frame update
    private Dictionary<string, Sprite> emotions = new Dictionary<string, Sprite>();


    public Sprite GetEmotion(string emotion)
    {

        return this.emotions[emotion];
    }


    public void InitializeDict()
    {

        emotions["Angry"] = Angry;
        emotions["Disgusted"] = Disgusted;
        emotions["Elated"] = Elated;
        emotions["Happy"] = Happy;
        emotions["Neutral"] = Neutral;
        emotions["Sad"] = Sad;
        emotions["Surprised"] = Surprised;
    }
}