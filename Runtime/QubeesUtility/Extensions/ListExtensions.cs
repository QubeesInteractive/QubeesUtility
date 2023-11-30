using System.Collections.Generic;

namespace QubeesUtility.Runtime.QubeesUtility.Extensions
{
    public static class ListExtensions
    {
        private static readonly System.Random Rng = new();  

        public static void Shuffle<T>(this IList<T> list)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = Rng.Next(n + 1);  
                (list[k], list[n]) = (list[n], list[k]);
            }  
        }
    }
}