using CommonInterfaces;
using Functions;

namespace FunctionsImplementation
{
   class LineFunction : IParametricFunction
   {
      class InternalLineFunction : IFunction
      {
         public double a, b;
         public double Value(IVector point) => a * point[0] + b;
      }
      public IFunction Bind(IVector parameters) => new InternalLineFunction() { a = parameters[0], b = parameters[1] };
   }
}