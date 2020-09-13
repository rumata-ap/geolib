using System;
using System.Collections.Generic;

namespace Geo.Calc
{
   [Serializable]
   public class Matrix
   {
      private double precalculatedDeterminant = double.NaN;

      public double[,] arr { get; private set; }
      private int m, n;

      public int N
      {
         get
         {
            if (n > 0) return n;
            else return -1;
         }
      }

      public int M
      {
         get
         {
            if (m > 0) return m;
            else return -1;
         }
      }

      public double this[int i, int j]
      {
         get
         {
            if (n > 0 && m > 0)
               if (i > -1 && j > -1) return arr[i, j];
               else throw new ArgumentException("Неверно задана индексация");
            else throw new ArgumentException("Не задана матрица");
         }
         set
         {
            if (n > 0 && m > 0)
               if (i > -1 && j > -1)
                  arr[i, j] = value;
               else throw new ArgumentException("Неверно задана индексация");
            else throw new ArgumentException("Не задана матрица");
         }
      }

      public bool IsSquare { get => N == M; }

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

      private double Minor(int i, int j)
      {
         return ColumnException(j).RowException(i).Determinant();
      }

      private Matrix RowException(int row)
      {
         if (row < 0 || row >= N)
         {
            throw new ArgumentException("не верно указан индекс строки");
         }
         var result = new Matrix(N - 1, M);
         result.ProcessFunctionOverData((i, j) => result[i, j] = i < row ? this[i, j] : this[i + 1, j]);
         return result;
      }

      private Matrix ColumnException(int column)
      {
         if (column < 0 || column >= M)
         {
            throw new ArgumentException("не верно указан индекс столбца");
         }
         var result = new Matrix(N, M - 1);
         result.ProcessFunctionOverData((i, j) => result[i, j] = j < column ? this[i, j] : this[i, j + 1]);
         return result;
      }

      /// <summary>
      /// Получение нулевой матрицы.
      /// </summary>
      public static Matrix Zero
      {
         get
         {
            return new Matrix(3, 3);
         }
      }

      /// <summary>
      /// Получение единичной матрицы.
      /// </summary>
      public static Matrix Identity
      {
         get
         {
            Matrix res = new Matrix(3, 3);
            for (int i = 0; i < res.N; i++)
            {
               res[i, i] = 1;
            }
            return res;
         }
      }

      /// <summary>
      /// Mетод, позволяющий выполнить какое-либо действие над всеми элементами матрицы.
      /// </summary>
      /// <param name="func"></param>
      public void ProcessFunctionOverData(Action<int, int> func)
      {
         for (var i = 0; i < N; i++)
         {
            for (var j = 0; j < M; j++)
            {
               func(i, j);
            }
         }
      }

      /// <summary>
      /// Вычисления определителя матрицы
      /// </summary>
      public double Determinant()
      {
         if (!double.IsNaN(precalculatedDeterminant))
         {
            return precalculatedDeterminant;
         }
         if (!IsSquare)
         {
            throw new InvalidOperationException("Определитель может быть вычислен только для квадратной матрицы");
         }
         if (N == 2)
         {
            return this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];
         }
         double result = 0;
         for (var j = 0; j < M; j++)
         {
            result += (j % 2 == 1 ? 1 : -1) * this[1, j] * ColumnException(j).RowException(1).Determinant();
         }
         precalculatedDeterminant = result;
         return result;
      }

      /// <summary>
      /// Вычисление обратной матрицы методом Крамера
      /// </summary>
      public Matrix Inverse()
      {
         if (M != N) return null;

         double determinant = Determinant();
         if (Math.Abs(determinant) < 0.0000001) return null;

         Matrix result = new Matrix(N, N);
         ProcessFunctionOverData((i, j) =>
         {
            result[i, j] = ((i + j) % 2 == 1 ? -1 : 1) * Minor(i, j) / determinant;
         });

         result = result.Transpose();
         return result;
      }

      /// <summary>
      /// Создание матрицы, образованной последовательным размещением матриц-аргументов друг за другом (слева на право).
      /// </summary>
      public static Matrix Augment(params Matrix[] args)
      {
         for (int i = 1; i < args.Length; i++)
         {
            if (args[i] == null) return null;
         }
         int n = args[0].N;
         int m = args[0].M;
         for (int i = 1; i < args.Length; i++)
         {
            if (args[i].N != n) return null;
            m += args[i].M;
         }
         Matrix res = new Matrix(n, m);
         int c = 0;
         for (int k = 0; k < args.Length; k++)
         {
            for (int i = 0; i < n; i++)
            {
               for (int j = 0; j < args[k].M; j++)
               {
                  res[i, c + j] = args[k][i, j];
               }
            }
            c += args[k].M;
         }
         return res;
      }

      /// <summary>
      /// Создание матрицы, образованной последовательным размещением матриц-аргументов друг под другом.
      /// </summary>
      public static Matrix Stack(params Matrix[] args)
      {
         for (int i = 1; i < args.Length; i++)
         {
            if (args[i] == null) return null;
         }
         int n = args[0].N;
         int m = args[0].M;
         for (int i = 1; i < args.Length; i++)
         {
            if (args[i].M != m || args[i] == null) return null;
            n += args[i].N;
         }
         Matrix res = new Matrix(n, m);
         int r = 0;
         for (int k = 0; k < args.Length; k++)
         {
            for (int i = 0; i < args[k].N; i++)
            {
               for (int j = 0; j < m; j++)
               {
                  res[i + r, j] = args[k][i, j];
               }
            }
            r += args[k].N;
         }
         return res;
      }

      public static void PrintConsole(Matrix matrix, int digit)
      {
         if (matrix == null)
         {
            Console.WriteLine("Матрица не существует");
            return;
         }

         int max = 0;
         for (int i = 0; i < matrix.N; i++)
         {
            for (int j = 0; j < matrix.M; j++)
            {
               string s = Math.Round(matrix[i, j], digit).ToString();
               if (s.Length > max) max = s.Length;
            }
         }
         string opt = "{0," + max + "}";

         for (int i = 0; i < matrix.N; i++)
         {
            for (int j = 0; j < matrix.M; j++)
            {
               Console.Write(String.Format(opt, Math.Round(matrix[i, j], digit)));
            }
            Console.WriteLine();
         }
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

      public MatrixL<double> ToMatrixL()
      {
         MatrixL<double> res = new MatrixL<double>(n, m, 0);
         for (int i = 0; i < n; i++)
         {
            for (int j = 0; j < m; j++)
            {
               res[i, j] = arr[i, j];
            }
         }
         return null;
      }

      public Matrix Transpose()
      {
         Matrix res = new Matrix(M, N);
         for (int i = 0; i < N; i++)
         {
            for (int j = 0; j < M; j++)
            {
               res[j, i] = arr[i, j];
            }
         }

         return res;
      }

      public static Matrix operator *(Matrix A, Matrix B)
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

      public static Matrix operator ^(Matrix A, Matrix B)
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