using System;

namespace Geo.Calc
{
   /// <summary>
   ///Класс, реализующий численный диапазон значений
   /// </summary>
   public class Range
   {
      double s;
      double e;

      /// <summary>
      /// Конструктор класса
      /// </summary>
      /// <param name="start">Начальное значение диапазона</param>
      /// <param name="end">Конечное значение диапазона</param>
      public Range(double start, double end)
      {
         s = start;
         e = end;
      }

      /// <summary>
      /// Проверка на попадание заданного значения внутрь диапазона
      /// </summary>
      /// <param name="arg">Проверяемое значение</param>
      /// <returns>Возвращает результат проверки в виде булевого значения</returns>
      public bool Affiliation(double arg)
      {
         if (arg >= s && arg <= e) return true;
         else return false;
      }

      /// <summary>
      /// Получение вектора значений путем деления диапазона на заданное число участков равной длины
      /// </summary>
      /// <param name="ndiv">Число участков деления</param>
      /// <param name="first">Включение в вектор начального значения диапазона</param>
      /// <param name="last">Включение в вектор конечного значения диапазона</param>
      /// <returns>Возвращает вектор значений в результате деления диапазона на заданное число участков равной длины</returns>
      public Vector GetVectorByNumber(int ndiv, bool first = true, bool last = true)
      {
         Vector res = null;
         if (ndiv <= 0) return res;

         double diff = e - s;
         double step = diff / ndiv;

         if (first && last)
         {
            res = new Vector(ndiv + 1);
            for (int i = 0; i < ndiv + 1; i++) res[i] = s + step * i;
         }
         else if (!first && last)
         {
            res = new Vector(ndiv);
            for (int i = 1; i <= ndiv; i++) res[i] = s + step * i;
         }
         else if (first && !last)
         {
            res = new Vector(ndiv);
            for (int i = 0; i < ndiv; i++) res[i] = s + step * i;
         }
         else
         {
            res = new Vector(ndiv - 1);
            for (int i = 1; i < ndiv - 1; i++) res[i] = s + step * i;
         }
         return res;
      }

      /// <summary>
      /// Получение вектора промежуточных значений путем деления диапазона по папаметрическому шагу
      /// </summary>
      /// <param name="stepParam">Значение параметрического шага (в долях единицы)</param>
      /// <param name="first">Включение в вектор начального значения диапазона</param>
      /// <param name="last">Включение в вектор конечного значения диапазона</param>
      /// <returns>Возвращает вектор значений в результате деления диапазона по параметрическому шагу</returns>
      public Vector GetVectorByStepParam(double stepParam, bool first = true, bool last = false)
      {
         Vector res = null;
         if (stepParam <= 0 || stepParam > 1) return res;

         double diff = e - s;
         double step = diff * stepParam;
         int ndiv = (int)Math.Floor(1.0 / stepParam);
         double balance = (1.0 % stepParam) * diff;

         if (first && last)
         {
            res = new Vector(ndiv + 3);
            res[0] = s;
            res[ndiv + 2] = e;
            for (int i = 1; i < ndiv + 1; i++) res[i + 1] = s + 0.5 * balance + step * (i - 1);
         }
         else if (!first && last)
         {
            res = new Vector(ndiv + 2);
            for (int i = 0; i < ndiv + 1; i++) res[i] = s + balance + step * i;
            res[ndiv + 1] = e;
         }
         else if (first && !last)
         {
            res = new Vector(ndiv + 2);
            for (int i = 0; i < ndiv + 1; i++) res[i] = s + step * i;           
         }
         else
         {
            res = new Vector(ndiv + 1);
            for (int i = 0; i < ndiv + 1; i++) res[i] = s + 0.5 * balance + step * i;
         }
         return res;
      }

      /// <summary>
      /// Получение вектора промежуточных значений путем деления диапазона по заданному шагу
      /// </summary>
      /// <param name="step">Шаг деления</param>
      /// <param name="align">Выравнивание с целью деления диапазона на целое число участков</param>
      /// <param name="first">Включение в вектор начального значения диапазона</param>
      /// <param name="last">Включение в вектор конечного значения диапазона</param>
      /// <returns>Возвращает вектор значений в результате деления диапазона по заданному шагу</returns>
      public Vector GetVectorByStep(double step, bool align = true, bool first = true, bool last = true)
      {
         Vector res = null;
         double diff = e - s;
         if (step <= 0 || step > diff) return res;
         
         int ndiv = (int)Math.Floor(diff / step);
         double balance = diff - ndiv * step;

         if (align)
         {
            ndiv = (int)Math.Round(diff / step);
            step = diff / ndiv;

            if (first && last)
            {
               res = new Vector(ndiv + 1);
               for (int i = 0; i < ndiv + 1; i++) res[i] = s + step * i;
            }
            else if (!first && last)
            {
               res = new Vector(ndiv);
               for (int i = 1; i <= ndiv; i++) res[i] = s + step * i;
            }
            else if (first && !last)
            {
               res = new Vector(ndiv);
               for (int i = 0; i < ndiv; i++) res[i] = s + step * i;
            }
            else
            {
               res = new Vector(ndiv - 1);
               for (int i = 1; i < ndiv - 1; i++) res[i] = s + step * i;
            }
         }
         else
         {
            if (first && last)
            {
               res = new Vector(ndiv + 3);
               res[0] = s;
               res[ndiv + 2] = e;
               for (int i = 1; i < ndiv + 1; i++) res[i + 1] = s + 0.5 * balance + step * (i - 1);
            }
            else if (!first && last)
            {
               res = new Vector(ndiv + 2);
               for (int i = 0; i < ndiv + 1; i++) res[i] = s + balance + step * i;
               res[ndiv + 1] = e;
            }
            else if (first && !last)
            {
               res = new Vector(ndiv + 2);
               for (int i = 0; i < ndiv + 1; i++) res[i] = s + step * i;
            }
            else
            {
               res = new Vector(ndiv + 1);
               for (int i = 0; i < ndiv + 1; i++) res[i] = s + 0.5 * balance + step * i;
            }
         }
         
         return res;
      }
   }
}
