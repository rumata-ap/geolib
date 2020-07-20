using System;
using System.Collections.Generic;

namespace Geo.Calc
{
   [Serializable]
   public class Matrix
   {
      public double[,] arr { get; private set; }
      int m, n;
      public int N
      {
         get
         {
            if (n > 0)
               return n;
            else
               return -1;
         }
      }
      public int M
      {
         get
         {
            if (m > 0)
               return m;
            else
               return -1;
         }
      }
      public double this[int i, int j]
      {
         get
         {
            if (n > 0 && m > 0)
               if (i > -1 && j > -1)
                  return arr[i, j];
               else
                  Console.WriteLine("Неверный индексы");
            else
               Console.WriteLine("Не задана матрица");
            return -1;
         }
         set
         {
            if (n > 0 && m > 0)
               if (i > -1 && j > -1)
                  arr[i, j] = value;
               else
                  Console.WriteLine("Неверный индексы");
            else
               Console.WriteLine("Не задана матрица");
         }
      }

      public Matrix(int N, int M)
      {
         m = M;
         n = N;
         arr = new double[N, M];
      }

      public Matrix(Matrix source)
      {
         m = source.M;
         n = source.N;
         arr = new double[source.N, source.M];
         arr = (double[,])source.arr.Clone();
      }

      public Matrix(double[,] source)
      {
         m = source.GetLength(1);
         n = source.GetLength(0);
         arr = new double[source.GetLength(0), source.GetLength(1)];
         arr = (double[,])source.Clone();
      }

      public double[,] ToArray()
      {
         return arr;
      }

      public Matrix Copy()
      {
         return new Matrix(this);
      }

      public List<double> ToList()
      {
         List<double> res = new List<double>(n * m);
         foreach (double item in arr)
         {
            res.Add(item);
         }
         return res;
      }

      public Vector ToVector()
      {
         Vector res = new Vector(n * m);
         int i = 0;
         foreach (double item in arr)
         {
            res[i] = item;
         }
         return res;
      }

      public static Matrix operator ^(Matrix A, Matrix B)
      {
         if (A.M != B.N) { throw new System.ArgumentException("Не совпадают размерности матриц"); } //Нужно только одно соответствие
         Matrix C = new Matrix(A.N, B.M); //Столько же строк, сколько в А; столько столбцов, сколько в B 
         for (int i = 0; i < A.N; ++i)
         {
            for (int j = 0; j < B.M; ++j)
            {
               C[i, j] = 0;
               for (int k = 0; k < A.M; ++k)
               { //ТРЕТИЙ цикл, до A.m=B.n
                  C[i, j] += A[i, k] * B[k, j]; //Собираем сумму произведений
               }
            }
         }
         return C;
      }

      public static Matrix operator *(Matrix A, Matrix B)
      {
         if (((A.n != B.n) || (A.m != B.m)) == true) { throw new System.ArgumentException("Не совпадают размерности матриц"); }
         double[,] res = new double[A.n, B.m];

         for (int i = 0; i < A.n; i++)
         {
            for (int j = 0; j < B.m; j++)
            {
               res[i, j] = A[i, j] * B[i, j];
            }
         }
         return new Matrix(res);
      }

      public static Vector operator *(Matrix A, Vector B)
      {
         if (A.M != B.N) { throw new System.ArgumentException("Не совпадают размерности матрицы и вектора"); }
         double[] res = new double[A.N];

         for (int i = 0; i < A.N; i++)
         {
            for (int j = 0; j < B.N; j++)
            {
               res[i] += A[i, j] * B[j];
            }
         }
         return new Vector(res);
      }

      public static Vector3d operator *(Matrix A, Vector3d B)
      {
         if (A.n != 3 && A.m != 3) { throw new System.ArgumentException("Не верна размерность матрицы"); }
         double[] res = new double[3];

         for (int i = 0; i < 3; i++)
         {
            for (int j = 0; j < 3; j++)
            {
               res[i] += A[i, j] * B[j];
            }
         }
         return new Vector3d(res);
      }

      public static Point3d operator *(Matrix A, Point3d B)
      {
         if (A.n != 3 && A.m != 3) { throw new System.ArgumentException("Не верна размерность матрицы"); }
         double[] res = new double[3];

         for (int i = 0; i < 3; i++)
         {
            for (int j = 0; j < 3; j++)
            {
               res[i] += A[i, j] * B[j];
            }
         }
         return new Point3d(res);
      }

      public static Matrix operator *(Matrix A, double B)
      {
         double[,] res = new double[A.n, A.m];

         for (int i = 0; i < A.n; i++)
         {
            for (int j = 0; j < A.m; j++)
            {
               res[i, j] = A[i, j] * B;
            }
         }

         return new Matrix(res);
      }

      public static Matrix operator +(Matrix A, Matrix B)
      {
         if (((A.n != B.n) || (A.m != B.m)) == true) { throw new System.ArgumentException("Не совпадают размерности матриц"); }
         double[,] res = new double[A.n, B.m];

         for (int i = 0; i < A.n; i++)
         {
            for (int j = 0; j < B.m; j++)
            {
               res[i, j] = A[i, j] + B[i, j];
            }
         }
         return new Matrix(res);
      }

      public static Matrix operator +(Matrix A, double B)
      {
         double[,] res = new double[A.n, A.m];

         for (int i = 0; i < A.n; i++)
         {
            for (int j = 0; j < A.m; j++)
            {
               res[i, j] = A[i, j] + B;
            }
         }

         return new Matrix(res);
      }

      public double Summ()
      {
         double sum = 0;
         foreach (double num in arr) sum += num;
         return sum;
      }
   }
}
