using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace System.Collections.Generic
{
    public class BidirectionalDictionary<T1, T2>
    {
        private Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
        private Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

        public BidirectionalDictionary()
        {
            this.Forward = new Indexer<T1, T2>(_forward);
            this.Reverse = new Indexer<T2, T1>(_reverse);
        }

        public class Indexer<T3, T4>
        {
            private Dictionary<T3, T4> _dictionary;
            public Indexer(Dictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }

            public IEnumerable<T3> Keys => _dictionary.Keys;

            public bool ContainsKey(T3 key)
            {
                return _dictionary.ContainsKey(key);
            }

            public T4 this[T3 index]
            {
                get { return _dictionary[index]; }
                set { _dictionary[index] = value; }
            }

            public override string ToString()
            {
                return string.Join(", ", _dictionary.Select(x => x.Key + ": " + x.Value));
            }
        }

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public Indexer<T1, T2> Forward { get; private set; }
        public Indexer<T2, T1> Reverse { get; private set; }
    }
}
