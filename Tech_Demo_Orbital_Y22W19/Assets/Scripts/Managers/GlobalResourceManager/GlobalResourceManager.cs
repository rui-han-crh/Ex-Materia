using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class GlobalResourceManager : MonoBehaviour
    {
        private static GlobalResourceManager instance;
        public static GlobalResourceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GlobalResourceManager>();
                    Debug.Assert(instance != null, "There is no GlobalResourceManager, consider adding one");
                }
                return instance;
            }
        }


        //public LineRenderer LineRenderer;
    }
}
