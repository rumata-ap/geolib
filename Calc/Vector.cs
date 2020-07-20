using System;
using System.Collections.Generic;

namespace Geo.Calc
{
   [Serializable]
   public class Vector
   {
      double[] arr;
      int n;

      public int N { get => n; }
      public double this[int i] { get => arr[i]; set => arr[i] = value; }

      public Vector(int N)
      {
         n = N;
         arr = new double[N];
      }

      public Vector(Vector source)
      {
         n = source.N;
         arr = new double[source.N];
         arr = (double[])source.arr.Clone();
      }

      public Vector(Vector3d source)
      {
         n = 3;
         arr = new double[n];
         arr[0] = source.Vx;
         arr[1] = source.Vy;
         arr[2] = source.Vz;
      }

      public Vector(double[] source)
      {
         n = source.Length;
         arr = new double[source.Length];
         arr = (double[])source.Clone();
      }

      public Vector(List<double> source)
      {
         n = source.Count;
         arr = source.ToArray();
      }

      public double[] ToArray()
      {
         return arr;
      }

      public List<double> ToList()
      {
         return new List<double>(arr);
      }

      public Matrix ToMatrix(int r, int c)
      {
         if (r*c != n) { throw new System.ArgumentException("Размерность вектора не соответствует размерности матрицы."); }
         Matrix res = new Matrix(r, c);
         int k = 0;
         for (int i = 0; i < r; i++)
         {
            for (int j = 0; j < c; j++)
            {
               res[i, j] = arr[k];
               k++;
            }
         }

         return res;
      }

      public Vector3d ToVector3d()
      {
         return new Vector3d { Vx = arr[0], Vy = arr[1], Vz = arr[2] };
      }

      public static double operator *(Matrix v1T, Vector v2)
      {
         if (v1T.M != v2.N) { throw new ArgumentException("Не совпадают размерности векторов."); }
         double res = 0;
         for (int i = 0; i < v1T.M; ++i)
         {
            res += v1T[0, i] * v2[i];
         }

         return res;
      }

      public static Matrix operator *(Vector v1, Matrix v2T)
      {
         if (v1.n != v2T.M) { throw new ArgumentException("Не совпадают размерности векторов."); }
         double[,] res = new double[v1.n,v2T.M];
         for (int i = 0; i < v1.n; ++i)
         {
            for (int j = 0; j < v2T.M; ++j)
            {
               res[i, j] = v1[i] * v2T[0, j];
            }
         }

         return new Matrix(res);
      }

      public static Vector operator *(Vector v1, Vector v2)
      {
         if (v1.n != v2.n) { throw new ArgumentException("Не совпадают размерности векторов."); }
         double[] res = new double[v1.n];
         for (int i = 0; i < v1.n; i++)
         {
            res[i] = v1[i] * v2[i];
         }

         return new Vector(res);
      }

      public static Vector operator *(Vector v1, double prime)
      {
         double[] res = new double[v1.n];
         for (int i = 0; i < v1.n; i++)
         {
            res[i] = v1[i] * prime;
         }

         return new Vector(res);
      }

      public static Vector operator /(Vector v1, Vector v2)
      {
         if (v1.n != v2.n) { throw new System.ArgumentException("Не совпадают размерности векторов."); }
         double[] res = new double[v1.n];
         for (int i = 0; i < v1.n; i++)
         {
            res[i] = v1[i] / v2[i];
         }

         return new Vector(res);
      }

      public static Vector operator /(Vector v1, double prime)
      {
         double[] res = new double[v1.n];
         for (int i = 0; i < v1.n; i++)
         {
            res[i] = v1[i] / prime;
         }

         return new Vector(res);
      }

      public static Vector operator +(Vector v1, Vector v2)
      {
         if (v1.n != v2.n) { throw new System.ArgumentException("Не совпадают размерности векторов."); }
         double[] res = new double[v1.n];
         for (int i = 0; i < v1.n; i++)
         {
            res[i] = v1[i] + v2[i];
         }

         return new Vector(res);
      }


      public static Vector operator +(Vector v1, double prime)
      {
         double[] res = new double[v1.n];
         for (int i = 0; i < v1.n; i++)
         {
            res[i] = v1[i] + prime;
         }

         return new Vector(res);
      }

      public static Vector operator -(Vector v1, Vector v2)
      {
         if (v1.n != v2.n) { throw new System.ArgumentException("Не совпадают размерности векторов."); }
         double[] res = new double[v1.n];
         for (int i = 0; i < v1.n; i++)
         {
            res[i] = v1[i] - v2[i];
         }

         return new Vector(res);
      }


      public static Vector operator -(Vector v1, double prime)
      {
         double[] res = new double[v1.n];
         for (int i = 0; i < v1.n; i++)
         {
            res[i] = v1[i] - prime;
         }

         return new Vector(res);
      }

      public double Summ()
      {
         double sum = 0;
         foreach (double num in arr) sum += num;
         return sum;
      }

      public Matrix Transpose()
      {
         Matrix res = new Matrix(1, n);
         for (int i = 0; i < n; i++)
         {
            res[0, i] = arr[i];
         }
         return res;
      }
   }
}
