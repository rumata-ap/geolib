
namespace Geo.Calc
{
   class Range
   {
      double s;
      double e;

      public Range(double start, double end)
      {
         s = start;
         e = end;
      }

      public bool Affiliation(double arg)
      {
         if (arg >= s && arg <= e) return true;
         else return false;
      }
   }
}
