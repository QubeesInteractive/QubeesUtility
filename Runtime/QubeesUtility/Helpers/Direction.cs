using UnityEngine;

namespace QubeesUtility.Runtime.QubeesUtility.Helpers
{
    public class Direction
    {
        public readonly Vector2 DirectionVector;
        public readonly DirectionType DirectionType;

        private Direction(float x, float y, DirectionType directionType)
        {
            DirectionVector = new Vector2(x, y);
            DirectionType = directionType;
        }
        
        public static implicit operator Vector3(Direction direction)
        {
            return direction.DirectionVector;
        }
        
        public static implicit operator Vector2(Direction direction)
        {
            return direction.DirectionVector;
        }
        
        public static readonly Direction None      = new(0, 0, DirectionType.None);
        public static readonly Direction Up        = new(0, 1, DirectionType.Up);
        public static readonly Direction UpRight   = new(1, 1, DirectionType.UpRight);
        public static readonly Direction Right     = new(1, 0, DirectionType.Right);
        public static readonly Direction DownRight = new(1, -1, DirectionType.DownRight);
        public static readonly Direction Down      = new(0, -1, DirectionType.Down);
        public static readonly Direction DownLeft  = new(-1, -1, DirectionType.DownLeft);
        public static readonly Direction Left      = new(-1, 0, DirectionType.Left);
        public static readonly Direction UpLeft    = new(-1, 1, DirectionType.UpLeft);
    }

    public enum DirectionType
    {
        None,
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft,
    }
}       