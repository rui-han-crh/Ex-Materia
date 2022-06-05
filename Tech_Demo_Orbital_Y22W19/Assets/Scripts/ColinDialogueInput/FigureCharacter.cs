using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Dialogues/Characters")]
public class FigureCharacter : ScriptableObject
{

    //public static Dictionary<string, FigureCharacter> DictionaryBase;
    public string CharName;
    public Sprite Angry, Disgusted, Elated, Happy, Neutral, Sad, Surprised;
    // Start is called before the first frame update
    private Dictionary<string, Sprite> emotions = new Dictionary<string, Sprite>();


    // Start is called before the first frame update

    //public FigureCharacter()
    //{   if (DictionaryBase == null) DictionaryBase = new Dictionary<string, FigureCharacter>(); }

    public Sprite GetEmotion(string emotion)
    {
        //Question: How do I initialize the dict b4 getEMotion?
        if (emotions.Count == 0)
        {
            emotions.Add("Angry", Angry);
            emotions.Add("Disgusted", Disgusted);
            emotions.Add("Elated", Elated);
            emotions.Add("Happy", Happy);
            emotions.Add("Neutral", Neutral);
            emotions.Add("Sad", Sad);
            emotions.Add("Surprised", Surprised);
        }
        return this.emotions[emotion];
    }
}