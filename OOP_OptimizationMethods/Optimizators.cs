using CommonInterfaces;
using Functions;
using Functionals;
using Optimizators;

namespace OptimizatorsImplementation
{
   public class Vector : List<double>, IVector
   {
   }

   class MinimizerMonteCarlo : IOptimizator
   {
      public int MaxIter = 100000;
      public IVector Minimize(IFunctional objective,
                              IParametricFunction function,
                              IVector initialParams,
                              IVector minParams = null,
                              IVector maxParams = null)
      {
         var param = new Vector();
         var minparam = new Vector();
         foreach (var p in initialParams) param.Add(p);
         foreach (var p in initialParams) minparam.Add(p);
         var fun = function.Bind(param);
         var currentmin = objective.Value(fun);
         var rand = new Random(0);
         for (int i = 0; i < MaxIter; i++)
         {
            for (int j = 0; j < param.Count; j++)
               param[j] = rand.NextDouble();
            if (minParams != null && maxParams != null)
               for (int j = 0; j < param.Count; j++)
                  param[j] = minParams[j] + (maxParams[j] - minParams[j]) * param[j];
            var f = objective.Value(function.Bind(param));
            if (f < currentmin)
            {
               currentmin = f;
               for (int j = 0; j < param.Count; j++) minparam[j] = param[j];
            }
         }
         return minparam;
      }
   }

   class MinimizerGradientDescent : IOptimizator
   {
      public IVector Minimize(IFunctional objective,
                              IParametricFunction function,
                              IVector initialParameters,
                              IVector minimumParameters = null,
                              IVector maximumParameters = null)
      {
         throw new NotImplementedException();
      }
   }

   class MinimizerGaussNewton : IOptimizator
   {
      public IVector Minimize(IFunctional objective,
                              IParametricFunction function,
                              IVector initialParameters,
                              IVector minimumParameters = null,
                              IVector maximumParameters = null)
      {
         throw new NotImplementedException();
      }
   }
}