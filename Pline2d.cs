using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Geo.Calc;

namespace Geo
{
   public class Pline2d
   {
      protected List<Vertex2d> vrtxs;
      protected BoundingBox2d bb;

      public List<Vertex2d> Vertices { get => vrtxs; internal set { vrtxs = value; CalcBB(); CalcVertices(); } }
      public BoundingBox2d BoundingBox { get => bb; }
      public bool IsClosed { get; private set; }

      public Pline2d()
      {
         vrtxs = new List<Vertex2d>();
      }

      public Pline2d(IEnumerable<IXYZ> vertices)
      {
         vrtxs = new List<Vertex2d>(vertices.Count());
         if (vertices.Count() > 0)
         {
            foreach (IXYZ item in vertices)
            {
               vrtxs.Add(new Vertex2d(item));
            }
         }
         CalcBB();
         CalcVertices();
      }

      public Pline2d(List<Vertex2d> vertices)
      {
         vrtxs = new List<Vertex2d>(vertices.Count());
         if (vertices.Count() > 0)
         {
            foreach (Vertex2d item in vertices)
            {
               vrtxs.Add(Vertex2d.Copy(item));
            }
         }
         CalcBB();
         CalcVertices();
      }

      protected void AddVertexNew(IXYZ pt)
      {
         Open();
         vrtxs.Add(new Vertex2d(pt));
         vrtxs[vrtxs.Count - 1].Pos = VertexPosition.Last;
         vrtxs[vrtxs.Count - 1].Id = vrtxs.Count;
         vrtxs[vrtxs.Count - 1].Prev = vrtxs[vrtxs.Count - 2];
         vrtxs[vrtxs.Count - 1].Next = null;
         CalcBB();
      }
      
      protected void AddVertexNew(Vertex2d pt)
      {
         Open();
         vrtxs.Add(Vertex2d.Copy(pt));
         vrtxs[vrtxs.Count - 1].Pos = VertexPosition.Last;
         vrtxs[vrtxs.Count - 1].Id = vrtxs.Count;
         vrtxs[vrtxs.Count - 1].Prev = vrtxs[vrtxs.Count - 2];
         vrtxs[vrtxs.Count - 1].Next = null;
         CalcBB();
      }

      public void AddVertex(Vertex2d pt)
      {
         if (vrtxs.Count == 0)
         {
            vrtxs.Add(pt);
            vrtxs[vrtxs.Count - 1].Pos = VertexPosition.First;
         }
         else if (vrtxs.Count < 3)
         {
            vrtxs.Add(pt);
            vrtxs[vrtxs.Count - 1].Pos = VertexPosition.Middle;
            vrtxs[vrtxs.Count - 1].Id = vrtxs.Count;
            vrtxs[vrtxs.Count - 1].Prev = vrtxs[vrtxs.Count - 2];
            vrtxs[vrtxs.Count - 2].Next = vrtxs[vrtxs.Count - 1];
            vrtxs[vrtxs.Count - 1].Next = null;
         }
         else
         {
            Open();
            vrtxs.Add(pt);
            vrtxs[vrtxs.Count - 1].Pos = VertexPosition.Last;
            vrtxs[vrtxs.Count - 1].Id = vrtxs.Count;
            vrtxs[vrtxs.Count - 1].Prev = vrtxs[vrtxs.Count - 2];
            vrtxs[vrtxs.Count - 2].Next = vrtxs[vrtxs.Count - 1];
            vrtxs[vrtxs.Count - 1].Next = null;
         }
         CalcBB();
      }

      public void AddPointsNew(IEnumerable<IXYZ> vertices, bool recalc = true)
      {
         Open();
         List<IXYZ> tmp = new List<IXYZ>(vertices);
         foreach (Vertex2d item in tmp)
         {
            vrtxs.Add(new Vertex2d(item));
         }
         CalcBB();
         if (recalc) CalcVertices();
      }

      public void AddVerticesNew(IEnumerable<Vertex2d> vertices, bool recalc = true)
      {
         Open();
         List<Vertex2d> tmp = new List<Vertex2d>(vertices);
         foreach (Vertex2d item in tmp) vrtxs.Add(item);
         CalcBB();
         if (recalc) CalcVertices();
      }

      public void AddVerticesNew(List<Vertex2d> vertices, bool recalc = true)
      {
         Open();
         foreach (Vertex2d item in vertices)
         {
            vrtxs.Add(Vertex2d.Copy(item));
         }
         CalcBB();
         if (recalc) CalcVertices();
      }

      public void CopySelfVertices()
      {
         List<Vertex2d> tmp = new List<Vertex2d>(vrtxs);
         vrtxs = new List<Vertex2d>(tmp.Count);
         foreach (Vertex2d item in tmp)
         {
            vrtxs.Add(new Vertex2d(item) { Nref = item.Nref });
         }
      }

      public void ReplaceVertices(IEnumerable<Vertex2d> vertices, bool recalc = true)
      {
         Open();
         List<Vertex2d> tmp = new List<Vertex2d>(vertices);
         vrtxs = new List<Vertex2d>(tmp.Count);
         foreach (Vertex2d item in tmp)
         {
            vrtxs.Add(Vertex2d.Copy(item));
         }
         CalcBB();
         if(recalc) CalcVertices();
      }

      public virtual Pline2d Copy()
      {
         Pline2d res = new Pline2d();
         res.AddVerticesNew(vrtxs.ToArray());

         return res;
      }

      public virtual void RemoveVertex(Vertex2d vertex, bool recalc = true)
      {
         vrtxs.Remove(vertex);
         CalcBB();
         if (recalc) CalcVertices();
      }
      
      public virtual void InsertVertex(int id, Vertex2d vertex, bool recalc = true)
      {
         vrtxs.Insert(id, vertex);
         CalcBB();
         if (recalc) CalcVertices();
      }

      public void AddVertices(IEnumerable<Vertex2d> vertices, bool recalc = true)
      {
         Open();
         if (vrtxs.Count > 0 && vrtxs.Last().IsMatch(vertices.First())) vertices.ToList().RemoveAt(0);
         vrtxs.AddRange(new List<Vertex2d>(vertices));
         CalcBB();
         if (recalc) CalcVertices();
      }

      public void AddVertices(Pline2d pline, bool recalc = true)
      {
         Open();
         if (pline == null || pline.Vertices.Count == 0) return;
         if (vrtxs.Count>0 && vrtxs.Last().IsMatch(pline.Vertices[0])) pline.Vertices.RemoveAt(0);
         vrtxs.AddRange(pline.Vertices);
         CalcBB();
         if (recalc) CalcVertices();
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
         if (!IsClosed) return;
         else if (vrtxs.Count < 2) { IsClosed = false; return; }
         else if (IsClosed)
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
         if (!range.In(idx)) return null;
         if (vrtxs[idx].Bulge == 0) return new Line2d(vrtxs[idx], vrtxs[idx].Next) { Id = idx + 1 };
         else return new Arc2d(vrtxs[idx], vrtxs[idx].Next, vrtxs[idx].Bulge) { Id = idx + 1 };
      }
      

      public List<ICurve2d> GetAllSegments()
      {
         int count = GetSegsCount();
         List<ICurve2d> res = new List<ICurve2d>(count);
         if (count>=1)
         {
            for (int i = 0; i < count; i++)
            {
               res.Add(new Line2d(vrtxs[i], vrtxs[i].Next) { Id = i + 1 });
            }
         }
         return res;
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
      public virtual Pline2d TesselationByStep(double step, ParamType stepType = ParamType.abs)
      {
         Pline2d res = new Pline2d();
         int count = GetSegsCount();
         for (int i = 0; i < count; i++)
         {
            ICurve2d seg = GetSegment(i);
            res.AddVertices(seg.TesselationByStep(step, stepType, true, true));
         }
         res.CalcBB();
         return res;
      }

      /// <summary>
      /// Деление отрезка на равные участки по заданному количеству участков.
      /// </summary>
      /// <param name="nDiv">Количество участков деления.</param>
      /// <param name="start">Флаг, указывающий на включение начальной точки отрезка в результат деления.</param>
      /// <param name="end">Флаг, указывающий на включение конечной точки отрезка в результат деления.</param>
      /// <returns>Возврашает плоскую полилинию с вершинами в точках деления и линейными сегментами.</returns>
      public virtual Pline2d TesselationByNumber(int nDiv)
      {
         Pline2d res = new Pline2d();
         int count = GetSegsCount();
         for (int i = 0; i < count; i++)
         {
            res.AddVertices(GetSegment(i).TesselationByNumber(nDiv, true, true));
         }
         res.CalcBB();
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

      protected void CalcVertices()
      {
         if (vrtxs.Count < 2) return;

         for (int i = 1; i < vrtxs.Count - 1; i++)
         {
            vrtxs[i].Id = i + 1;
            vrtxs[i].Prev = vrtxs[i - 1];
            vrtxs[i].Next = vrtxs[i + 1];
            vrtxs[i].Pos = VertexPosition.Middle;
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
      public virtual void Inverse()
      {
         bool tmpClosed = IsClosed;
         Open();
         vrtxs.Reverse();
         for (int i = 1; i < vrtxs.Count; i++) vrtxs[i - 1].Bulge = -vrtxs[i].Bulge;         
         CalcVertices();
         if (tmpClosed) Close();
      }
   }
}
