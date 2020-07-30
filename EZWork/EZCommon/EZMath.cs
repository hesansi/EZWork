using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EZWork
{
    public class EZMath : MonoBehaviour
    {
        
        /// <summary>
        /// 从给定元素中，随机抽取一个
        /// </summary>
        /// <param name="array">给定元素</param>
        /// <returns></returns>
        public static int RandomFrom(params int[] array)
        {
            var range = Enumerable.Range(0, array.Length);
            var rand = new System.Random();
            int index = rand.Next(0, array.Length);
            return array[range.ElementAt(index)];
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string RandomFrom(params string[] array)
        {
            var range = Enumerable.Range(0, array.Length);
            var rand = new System.Random();
            int index = rand.Next(0, array.Length);
            return array[range.ElementAt(index)];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringList"></param>
        /// <returns></returns>
        public static string RandomFrom( List<string> stringList)
        {
            var range = Enumerable.Range(0, stringList.Count);
            var rand = new System.Random();
            int index = rand.Next(0, stringList.Count);
            return stringList[index];
        }
        
        /// <summary>
        /// 从[min, max]范围内，排除某些整数后，随机抽取一个
        /// </summary>
        /// <param name="min">最小值，包含在范围内</param>
        /// <param name="max">最大值，包含在范围内</param>
        /// <param name="excludeArray">排除的数</param>
        /// <returns></returns>
        public static int RandomExclude(int min, int max, params int[] excludeArray)
        {
            var range = Enumerable.Range(min, max - min + 1).Where(i => !excludeArray.Contains(i));
            var rand = new System.Random();
            var excludeLength = (max - min + 1) == range.Count() ? 0 : excludeArray.Length;
            // rand.Next 不包括上边，但是本方法想要包含上边，因此+1
            int index = rand.Next(0, max - min - excludeLength + 1);
            return range.ElementAt(index);
        }
        
        
        
    }
    
}

