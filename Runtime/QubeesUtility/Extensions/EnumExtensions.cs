using System;

namespace qubees_utility.Runtime.QubeesUtility.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns the next enum in order. If current enum is last enum return first enum 
        /// </summary>
        public static T Next<T>(this T src) where T : struct, Enum
        {
            // if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");
            T[] arr = (T[]) Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(arr, src) + 1;
            return (j>=arr.Length) ? arr[0] : arr[j];            
        }
        
        /// <summary>
        /// Returns previous enum element in order. If current is first enum returns last enum
        /// </summary>
        public static T Previous<T>(this T src) where T : struct, Enum
        {
            // if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");
            T[] arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(arr, src) - 1;
            return (j<=0) ? arr[^1] : arr[j];            
        }
    }
}