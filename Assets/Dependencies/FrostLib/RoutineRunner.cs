using System.Collections;
using UnityEngine;

namespace FrostLib
{
    public class RoutineRunner : MonoBehaviour
    {
        public static RoutineRunner Create()
        {
            var instance = new GameObject("RoutineRunner").AddComponent<RoutineRunner>();
            DontDestroyOnLoad(instance.gameObject);
            return instance;
        }

        public Coroutine StartRoutine(IEnumerator routine) => StartCoroutine(routine);
    }
}