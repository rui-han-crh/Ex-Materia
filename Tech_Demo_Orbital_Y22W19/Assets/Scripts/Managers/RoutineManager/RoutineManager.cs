using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class RoutineManager : MonoBehaviour
    {
        private Queue<IEnumerator> routineQueue = new Queue<IEnumerator>();
        private Task lastRoutine;

        public bool IsEmpty()
        {
            return routineQueue.Count == 0;
        }

        public bool IsDormant()
        {
            return lastRoutine == null || !lastRoutine.Running;
        }

        public void Enqueue(IEnumerator coroutine)
        {
            routineQueue.Enqueue(coroutine);
        }

        public void Update()
        {
            if (IsDormant() && !IsEmpty())
            {
                lastRoutine = new Task(routineQueue.Dequeue());
            }
        }
    }
}
