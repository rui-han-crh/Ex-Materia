using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//FOR FUTURE USE: 
//ALL SCRIPTS THAT WANT TO SAVE SOME DATA NEED TO ADD THIS IN!
public interface IDataPersistence
{
    void LoadData(GameData data);
    void SaveData(GameData data);
}
