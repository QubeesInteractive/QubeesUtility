using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace QubeesUtility.Runtime.QubeesUtility.Helpers
{
    public class VectorHelpers
    {
        public static Vector3 GetRandomVector3(float min, float max)
        {
            return new Vector3(Random.Range(min, max), Random.Range(min, max), 
                Random.Range(min, max));
        }
        
        public static Vector3 GetRandomVector3(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 
                Random.Range(minZ, maxZ));
        }

        public static Vector2 GetRandomVector2(float min, float max)
        {
            return new Vector2(Random.Range(min, max), Random.Range(min, max));
        } 
        
        public static Vector2 GetRandomVector2(float minX, float maxX, float minY, float maxY)
        {
            return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        } 
    }
}