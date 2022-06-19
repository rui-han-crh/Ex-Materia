using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler 
{
    private string dataDirPath = "";
    private string dataFileName = "";


    public FileDataHandler(string dataDP, string dataFileName)
    {
        this.dataDirPath = dataDP;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string fullpath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullpath))
        { 
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullpath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                Debug.Log("loaded data successfully loaded from file");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error occured when trying to load data to file: " + fullpath + "\n" + ex);

            }
        }
        if (loadedData == null)
        {
            Debug.Log("Error when loading data / No Data yet!");
        }
        return loadedData;


    }

    public void Save(GameData data)
    {
        string fullpath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            //cr8 directiory first if it alr doesn't exist 
            Directory.CreateDirectory(Path.GetDirectoryName(fullpath));

            string dataToStore = JsonUtility.ToJson(data, true); //the second param is just to make it nicely 

            using (StreamWriter sw = new StreamWriter(fullpath))
            {
                sw.WriteLine(dataToStore);
            }

            Debug.Log("Successfully written");

        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullpath + "\n" + e);

        }

    }


}
