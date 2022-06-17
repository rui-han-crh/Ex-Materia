using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File storage Config")]

    //you choose the filename!
    [SerializeField]
    private string fileName;
    /**
     * Singleton class that stores the "state" (which is mostly unusused
     * for the game! (Basically GameData)
     */
    private GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects = new List<IDataPersistence>();

    public static DataPersistenceManager Instance { get; private set; }

    private FileDataHandler fileDataHandler;

    public void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found > DPM in the scene. Destroying the more recent one! ");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        //can change App.persistentDataPath, but im scared of bugs
        this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }


    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }



    //sub onEnable
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded called");
        this.dataPersistenceObjects = FindAllDataPersistanceObjects();
        LoadGame();

    }


    //sub OnDisable
    public void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("OnSceneUnloaded called");
        SaveGame();
    }

    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }


    /**
     * Once you've set this, you also access all the menu items namely 
     * 1) NewGame()
     * 2) LoadGame()
     * 3) SaveGame()
     * from here
     */
    public void NewGame()
    {
        this.gameData = new GameData();
    }

    //1) Load saved data from a file using the data handler 
    public void LoadGame()
    {
        this.gameData = fileDataHandler.Load(); 
        // If gameData is not present, don't continue
        if (this.gameData == null)
        {
            Debug.Log("No data was found. A new Game needs to be started b4 data can be loaded.");
            return;
        }

        //Push loaded data to all other scripts that need it!

        foreach(IDataPersistence dpObj in dataPersistenceObjects)
        {
            dpObj.LoadData(gameData);

        }


    }


    

    public void SaveGame()
    {
        //if no data can be loaded, we cannot continue!
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A new game needs to be started before data can be loaded!");
            return;
        }

        // Pass data to other scripts to update it 
        foreach (IDataPersistence dpObj in dataPersistenceObjects)
        {
            dpObj.LoadData(gameData);

        }
        //Save data to file using data handler 
        fileDataHandler.Save(gameData);

    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    //Loads all Savable objects (that implement IDataPersistence) within the scnee!

    private List<IDataPersistence> FindAllDataPersistanceObjects()
    {
        //Sadly, they need to be monobehavs in this way
        IEnumerable<IDataPersistence> dpObjects = FindObjectsOfType<MonoBehaviour>().
        OfType<IDataPersistence>();
        return new List<IDataPersistence>(dpObjects);



    }



}
