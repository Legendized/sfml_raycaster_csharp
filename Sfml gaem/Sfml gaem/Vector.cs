using System;
using System.Collections.Generic;
using System.Text;

namespace Sfml_gaem //credit to https://github.com/garciadelcastillo for this.
{
    /// <summary>
    /// Represents a three dimensional vector.
    /// </summary>
    public class Vector
    {
        // Properties
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        //indexer
        public float this[int i]
        {
            get
            {
                if (i == 0)
                {
                    return this.X;
                }
                else if (i == 1)
                {
                    return this.Y;
                }
                else if (i == 2)
                {
                    return this.Z;
                }
                throw new Exception();
            }
            set
            {
                if (i == 0)
                {
                    this.X = value;
                }
                else if (i == 1)
                {
                    this.Y = value;
                }
                else if (i == 2)
                {
                    this.Z = value;
                }
                else
                {
                    throw new Exception();
                }
            }
        }


        //static properties
        public static Vector XAxis
        {
            get => new Vector(1, 0, 0);
        }
        public static Vector YAxis
        {
            get => new Vector(0, 1, 0);
        }
        public static Vector ZAxis
        {
            get => new Vector(0, 0, 1);
        }

        public float Length
        {
            get
            {
                return GetLength();
            }
        }

        //constructors
        public Vector()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        public Vector(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector(Vector other)
        {
            this.X = other.X;
            this.Y = other.Y;
            this.Z = other.Z;
        }

        //methods
        private float GetLength()
        {
            float sql = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
            float len = MathF.Sqrt(sql);
            return len;
        }

        public void Reverse()
        {
            this.X = -this.X;
            this.Y = -this.Y;
            this.Z = -this.Z;
        }

        public void Scale(float factor)
        {
            this.X *= factor;
            this.Y *= factor;
            this.Z *= factor;
        }

        public bool Unitize()
        {
            float len = this.GetLength();
            if (len <= 0)
            {
                return false;
            }

            this.X /= len;
            this.Y /= len;
            this.Z /= len;
            return true;
        }

        public void Add(Vector other)
        {
            this.X += other.X;
            this.Y += other.Y;
            this.Z += other.Z;
        }

        //operator overloads
        public static Vector operator +(Vector a, Vector b)
        {
            return Vector.Addition(a, b);
        }

        public static float operator *(Vector a, Vector b)
        {
            return Vector.DotProduct(a, b);
        }

        public static Vector operator *(float a, Vector b)
        {
            Vector v = new Vector(b);
            v.Scale(a);
            return v;
        }

        //static methods
        public static Vector Addition(Vector a, Vector b)
        {
            float newX = a.X + b.X;
            float newY = a.Y + b.Y;
            float newZ = a.Z + b.Z;
            Vector v = new Vector(newX, newY, newZ);
            return v;
        }

        public static float DotProduct(Vector a, Vector b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static Vector CrossProduct(Vector a, Vector b)
        {
            float x = a.Y * b.Z - a.Z * b.Y;
            float y = a.Z * b.X - a.X * b.Z;
            float z = a.X * b.Y - a.Y * b.X;
            return new Vector(x, y, z);
        }


        //overrides
        public override string ToString()
        {
            return $"[{this.X}, {this.Y}, {this.Z}]";
        }

    }
}
