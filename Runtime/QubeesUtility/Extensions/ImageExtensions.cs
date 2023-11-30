using UnityEngine.UI;

namespace QubeesUtility.Runtime.QubeesUtility.Extensions
{
    public static class ImageExtensions
    {
        public static void SetAlpha(this Image i, float newAlpha) {
            var color = i.color;
            color.a = newAlpha;
            i.color = color;
        } 
    }
}