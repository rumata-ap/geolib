﻿namespace Geo
{
   public interface ICurve2d
   {
      int Id { get; set; }
      object Parent { get; set; }
      Point3d EndPoint { get; set; }
      Point3d StartPoint { get; set; }
      CurveType Type { get; }
      double Length { get; }

      Pline2d TesselationByNumber(int nDiv, bool start = true, bool end = true);
      Pline2d TesselationByStep(double step, ParamType stepType = ParamType.rel, bool start = true, bool end = true);
   }

   public enum CurveType
   {
      line,
      arc,
      circle
   }
}