using System.Numerics;

namespace QubeesUtility.Runtime.QubeesUtility.Helpers
{
    public static class UIHelpers
    {
        public static Vector2 GetVector2FromPivot(PivotPreset preset)
        {
            switch (preset)
            {
                case (PivotPreset.TopLeft):
                {
                    return new Vector2(0, 1);
                }
                case (PivotPreset.TopCenter):
                {
                    return new Vector2(0.5f, 1);
                }
                case (PivotPreset.TopRight):
                {
                    return new Vector2(1, 1);
                }
                case (PivotPreset.MiddleLeft):
                {
                    return new Vector2(0, 0.5f);
                }
                case (PivotPreset.MiddleCenter):
                {
                    return new Vector2(0.5f, 0.5f);
                }
                case (PivotPreset.MiddleRight):
                {
                    return new Vector2(1, 0.5f);
                }
                case (PivotPreset.BottomLeft):
                {
                    return new Vector2(0, 0);
                }
                case (PivotPreset.BottomCenter):
                {
                    return new Vector2(0.5f, 0);
                }
                case (PivotPreset.BottomRight):
                {
                    return new Vector2(1, 0);
                }
            }
            return default;
        }
    }
    public enum PivotPreset
    {
        TopLeft, TopCenter, TopRight,
        MiddleLeft, MiddleCenter, MiddleRight,
        BottomLeft, BottomCenter, BottomRight,
    }

}