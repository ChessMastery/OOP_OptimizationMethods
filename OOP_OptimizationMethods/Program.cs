using CommonInterfaces;

namespace OOP_OptimizationMethods
{
   public class Vector : List<double>, CommonInterfaces.IVector
   {
   }

   class Program
   {
      static void Main(string[] args)
      {
         Console.WriteLine("Hello, world.");


         var optimizer = new OptimizatorsImplementation.MinimizerGaussNewton();
         var initial = new Vector();
         initial.Add(1.0);
         initial.Add(1.0);
         initial.Add(1.0);
         int n = 2;
         List<(Vector x, double y)> points = new();
         var pts1 = new Vector();
         pts1.Add(1.5);
         pts1.Add(2.5);
         //pts1.Add(1.7);
         points.Add((pts1, 2.0));

         var pts2 = new Vector();
         pts2.Add(2.3);
         pts2.Add(3.3);
         //pts2[2] = 4.3;
         points.Add((pts2, 3.0));

         var pts3 = new Vector();
         pts3.Add(5.4);
         pts3.Add(3.1);
         //pts3[2] = 2.5;
         points.Add((pts3, 4.0));

         var functional = new FunctionalsImplementation.DifferenceNormL2();
         functional.points = new List<(IVector x, double y)>();
         for (int i = 0; i < points.Count; i++)
            functional.points.Add(points[i]);
         var fun = new FunctionsImplementation.LineFunction();

         var res = optimizer.Minimize(functional, fun, initial);
         Console.WriteLine($"res[0] = {res[0]}, res[1] = {res[1]}, res[2] = {res[2]}");



         var optimizer2 = new OptimizatorsImplementation.MinimizerGradientDescent();
         var initial2 = new Vector();
         initial2.Add(1.0);
         initial2.Add(1.0);
         initial2.Add(1.0);

         var functional2 = new FunctionalsImplementation.DifferenceNormL2();
         functional2.points = new List<(IVector x, double y)>();
         for (int i = 0; i < points.Count; i++)
            functional2.points.Add(points[i]);
         var fun2 = new FunctionsImplementation.LineFunction();


         var res2 = optimizer2.Minimize(functional2, fun2, initial2);
         Console.WriteLine($"res2[0] = {res2[0]}, res2[1] = {res2[1]}, res2[2] = {res2[2]}");



         var optimizer3 = new OptimizatorsImplementation.MinimizerMonteCarlo();
         var initial3 = new Vector();
         initial3.Add(1);
         initial3.Add(1);
         initial3.Add(1);
         var functional3 = new FunctionalsImplementation.DifferenceNormInf();
         functional3.points = new List<(IVector x, double y)>();
         for (int i = 0; i < points.Count; i++)
            functional3.points.Add(points[i]);
         var fun3 = new FunctionsImplementation.LineFunction();

         Console.WriteLine("Before minimization: Functional = " + functional3.Value(fun3.Bind(initial3)).ToString());
         var res3 = optimizer3.Minimize(functional3, fun3, initial);
         Console.WriteLine("MonteCarloMinimizer results:");
         Console.WriteLine("Functional = " + functional3.Value(fun3.Bind(res3)).ToString());
         Console.WriteLine($"res3[0] = {res3[0]}, res3[1] = {res3[1]}, res3[2] = {res3[2]}");
      }
   }
}