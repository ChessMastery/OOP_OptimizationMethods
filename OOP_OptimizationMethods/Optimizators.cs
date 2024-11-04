using CommonInterfaces;
using Functions;
using Functionals;
using Optimizators;
using System.Numerics;
using System.ComponentModel.Design;

namespace OptimizatorsImplementation
{
   public class Vector : List<double>, IVector
   {
   }

   static class Solver
   {
      static public void SlaeSolveGauss(IMatrix a, IVector x, IVector y)
      {
         double eps = 1e-13;
         int n = y.Count;
         x.Clear();
         for (int i = 0; i < n; i++) x.Add(0.0);
         int k, index;
         double max;
         k = 0;
         while (k < n)
         {
            max = Math.Abs(a[k][k]);
            index = k;
            for (int i = k + 1; i < n; i++)
            {
               if (Math.Abs(a[i][k]) > max)
               {
                  max = Math.Abs(a[i][k]);
                  index = i;
               }
            }
            if (max < eps)
            {
               Console.WriteLine("Gaussian elimination with row pivoting (columns swapping) error: could not find pivot >= eps from column ");
               Console.WriteLine(index.ToString() + " of the matrix.\n");
               throw new Exception("Solver.SlaeSolveGauss method error.");
            }
            for (int j = 0; j < n; j++)
            {
               double tmp = a[k][j];
               a[k][j] = a[index][j];
               a[index][j] = tmp;
            }
            double temp = y[k];
            y[k] = y[index];
            y[index] = temp;

            for (int i = k; i < n; i++)
            {
               double tmp = a[i][k];
               if (Math.Abs(tmp) < eps) continue;
               for (int j = 0; j < n; j++)
                  a[i][j] = a[i][j] / tmp;
               y[i] = y[i] / tmp;
               if (i == k) continue;
               for (int j = 0; j < n; j++)
                  a[i][j] = a[i][j] - a[k][j];
               y[i] = y[i] - y[k];
            }
            k++;
         }
         for (k = n - 1; k >= 0; k--)
         {
            x[k] = y[k];
            for (int i = 0; i < k; i++)
               y[i] = y[i] - a[i][k] * x[k];
         }

      }
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
         if (!(objective is IDifferentiableFunctional))
            throw new NotImplementedException("MinimizerGradientDescent.Minimize parameter objective does not implement IDifferentiableFunctional");
         
         Console.WriteLine("MinimizerGradientDescent params in iteration " + 0.ToString() + " = ");
         for (int i = 0; i < initialParameters.Count(); i++)
            Console.WriteLine(initialParameters[i].ToString());
         Console.WriteLine("Functional = " + ((IDifferentiableFunctional)objective).Value((IFunction)function.Bind(initialParameters)).ToString());

         int it_max = 20;
         for (int it = 0; it < it_max; it++)
         {
            var delta2 = ((IDifferentiableFunctional)objective).Gradient((IDifferentiableFunction)(function.Bind(initialParameters)));
            for (int i = 0; i < initialParameters.Count; i++)
               initialParameters[i] -= delta2[i] * 0.01;

            Console.WriteLine("MinimizerGradientDescent params in iteration " + (it + 1).ToString() + " = ");
            for (int i = 0; i < initialParameters.Count(); i++)
               Console.WriteLine(initialParameters[i].ToString());
            Console.WriteLine("Functional = " + ((IDifferentiableFunctional)objective).Value((IFunction)function.Bind(initialParameters)).ToString());
         }
         return initialParameters;

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
         if (!(objective is ILeastSquaresFunctional))
            throw new NotImplementedException("MinimizerGaussNewton.Minimize parameter objective does not implement ILeastSquaresFunctional");
         if (!(function.Bind(initialParameters) is IDifferentiableFunction))
            throw new NotImplementedException("MinimizerGaussNewton.Minimize parameter function does not implement IDifferentiableFunction");

         Console.WriteLine("GaussNewton params in iteration " + 0.ToString() + " = ");
         for (int i = 0; i < initialParameters.Count(); i++)
            Console.WriteLine(initialParameters[i].ToString());
         Console.WriteLine("Functional = " + ((ILeastSquaresFunctional)objective).Value((IFunction)function.Bind(initialParameters)).ToString());
         Console.WriteLine("Residual = ");
         var it0prntRes = ((ILeastSquaresFunctional)objective).Residual((IFunction)function.Bind(initialParameters));
         for (int i = 0; i < it0prntRes.Count(); i++)
            Console.WriteLine(it0prntRes[i].ToString());

         int it_max = 5;
         for (int it = 0; it < it_max; it++)
         {
            var delta = new Vector();
            // delta = u[it+1] - u[it] = J^-1 * (y - f(x, params))
            Solver.SlaeSolveGauss(((ILeastSquaresFunctional)objective).Jacobian((IDifferentiableFunction)function.Bind(initialParameters)),
               delta, ((ILeastSquaresFunctional)objective).Residual((IFunction)function.Bind(initialParameters)));
            for (int i = 0; i < initialParameters.Count; i++)
               initialParameters[i] += delta[i];

            Console.WriteLine("GaussNewton params in iteration " + (it + 1).ToString() + " = ");
            for (int i = 0; i < initialParameters.Count(); i++)
               Console.WriteLine(initialParameters[i].ToString());
            Console.WriteLine("Functional = " + ((ILeastSquaresFunctional)objective).Value((IFunction)function.Bind(initialParameters)).ToString());
            Console.WriteLine("Residual = ");
            var itPrntRes = ((ILeastSquaresFunctional)objective).Residual((IFunction)function.Bind(initialParameters));
            for (int i = 0; i < itPrntRes.Count(); i++)
               Console.WriteLine(itPrntRes[i].ToString());
         }
         return initialParameters;
      }
   }
}