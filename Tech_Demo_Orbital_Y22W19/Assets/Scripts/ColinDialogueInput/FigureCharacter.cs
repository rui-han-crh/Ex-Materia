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
        emotions.Add("Angry", Angry);
        emotions.Add("Disgusted", Disgusted);
        emotions.Add("Elated", Elated);
        emotions.Add("Happy", Happy);
        emotions.Add("Neutral", Neutral);
        emotions.Add("Sad", Sad);
        emotions.Add("Surprised", Surprised);
    }
}