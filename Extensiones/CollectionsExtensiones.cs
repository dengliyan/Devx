using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx
{
    public static class CollectionsExtensiones
    {
        public static void Reshuffle<T>(this List<T> listtemp)
        {
            //随机交换
            Random ram = new Random();
            int currentIndex;
            T tempValue;
            for (int i = 0; i < listtemp.Count; i++)
            {
                currentIndex = ram.Next(0, listtemp.Count - i);
                tempValue = listtemp[currentIndex];
                listtemp[currentIndex] = listtemp[listtemp.Count - 1 - i];
                listtemp[listtemp.Count - 1 - i] = tempValue;
            }
        }
    }
}
