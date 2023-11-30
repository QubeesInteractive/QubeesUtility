using System;
using UnityEngine;

namespace QubeesUtility.Runtime.QubeesUtility.Helpers
{
    public static class JsonHelpers
    {
        public static T FromJson<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }
        
        public static T[] FromJsonArray<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.items;
        }
        
        public static string ToJson (object content)
        {
            return JsonUtility.ToJson(content);
        }
        
        public static string ToJson<T>(T[] array)
        {
            var wrapper = new Wrapper<T>
            {
                items = array
            };
            return JsonUtility.ToJson(wrapper);
        }
        
        /// <summary>
        /// Using for serialize and deserialize list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [Serializable]
        private class Wrapper<T>
        {
            public T[] items;
        }
        
        public static bool CheckPlayerPrefsForIsNullOrEmpty(string key)
        {
            return string.IsNullOrEmpty(key);
        }
    }
}