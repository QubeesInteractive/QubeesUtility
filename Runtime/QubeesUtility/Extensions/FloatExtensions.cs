using System;
using UnityEngine;

namespace qubees_utility.Runtime.QubeesUtility.Extensions
{
    public static class FloatExtensions
    {
        public static string FormatNumberAsDecimalString(this float numberToFormat, int decimalCount = 1)
        {
            var suffix = DetectSuffix(numberToFormat);
            var divider = Mathf.Pow(10, (int)suffix * 3);
            var integerPart = (int)(numberToFormat / divider);
            var returnString = integerPart.ToString();
            if (decimalCount > 0 )
            {
                decimalCount = decimalCount > 3 ? 3 : decimalCount;
                var decimalPart = (numberToFormat % divider).ToString("000");
                decimalPart = decimalPart[..decimalCount];
                returnString += "." + decimalPart;
            }
            if (suffix == NumberSuffix.None) return returnString;
            returnString += " " + suffix;;
            return returnString;
        }

        private static NumberSuffix DetectSuffix(float number, int suffixIndicator = 0)
        {
            if (number > Math.Pow(10, (suffixIndicator + 1) * 3))
            {
                return DetectSuffix(number, suffixIndicator + 1);
            }
            return (NumberSuffix)(suffixIndicator);
        }
    }
        public enum NumberSuffix
        {
            None,
            K,
            M,
            B,
            T,
            Q,
        }
}