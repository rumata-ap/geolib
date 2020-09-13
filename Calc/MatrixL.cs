using System;
using System.Collections.Generic;

namespace Geo.Calc
{
   public class MatrixL<T>
   {
      private List<List<T>> src;
      private T init;
      private int r;
      private int c;

      public int R
      {
         get
         {
            if (r > 0) return r;
            else return -1;
         }
      }

      public int C
      {
         get
         {
            if (c > 0) return c;
            else return -1;
         }
      }

      public T this[int i, int j]
      {
         get
         {
            if (r > 0 && c > 0)
               if (i > -1 && j > -1) return src[i][j];
               else throw new ArgumentException("Неверно задана индексация");
            else throw new ArgumentException("Не задана матрица");
         }
         set
         {
            if (r > 0 && c > 0)
               if (i > -1 && j > -1)
                  src[i][j] = value;
               else throw new ArgumentException("Неверно задана индексация");
            else throw new ArgumentException("Не задана матрица");
         }
      }

      public MatrixL(int r, int c, T initValue)
      {
         this.r = r;
         this.c = c;
         init = initValue;
         src = new List<List<T>>(r);
         for (int i = 0; i < r; i++)
         {
            src.Add(new List<T>(c));
            for (int j = 0; j < c; j++)
            {
               src[i].Add(initValue);
            }
         }
      }

      /// <summary>
      ///Извлечение строки по указанному индексу.
      /// </summary>
      /// <param name="idxR">Индекс строки для извлечения.</param>
      /// <returns></returns>
      public List<T> Row(int idxR)
      {
         if (r == 0 || idxR < 0 || idxR > r) throw new ArgumentException("Матрица пустая либо не верно указан индекс строки.");

         return src[idxR];
      }

      /// <summary>
      /// Извлечение столбца по указанному индексу.
      /// </summary>
      /// <param name="idxC">Индекс столбца для извлечения.</param>
      /// <returns></returns>
      public List<T> Col(int idxC)
      {
         if (c == 0 || idxC < 0 || idxC > c) throw new ArgumentException("Матрица пустая либо не верно указан индекс столбца.");

         List<T> res = new List<T>(r);
         foreach (List<T> item in src) res.Add(item[idxC]);

         return res;
      }

      /// <summary>
      /// Вставка строки со значениями по умолчанию по указанному индексу.
      /// </summary>
      /// <param name="idxR"> Индекс строки для вставки.</param>
      public void InsertRow(int idxR)
      {
         if (r == 0 || idxR < 0 || idxR > r) throw new ArgumentException("Матрица пустая либо не верно указан индекс строки для вставки.");

         src.Insert(idxR, new List<T>(c));
         r++;
         for (int i = 0; i < c; i++) src[idxR].Add(init);
      }

      /// <summary>
      /// Вставка строки по указанному индексу.
      /// </summary>
      /// <param name="idxR">Индекс строки для вставки.</param>
      /// <param name="row">Список значений элементов строки</param>
      public void InsertRow(int idxR, List<T> row)
      {
         if (r == 0 || idxR < 0 || idxR > r) throw new ArgumentException("Матрица пустая либо не верно указан индекс строки для вставки.");

         src.Insert(idxR, new List<T>(c));
         r++;
         int n = c;
         if (row.Count < c) n = row.Count;

         for (int i = 0; i < n; i++) src[idxR].Add(row[i]);

         if (row.Count < c)
         {
            for (int i = n; i < c; i++) src[idxR].Add(init);
         }
      }

      /// <summary>
      /// Вставка столбца со значениями по умолчанию по указанному индексу.
      /// </summary>
      /// <param name="idxС"> Индекс столбца для вставки.</param>
      public void InsertCol(int idxC)
      {
         if (c == 0 || idxC < 0 || idxC > c) throw new ArgumentException("Матрица пустая либо не верно указан индекс столбца для вставки.");

         c++;
         for (int i = 0; i < r; i++) src[i].Insert(idxC, init);
      }

      /// <summary>
      /// Вставка столбца по указанному индексу.
      /// </summary>
      /// <param name="idxC">Индекс столбца для вставки.</param>
      /// <param name="col">Список значений элементов столбца</param>
      public void InsertCol(int idxC, List<T> col)
      {
         if (c == 0 || idxC < 0 || idxC > c) throw new ArgumentException("Матрица пустая либо не верно указан индекс столбца для вставки.");

         c++;
         for (int i = 0; i < r; i++) src[i].Insert(idxC, init);

         int n = r;
         if (col.Count < r) n = col.Count;

         for (int i = 0; i < n; i++)
         {
            for (int j = 0; j < r; j++)
            {
               //src[j].Insert(idxC, col[i]);
               if (j >= n) continue;
               src[j][idxC] = col[i];
            }
         }
      }

      public void RemoveRowAt(int idxR)
      {
         if (r > 0 && idxR >= 0 && idxR <= r) { src.RemoveAt(idxR); r--; }
         else throw new ArgumentException("Матрица пустая либо не верно указан индекс строки для удаления.");
      }

      public void RemoveColAt(int idxC)
      {
         if (c > 0 && idxC >= 0 && idxC <= c)
         {
            foreach (List<T> item in src)
            {
               item.RemoveAt(idxC);
            }
            c--;
         }
         else throw new ArgumentException("Матрица пустая либо не верно указан индекс столбца для удаления.");
      }

      public List<T> ToList()
      {
         List<T> res = new List<T>(r * c);
         for (int i = 0; i < r; i++)
         {
            for (int j = 0; j < c; j++)
            {
               res.Add(src[i][j]);
            }
         }

         return res;
      }

      public Matrix ToMatrix()
      {
         Matrix res = null;
         if (init is double)
         {
            res = new Matrix(R, C);
            for (int i = 0; i < R; i++)
            {
               for (int j = 0; j < C; j++)
               {
                  res[i, j] = (double)(object)src[i][j];
               }
            }
         }

         return res;
      }
   }
}