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
      
      public Vector2d(ICoordinates source)
      {
         arr = new double[2];
         arr[0] = source.X;
         arr[1] = source.Y;
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

      public double[] ToArray()
      {
         return arr;
      }

      public Vector ToVector()
      {
         return new Vector(arr);
      }

      public Vector3d ToVector3d()
      {
         return new Vector3d(arr);
      }


      public Point3d ToPoint3d()
      {
         return new Point3d(Vx, Vy);
      }

      public Point2d ToPoint2d()
      {
         return new Point2d(Vx, Vy);
      }

      /// <summary>
      /// Checks if two vectors are perpendicular.
      /// </summary>
      /// <param name="u">Vector2.</param>
      /// <param name="v">Vector2.</param>
      /// <returns>True if are perpendicular or false in any other case.</returns>
      public static bool ArePerpendicular(Vector2d u, Vector2d v)
      {
         return ArePerpendicular(u, v, Calc.Epsilon);
      }

      /// <summary>
      /// Checks if two vectors are perpendicular.
      /// </summary>
      /// <param name="u">Vector2.</param>
      /// <param name="v">Vector2.</param>
      /// <param name="threshold">Tolerance used.</param>
      /// <returns>True if are perpendicular or false in any other case.</returns>
      public static bool ArePerpendicular(Vector2d u, Vector2d v, double threshold)
      {
         return Calc.IsZero(u / v, threshold);
      }

      /// <summary>
      /// Checks if two vectors are parallel.
      /// </summary>
      /// <param name="u">Vector2.</param>
      /// <param name="v">Vector2.</param>
      /// <returns>True if are parallel or false in any other case.</returns>
      public static bool AreParallel(Vector2d u, Vector2d v)
      {
         return AreParallel(u, v, Calc.Epsilon);
      }

      /// <summary>
      /// Checks if two vectors are parallel.
      /// </summary>
      /// <param name="u">Vector2.</param>
      /// <param name="v">Vector2.</param>
      /// <param name="threshold">Tolerance used.</param>
      /// <returns>True if are parallel or false in any other case.</returns>
      public static bool AreParallel(Vector2d u, Vector2d v, double threshold)
      {
         return Calc.IsZero(u % v, threshold);
      }

      /// <summary>
      /// Rounds the components of a vector.
      /// </summary>
      /// <param name="u">Vector2.</param>
      /// <param name="numDigits">Number of decimal places in the return value.</param>
      /// <returns>Vector2.</returns>
      public static Vector2d Round(Vector2d u, int numDigits)
      {
         return new Vector2d(Math.Round(u.Vx, numDigits), Math.Round(u.Vy, numDigits));
      }

      /// <summary>
      /// Возвращает косинус угла между векторами.
      /// </summary>
      /// <param name="v1">Двумерный вектор.</param>
      /// <param name="v2">Двумерный вектор.</param>
      /// <returns>Косинус угла между векторами.</returns>
      public static double CosAngleBetVectors(Vector2d v1, Vector2d v2)
      {
         return (v1.Vx * v2.Vx + v1.Vy * v2.Vy) / (Sqrt(v1.Vx * v1.Vx + v1.Vy * v1.Vy) * Sqrt(v2.Vx * v2.Vx + v2.Vy * v2.Vy));
      }

      /// <summary>
      /// Возвращает угол между векторами.
      /// </summary>
      /// <param name="v1">Двумерный вектор.</param>
      /// <param name="v2">Двумерный вектор.</param>
      /// <returns>Угол между векторами в радианах.</returns>
      public static double AngleBetVectors(Vector2d v1, Vector2d v2)
      {
         return Acos(CosAngleBetVectors(v1, v2));
      }

      /// <summary>
      /// Возвращает скалярное произведение двух векторов.
      /// </summary>
      /// <param name="u">Двумерный вектор.</param>
      /// <param name="v">Двумерный вектор.</param>
      /// <returns>Точечный продукт.</returns>
      public static double operator /(Vector2d u, Vector2d v)
      {
         return u.Vx * v.Vx + u.Vy * v.Vy;
      }

      /// <summary>
      /// Возвращает векторное произведение двух векторов.
      /// </summary>
      /// <param name="v1">Двумерный вектор.</param>
      /// <param name="v2">Двумерный вектор.</param>
      /// <returns>Число (кросс-продукт).</returns>
      public static double operator %(Vector2d u, Vector2d v)
      {
         return u.Vx * v.Vy - u.Vy * v.Vx;
      }

      /// <summary>
      /// Возвращает векторное произведение двух векторов.
      /// </summary>
      /// <param name="u">Двумерный вектор.</param>
      /// <param name="v">Двумерный вектор.</param>
      /// <returns>Трехмерный вектор (нормаль).</returns>
      public static Vector3d operator ^(Vector2d u, Vector2d v)
      {
         return new Vector3d
         {
            Vx = 0,
            Vy = 0,
            Vz = u.Vx * v.Vy - u.Vy * v.Vx
         };
      }

      /// <summary>
      /// Возвращает поэлементное произведение двух векторов.
      /// </summary>
      /// <param name="v1">Двумерный вектор.</param>
      /// <param name="v2">Двумерный вектор.</param>
      /// <returns>Двумерный вектор.</returns>
      public static Vector2d operator *(Vector2d v1, Vector2d v2)
      {
         return new Vector2d
         {
            Vx = v1.Vx * v2.Vx,
            Vy = v1.Vy * v2.Vy
         };
      }

      /// <summary>
      /// Умножение вектора на число.
      /// </summary>
      /// <param name="v1">Двумерный вектор.</param>
      /// <param name="v2">Число.</param>
      /// <returns>Двумерный вектор.</returns>
      public static Vector2d operator *(Vector2d v1, int v2)
      {
         return new Vector2d
         {
            Vx = v1.Vx * v2,
            Vy = v1.Vy * v2
         };
      }

      /// <summary>
      /// Умножение вектора на число.
      /// </summary>
      /// <param name="v1">Двумерный вектор.</param>
      /// <param name="v2">Число.</param>
      /// <returns>Двумерный вектор.</returns>
      public static Vector2d operator *(Vector2d v1, double v2)
      {
         return new Vector2d
         {
            Vx = v1.Vx * v2,
            Vy = v1.Vy * v2
         };
      }

      /// <summary>
      /// Возвращает сумму двух векторов.
      /// </summary>
      /// <param name="v1">Двумерный вектор.</param>
      /// <param name="v2">Двумерный вектор.</param>
      /// <returns>Двумерный вектор.</returns>
      public static Vector2d operator +(Vector2d v1, Vector2d v2)
      {
         return new Vector2d
         {
            Vx = v1.Vx + v2.Vx,
            Vy = v1.Vy + v2.Vy
         };
      }

      /// <summary>
      /// Возвращает разность двух векторов.
      /// </summary>
      /// <param name="v1">Двумерный вектор.</param>
      /// <param name="v2">Двумерный вектор.</param>
      /// <returns>Двумерный вектор.</returns>
      public static Vector2d operator -(Vector2d v1, Vector2d v2)
      {
         return new Vector2d
         {
            Vx = v1.Vx - v2.Vx,
            Vy = v1.Vy - v2.Vy
         };
      }

   }
}
