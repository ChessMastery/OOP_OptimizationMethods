using CommonInterfaces;
using Functions;
using System.Numerics;

namespace FunctionsImplementation
{
    public class Vector : List<double>, IVector
    {
    }
   class LineFunction : IParametricFunction
   {
      class InternalLineFunction : IFunction, IDifferentiableFunction, IParametricFunction
      {
         public IVector coefficients;
         public double Value(IVector point)
         {
            double sum = 0;
            for (int i = 0; i < point.Count; i++)
            {
               sum += point[i] * coefficients[i + 1];
            }
            sum += coefficients[0];
            return sum;
         }
         public IVector GradientOnVariables(IVector point)
         {
            IVector result = coefficients;
            result.RemoveAt(0);
            return result;
         }
         public IVector Gradient(IVector point)
         {
            var result = new Vector();
            result.Add(1.0);
            for (int i = 0; i < point.Count; i++)
               result.Add(point[i]);
            return result;
         }

         public IFunction Bind(IVector parameters) => new InternalLineFunction() { coefficients = parameters };
      }
      public IFunction Bind(IVector parameters) => new InternalLineFunction() { coefficients = parameters };
   }

   class Polynomial : IParametricFunction
   {
      class InternalPolynomialFunction : IFunction
      {
         public IVector coefficients;

         public double Value(IVector point)
         {
            double sum = 0;
            for (int i = 0; i < coefficients.Count; i++)
            {
               sum += Math.Pow(point[0], i) * coefficients[i];
            }
            return sum;
         }

      }
      public IFunction Bind(IVector parameters) => new InternalPolynomialFunction() { coefficients = parameters };
   }

   class PiecewiseLinear1D : IParametricFunction
   {
      class InternalPiecewiseLinear1D : IFunction, IDifferentiableFunction
      {
         public IVector coefficients;

         public double Value(IVector point)
         {
            int breakpointsnum = (coefficients.Count() - 2) / 3;
            int index = -1;
            for (int i = 0; i < breakpointsnum; i++)
            {
               if (coefficients[i] > point[0])
               {
                  index = i;
                  break;
               }
            }
            if (index == -1)
            {
               index = breakpointsnum;
            }
            return coefficients[breakpointsnum + index * 2] * point[0] + coefficients[breakpointsnum + index * 2 + 1];
         }
         public IVector GradientOnVariables(IVector point)
         {
            int breakpointsnum = (coefficients.Count() - 2) / 3;
            int index = -1;
            for (int i = 0; i < breakpointsnum; i++)
            {
               if (coefficients[i] > point[0])
               {
                  index = i;
                  break;
               }
            }
            if (index == -1)
            {
               index = breakpointsnum;
            }
            var result = new Vector();
            result.Add(coefficients[breakpointsnum + index * 2]);
            return result;
         }

         public IVector Gradient(IVector point)
         {
            throw new NotImplementedException("PiecewiseLinear1D.Gradient is not defined in joint points, thus not implemented.");
         }
      }
      public IFunction Bind(IVector parameters) => new InternalPiecewiseLinear1D() { coefficients = parameters };
   }
}