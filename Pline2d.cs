using System.Collections.Generic;
using System.Linq;

using Geo.Calc;

namespace Geo
{
   public class Pline2d
   {
      protected List<Vertex2d> vrtxs;
      //protected List<ICurve2d> segs;
      protected BoundingBox2d bb;

      public List<Vertex2d> Vertices { get => vrtxs; internal set { vrtxs = value; CalcBB(); CalcVertices(); } }
      public BoundingBox2d BoundingBox { get => bb; }
      //public List<ICurve2d> Segments { get => segs; }
      public bool IsClosed { get; private set; }

      public Pline2d()
      {
         vrtxs = new List<Vertex2d>();
         //segs = new List<ICurve2d>();
      }

      public Pline2d(List<Point2d> points)
      {
         //segs = new List<ICurve2d>();
         vrtxs = new List<Vertex2d>();
         if (points.Count > 0)
         {
            foreach (Point2d item in points)
            {
               vrtxs.Add(new Vertex2d(item.ToPoint3d()));
            }
         }
         CalcBB();
         CalcVertices();
      }

      public Pline2d(List<Point3d> points)
      {
         //segs = new List<ICurve2d>();
         vrtxs = new List<Vertex2d>();
         if (points.Count > 0)
         {
            foreach (Point3d item in points)
            {
               vrtxs.Add(new Vertex2d(item));
            }
         }
         CalcBB();
         CalcVertices();
      }

      protected void AddVertex(Point2d pt)
      {
         vrtxs.Add(new Vertex2d(pt.ToPoint3d()));
         CalcBB();
         CalcVertices();
      }

      public void AddVertex(Point3d pt)
      {
         vrtxs.Add(new Vertex2d(pt));
         CalcBB();
         CalcVertices();
      }

      public void AddVertex(Vertex2d pt)
      {
         vrtxs.Add(pt);

         CalcBB();
         CalcVertices();
      }

      public void AddVertices(Pline2d pline)
      {
         if (pline == null || pline.Vertices.Count == 0) return;

         int id;
         if (vrtxs.Count == 0) id = 1;
         else id = vrtxs[vrtxs.Count - 1].Id + 1;

         if (vrtxs.Count == 0) { vrtxs = pline.Vertices; return; }
         if (pline.Vertices[0].X == vrtxs[vrtxs.Count - 1].X && pline.Vertices[0].Y == vrtxs[vrtxs.Count - 1].Y)
         {
            pline.vrtxs[1].Prev = vrtxs[vrtxs.Count - 1];
            for (int i = 1; i < pline.Vertices.Count; i++)
            {
               vrtxs.Add(pline.Vertices[i]);
               pline.Vertices[i].Id = id;
               id++;
            }
         }
         else
         {
            pline.vrtxs[0].Prev = vrtxs[vrtxs.Count - 1];
            pline.vrtxs[0].Pos = VertexPosition.Middle;
            //segs.Add(new Line2d(vertxs[vertxs.Count - 1], pline.vertxs[0]));
            foreach (Vertex2d item in pline.Vertices)
            {
               item.Id = id;
               id++;
               vrtxs.Add(item);
            }
         }
      }

      public void AddVertices(IEnumerable<Vertex2d> vertex2s)
      {
         List<Vertex2d> vl = new List<Vertex2d>(vertex2s);
         if (vl.Count == 0) return;

         int id;
         if (vrtxs.Count == 0) id = 1;
         else id = vrtxs[vrtxs.Count - 1].Id + 1;

         if (vrtxs.Count == 0) { vrtxs = vl; return; }
         id = vrtxs[vrtxs.Count - 1].Id + 1;
         if (vl[0].X == vl[vl.Count - 1].X && vl[0].Y == vl[vl.Count - 1].Y)
         {
            vl[1].Prev = vrtxs[vrtxs.Count - 1];
            for (int i = 1; i < vl.Count; i++)
            {
               vrtxs.Add(vl[i]);
               vl[i].Id = id;
               id++;
            }
         }
         else
         {
            vl[0].Prev = vrtxs[vrtxs.Count - 1];
            vl[0].Pos = VertexPosition.Middle;
            foreach (Vertex2d item in vl)
            {
               item.Id = id;
               id++;
               vrtxs.Add(item);
            }
         }

         if (vrtxs[0].IsMatch(vrtxs[vrtxs.Count - 1]))
         {
            vrtxs.RemoveAt(vrtxs.Count - 1);
            vrtxs[vrtxs.Count - 1].Pos = VertexPosition.Last;
            vrtxs[vrtxs.Count - 1].Next = vrtxs[0];
            vrtxs[0].Prev = vrtxs[vrtxs.Count - 1];
            IsClosed = true;
         }

         CalcBB();
      }

      public void Close()
      {
         if (vrtxs.Count < 2) { IsClosed = false; return; }

         if (vrtxs[0].IsMatch(vrtxs[vrtxs.Count - 1]) && !IsClosed)
         {
            vrtxs.RemoveAt(vrtxs.Count - 1);
            vrtxs[vrtxs.Count - 1].Pos = VertexPosition.Last;
            vrtxs[vrtxs.Count - 1].Next = vrtxs[0];
            vrtxs[0].Prev = vrtxs[vrtxs.Count - 1];
            IsClosed = true;
         }
         else if (!IsClosed)
         {
            vrtxs[vrtxs.Count - 1].Next = vrtxs[0];
            vrtxs[0].Prev = vrtxs[vrtxs.Count - 1];
            IsClosed = true;
         }
      }

      public void Open()
      {
         if (vrtxs.Count < 2) { IsClosed = false; return; }

         if (IsClosed)
         {
            vrtxs.Add(new Vertex2d(vrtxs[0]));
            CalcVertices();
            IsClosed = false;
         }
      }

      protected void CalcBB()
      {
         if (vrtxs.Count == 0) return;

         IOrderedEnumerable<Vertex2d> selectedX = from v in vrtxs orderby v.X select v;
         IOrderedEnumerable<Vertex2d> selectedY = from v in vrtxs orderby v.Y select v;

         double minX = selectedX.First().X;
         double minY = selectedY.First().Y;
         double maxX = selectedX.Last().X;
         double maxY = selectedY.Last().Y;

         bb = new BoundingBox2d(new Point2d(minX, minY), new Point2d(maxX, maxY));
      }

      /// <summary>
      /// Задание значение выпуклости в вершине.
      /// </summary>
      /// <param name="idx">Индекс вершины, в которой задается выпуклость.</param>
      /// <param name="bulge">Значение величины выпуклости.</param>
      /// <remarks>
      /// При задании величины выпуклости <0 - принимается направление обхода по дуге по часовой стрелке от начальной точки сегмента к конечной.
      /// Величина выпуклости численно равна тангенсу 1/4 угла дуги.
      /// </remarks>
      public void SetBulgeAt(int idx, double bulge)
      {
         vrtxs[idx].Bulge = bulge;
      }

      /// <summary>
      /// Получения количества сегментов полилиниию
      /// </summary>
      public int GetSegsCount()
      {
         if (IsClosed) return vrtxs.Count;
         else return vrtxs.Count - 1;
      }

      /// <summary>
      /// Получения сегмента полилинии по начальной вершине.
      /// </summary>
      /// <param name="sPt">Начальная вершина сегмента.</param>
      public ICurve2d GetSegment(Vertex2d sPt)
      {
         int idx = vrtxs.BinarySearch(sPt);
         if (vrtxs[idx].Pos == VertexPosition.Last && !IsClosed) return null;
         if (vrtxs[idx].Bulge == 0) return new Line2d(vrtxs[idx], vrtxs[idx].Next) { Id = idx + 1 };
         else return new Arc2d(vrtxs[idx], vrtxs[idx].Next, vrtxs[idx].Bulge) { Id = idx + 1 };
      }

      /// <summary>
      /// Получения сегмента полилинии по индексу.
      /// </summary>
      /// <param name="idx">Индекс сегмента.</param>
      public ICurve2d GetSegment(int idx)
      {
         int count = GetSegsCount();
         Range range = new Range(0, count);
         if (!range.Affiliation(idx)) return null;
         if (vrtxs[idx].Bulge == 0) return new Line2d(vrtxs[idx], vrtxs[idx].Next) { Id = idx + 1 };
         else return new Arc2d(vrtxs[idx], vrtxs[idx].Next, vrtxs[idx].Bulge) { Id = idx + 1 };
      }


      /// <summary>
      /// Деление полилинии на сегменты по заданному шагу.
      /// </summary>
      /// <param name="step">Шаг деления.</param>
      /// <param name="stepType">Тип значения шага деления (относительное или абсолютное).</param>
      /// <param name="start">Флаг, указывающий на включение начальной точки отрезка в результат деления.</param>
      /// <param name="end">Флаг, указывающий на включение конечной точки отрезка в результат деления.</param>
      /// <remarks>
      /// В качестве шага деления с абсолютным значением следует задавать значение части длины отрезка.
      /// </remarks>
      /// <returns>Возврашает плоскую полилинию с вершинами в точках деления и линейными сегментами.</returns>
      public Pline2d TesselationByStep(double step, ParamType stepType = ParamType.abs, bool start = true, bool end = true)
      {
         Pline2d res = new Pline2d();
         int count = GetSegsCount();
         for (int i = 0; i < count; i++)
         {
            ICurve2d seg = GetSegment(i);
            res.AddVertices(seg.TesselationByStep(step, stepType, start, end));
         }

         return res;
      }

      /// <summary>
      /// Деление отрезка на равные участки по заданному количеству участков.
      /// </summary>
      /// <param name="nDiv">Количество участков деления.</param>
      /// <param name="start">Флаг, указывающий на включение начальной точки отрезка в результат деления.</param>
      /// <param name="end">Флаг, указывающий на включение конечной точки отрезка в результат деления.</param>
      /// <returns>Возврашает плоскую полилинию с вершинами в точках деления и линейными сегментами.</returns>
      public Pline2d TesselationByNumber(int nDiv, bool start = true, bool end = true)
      {
         Pline2d res = new Pline2d();
         int count = GetSegsCount();
         for (int i = 0; i < count; i++)
         {
            res.AddVertices(GetSegment(i).TesselationByNumber(nDiv, start, end));
         }

         return res;
      }

      //protected void CalcSegs()
      //{
      //   if (vrtxs.Count > 1)
      //   {
      //      segs = new List<ICurve2d>();

      //      for (int i = 1; i < vertxs.Count; i++)
      //      {
      //         if (vertxs[i - 1].Bulge == 0) segs.Add(new Line2d(vertxs[i - 1], vertxs[i]));
      //         else segs.Add(new Arc2d(vertxs[i - 1].ToPoint3d(), vertxs[i].ToPoint3d(), vertxs[i - 1].Bulge));
      //      }

      //      if (vertxs.Count > 2) { segs.Add(new Line2d(vertxs[vertxs.Count - 1], vertxs[0])); }
      //   }
      //   CalcVertices();
      //}

      void CalcVertices()
      {
         if (vrtxs.Count < 2) return;

         for (int i = 1; i < vrtxs.Count - 1; i++)
         {
            vrtxs[i].Id = i + 1;
            vrtxs[i].Prev = vrtxs[i - 1];
            vrtxs[i].Next = vrtxs[i + 1];
         }
         vrtxs[0].Id = 1;
         vrtxs[0].Pos = VertexPosition.First;
         vrtxs[0].Next = vrtxs[1];
         vrtxs[0].Prev = null;
         vrtxs[vrtxs.Count - 1].Id = vrtxs.Count;
         vrtxs[vrtxs.Count - 1].Pos = VertexPosition.Last;
         vrtxs[vrtxs.Count - 1].Prev = vrtxs[vrtxs.Count - 2];
         vrtxs[vrtxs.Count - 1].Next = null;
      }

      /// <summary>
      ///Смена направления полилинии.
      /// </summary>
      public void Inverse()
      {
         vrtxs.Reverse();
         CalcVertices();
      }

   }
}
