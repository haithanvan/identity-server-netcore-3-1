using System;
using System.Collections.Generic;

namespace Nmb.Shared.Utils
{
    public static class NumberUtils
    {
        public static int[] GenerateRandomArray(int min, int max, int? count = null)
        {
            var random = new Random();
            var randomNumbers = new List<int>();
            var maxCountOfNumbers = max - min;
            if (!count.HasValue)
            {
                count = maxCountOfNumbers;
            }
            else
            {
                count = maxCountOfNumbers > count ? count : maxCountOfNumbers;
            }
            
            while (randomNumbers.Count < count)
            {
                var newNumber = random.Next(min, max);
                if (!randomNumbers.Contains(newNumber))
                    randomNumbers.Add(newNumber);
            }

            return randomNumbers.ToArray();
        }
    }
}
