using UnityEngine;
using System.Collections;

namespace IsoEngine1
{
    public enum EVectorComponents
    {
        X,
        Y,
        Z,
        XY,
        XZ
    }

    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2Int(float x, float y)
        {
            this.x = (int)x;
            this.y = (int)y;
        }

        public Vector2Int(Vector2 v)
        {
            this.x = (int)v.x;
            this.y = (int)v.y;
        }

        public Vector2Int(Vector3 v, EVectorComponents evc)
        {
            if (evc == EVectorComponents.XY)
            {
                this.x = (int)v.x;
                this.y = (int)v.y;
            }
            else if (evc == EVectorComponents.XZ)
            {
                this.x = (int)v.x;
                this.y = (int)v.z;
            }
            else
                throw new UnityException("Cannot convert Vector3 to Vector2 according to components specification " + evc);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return new Vector2(this.x, this.y).ToString();
        }
        public static bool operator ==(Vector2Int lhs, Vector2Int rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(Vector2Int lhs, Vector2Int rhs)
        {
            return lhs.x != rhs.x || lhs.y != rhs.y;
        }

        public static Vector2Int operator +(Vector2Int lhs, Vector2Int rhs)
        {
            return new Vector2Int(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static Vector2Int operator -(Vector2Int lhs, Vector2Int rhs)
        {
            return new Vector2Int(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public float magnitude
        {
            get
            {
                return this.Vector2.magnitude;
            }
        }

        public Vector2 normalized
        {
            get
            {
                return this.Vector2.normalized;
            }
        }

        public Vector2 Vector2
        {
            get
            {
                return new Vector2(this.x, this.y);

            }
        }

        public Vector3 Vector3(EVectorComponents evc)
        {
            if (evc == EVectorComponents.XY)
            {
                return new Vector3(this.x, this.y, 0f);
            }
            else if (evc == EVectorComponents.XZ)
            {
                return new Vector3(this.x, 0f, this.y);
            }
            else
                throw new UnityException("Cannot convert to Vector3 according to components specification " + evc);
        }

        public static Vector2Int One
        {
            get
            {
                return new Vector2Int(1, 1);
            }
        }
        public static Vector2Int Zero
        {
            get
            {
                return new Vector2Int(0, 0);
            }
        }
    };
}