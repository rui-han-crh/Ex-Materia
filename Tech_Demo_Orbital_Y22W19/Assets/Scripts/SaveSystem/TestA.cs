using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class TestA 
{
    public Dictionary<TestB, int> dictTest;
    public TestB[] arrayTest; 


    public TestA(int a, int b)
    {
        dictTest = new Dictionary<TestB, int>();
        dictTest.Add(new TestB(a), b);
        arrayTest = new TestB[] { new TestB(a) };
    }

    public void printTest()
    {
        foreach (KeyValuePair<TestB, int> entry in dictTest)
        {
            Debug.Log("TestB dictionary test numbers (key) = : " + entry.Key.num);
            Debug.Log("TestB dictionary test numbers (Value) = : " + entry.Value);

        }


        foreach (TestB thing in arrayTest)
        {
            Debug.Log("TestB Array test numbers = : " + thing.num);

        }
    }
}
