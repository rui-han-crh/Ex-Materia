using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BidirectionalMap.ExtensionMethods
{
    public static class BiMapExtension
    {
        public static BiMap<T1, T2> Clone<T1, T2>(this BiMap<T1, T2> biMap)
        {
            BiMap<T1, T2> newBiMap = new BiMap<T1, T2>();

            foreach (KeyValuePair<T1, T2> pair in biMap.Forward)
            {
                newBiMap.Add(pair.Key, pair.Value);
            }

            return newBiMap;
        }
    }
}
