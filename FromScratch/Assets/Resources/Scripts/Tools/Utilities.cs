using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtilities
{
    public static class Utilities
    {
        public static T GetRandomEnum<T>()
        {
            System.Array A = System.Enum.GetValues(typeof(T));
            T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
            return V;
        }

        public static T Choose<T>(List<T> L)
        {
            T V = (T)L[UnityEngine.Random.Range(0, L.Count)];
            return V;
        }

        public static Stack<T> ReverseStack<T> (Stack<T> input)
        {
            Stack<T> temp = new Stack<T>();

            while (input.Count != 0)
                temp.Push(input.Pop());

            return temp;
        }

        public static IEnumerator ExecuteAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
        }
    }
}
