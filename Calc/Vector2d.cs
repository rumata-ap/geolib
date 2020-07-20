using System;
using static System.Math;

namespace Geo.Calc
{
   [Serializable]
    public class Vector2d
    {
        double[] arr = new double[2];

        public double this[int i] { get => arr[i]; set => arr[i] = value; }
        public double Vx { get => arr[0]; set => arr[0] = value; }
        public double Vy { get => arr[1]; set => arr[1] = value; }
        public double Norma { get => Sqrt(Pow(Vx, 2) + Pow(Vy, 2)); }
        public double[] Unit { get => new double[] { Vx / Norma, Vy / Norma }; }

        public Vector2d()
        {
            arr = new double[3];
        }

        public Vector2d(double v1, double v2)
        {
            arr = new double[2];
            arr[0] = v1; arr[1] = v2;
        }

        public Vector2d(Vector2d source)
        {
            arr = new double[3];
            arr = (double[])source.arr.Clone();
        }

        public Vector2d(Point2d source)
        {
            arr = new double[2];
            arr[0] = source.X; arr[1] = source.Y;
        }

        public Vector2d(Vector source)
        {
            arr = new double[2];
            if (source.N >= 2)
            {
                arr[0] = source[0]; arr[1] = source[1];
            }
        }

        public Vector2d(double[] source)
        {
            arr = new double[2];
            if (source.Length >= 2)
            {
                arr[0] = source[0]; arr[1] = source[1];
            }
        }

        public static  double CosAngleBetVectors(Vector2d v1, Vector2d v2)
        {
            return (v1.Vx * v2.Vx + v1.Vy * v2.Vy) / (Sqrt(v1.Vx * v1.Vx + v1.Vy * v1.Vy) * Sqrt(v2.Vx * v2.Vx + v2.Vy * v2.Vy));
        }

        public double[] ToArray()
        {
            return arr;
        }

        public Vector ToVector()
        {
            return new Vector(arr);
        }

        public static Vector3d operator ^(Vector2d v1, Vector2d v2)
        {
            Vector3d tempV1 = new Vector3d(v1); Vector3d tempV2 = new Vector3d(v2);
            return new Vector3d
            {
                Vx = tempV1.Vy * tempV2.Vz - tempV1.Vz * tempV2.Vy,
                Vy = tempV1.Vz * tempV2.Vx - tempV1.Vx * tempV2.Vz,
                Vz = tempV1.Vx * tempV2.Vy - tempV1.Vy * tempV2.Vx
            };
        }

        public static Vector2d operator *(Vector2d v1, Vector2d v2)
        {
            return new Vector2d
            {
                Vx = v1.Vx * v2.Vx,
                Vy = v1.Vy * v2.Vy
            };
        }
    }
}
