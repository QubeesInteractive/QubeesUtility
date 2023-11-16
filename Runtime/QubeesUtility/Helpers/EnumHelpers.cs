using System;
using Random = UnityEngine.Random;

namespace qubees_utility.Runtime.QubeesUtility.Helpers
{
    public static class EnumHelpers
    {
        public static T StringToEnum<T>(string data) where T : struct, Enum
        {
            Enum.TryParse(data, out T type);
            return type;
        }

        public static T RandomEnum<T>() where T : struct
        {
            var values = Enum.GetValues(typeof(T));
            var random = Random.Range(0, values.Length);
            return (T) values.GetValue(random);
        }
    }
}