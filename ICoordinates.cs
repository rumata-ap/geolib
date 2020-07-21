using Geo.Calc;

namespace Geo
{
   public interface ICoordinates
   {
      double X { get; set; }
      double Y { get; set; }
      double Z { get; set; }

      bool IsEqual(ICoordinates pt);
      bool NotEqual(ICoordinates pt);
      double[] ToArray();
      Vector3d ToVector3d();
   }
}