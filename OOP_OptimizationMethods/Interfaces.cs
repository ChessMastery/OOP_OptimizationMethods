using CommonInterfaces;
using Functions;
using Functionals;

namespace CommonInterfaces
{
   interface IVector : IList<double> { }
   interface IMatrix : IList<IList<double>> { }
}

namespace Functions
{
   interface IParametricFunction
   {
      IFunction Bind(IVector parameters);
   }

   interface IFunction
   {
      double Value(IVector point);
   }

   interface IDifferentiableFunction : IFunction
   {
      // По параметрам исходной IParametricFunction
      IVector Gradient(IVector point);
   }
}

namespace Functionals
{
   interface IFunctional
   {
      double Value(IFunction function);
   }

   interface IDifferentiableFunctional : IFunctional
   {
      IVector Gradient(IFunction function);
   }

   interface ILeastSquaresFunctional : IFunctional
   {
      IVector Residual(IFunction function);
      IMatrix Jacobian(IFunction function);
   }
}

namespace Optimizators
{
   interface IOptimizator
   {
      IVector Minimize(IFunctional objective,
                       IParametricFunction function,
                       IVector initialParameters,
                       IVector minimumParameters = default,
                       IVector maximumParameters = default);
   }
}