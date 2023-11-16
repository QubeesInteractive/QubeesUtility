using UnityEngine;

namespace qubees_utility.Runtime.QubeesUtility.Extensions
{
    public static class TransformExtensions
    {
        public static void SetLocalXPosition(this Transform t, float newPosition) {
            var pos = t.localPosition;
            pos.x = newPosition;
            t.localPosition = pos;
        }    
        public static void SetLocalYPosition(this Transform t, float newPosition) {
            var pos = t.localPosition;
            pos.y = newPosition;
            t.localPosition = pos;
        }    
        public static void SetLocalZPosition(this Transform t, float newPosition) {
            var pos = t.localPosition;
            pos.z = newPosition;
            t.localPosition = pos;
        }    
        public static void SetLocalXScale(this Transform t, float newScale) {
            var scale = t.localScale;
            scale.x = newScale;
            t.localScale = scale;
        }    
        public static void SetLocalYScale(this Transform t, float newScale) {
            var scale = t.localScale;
            scale.y = newScale;
            t.localScale = scale;
        }    
        public static void SetLocalZScale (this Transform t, float newScale) {
            var scale = t.localScale;
            scale.z = newScale;
            t.localScale = scale;
        }
    }
}