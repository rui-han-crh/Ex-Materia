using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class InduceEventByDataRead : MonoBehaviour
{
    public enum Primitive 
    { 
        Integer,
        Float,
        Boolean,
        String
    }

    [SerializeField]
    private UnityEvent unityEvent;

    public bool showList = false;
    public int listSize = 0;

    [Serializable]
    public class DataRead
    {
        public bool isUniversalSingletonData;
        public GameObject gameObjectData;
        public MonoBehaviour monoBehaviourScript;
        public string variableName;
        public Primitive primitiveType;

        [SerializeField]
        private string expectedString;
        [SerializeField]
        private bool expectedBool;
        [SerializeField]
        private int expectedInt;
        [SerializeField]
        private float expectedFloat;

        public object ExpectedValue
        {
            get
            {
                switch (primitiveType)
                {
                    case Primitive.Integer:
                        return expectedInt;

                    case Primitive.Float:
                        return expectedFloat;

                    case Primitive.Boolean:
                        return expectedBool;

                    case Primitive.String:
                        return expectedString;
                }
                return default;
            }

            set
            {
                expectedString = "";
                expectedInt = 0;
                expectedFloat = 0;
                expectedBool = false;

                switch (primitiveType)
                {
                    case Primitive.Integer:
                        expectedInt = (int)value;
                        break;

                    case Primitive.Float:
                        expectedFloat = (float)value;
                        break;

                    case Primitive.Boolean:
                        expectedBool = (bool)value;
                        break;

                    case Primitive.String:
                        expectedString = (string)value;
                        break;
                }
            }
        }
    }

    public List<DataRead> dataToCompare = new List<DataRead>();


    private void Start()
    {
        if (CheckAllTrueData())
        {
            unityEvent.Invoke();
        }
    }

    private bool CheckAllTrueData()
    {
        bool allTrue = true;
        foreach (DataRead data in dataToCompare)
        {
            if (data.isUniversalSingletonData)
            {
                if (!SaveFile.file.HasData(data.monoBehaviourScript.GetType(), data.variableName) 
                    || !SaveFile.file.Load(data.monoBehaviourScript.GetType(), data.variableName).Equals(data.ExpectedValue))
                {
                    string output = $"{data.variableName} was not {data.ExpectedValue}. ";
                    if (SaveFile.file.HasData(data.monoBehaviourScript.GetType(), data.variableName))
                    {
                        output += $"Instead it was {SaveFile.file.Load(data.monoBehaviourScript.GetType(), data.variableName).Equals(data.ExpectedValue)}";
                    } 
                    else
                    {
                        output += $"Instead there was no data stored for type {data.monoBehaviourScript.GetType()} {data.variableName}";
                    }
                    Debug.Log(output);
                    allTrue = false;
                    break;
                }
            } 
            else
            {
                if (!SaveFile.file.HasData(data.gameObjectData, data.monoBehaviourScript.GetType(), data.variableName)
                    || !SaveFile.file.Load(data.gameObjectData, data.monoBehaviourScript.GetType(), data.variableName).Equals(data.ExpectedValue))
                {
                    allTrue = false;
                    break;
                }
            }
        }
        return allTrue;
    } 
}
