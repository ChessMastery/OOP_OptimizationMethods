using CommonInterfaces;
using Functions;
using Functionals;

namespace FunctionalsImplementation
{
    public class Vector : List<double>, IVector { }
    public class Matrix : List<IList<double>>, IMatrix { }

    class DifferenceNormL1 : IDifferentiableFunctional
    {
        public List<(IVector x, double y)> points;
        public IVector Gradient(IFunction function)
        {
            if (function is IParametricFunction)
            {
                if (function is IDifferentiableFunction)
                {
                    var functional_grad = ((IDifferentiableFunction)function).Gradient(points[0].x);
                    for (int i = 0; i < points[0].x.Count; i++)
                        functional_grad[i] = 0.0;

                    foreach (var point in points)
                    {
                        var grad = ((IDifferentiableFunction)function).Gradient(point.x);
                        if (point.y - function.Value(point.x) > 0)
                            for (int i = 0; i < functional_grad.Count; i++)
                                functional_grad[i] += grad[i];
                        else
                            for (int i = 0; i < functional_grad.Count; i++)
                                functional_grad[i] -= grad[i];
                    }
                    return functional_grad;
                }
                else
                {
                    throw new NotImplementedException("Function is not differentiable via parameters.");
                }
            }
            else
            {
                throw new NotImplementedException("Non-parametric function given to be differentiated via parameters.");
            }
        }

        public double Value(IFunction function)
        {
            double res = 0.0;
            foreach (var point in points)
            {
                // NOTE: Убрал, как по мне, лишнюю локальную переменную vec

                res += Math.Abs(function.Value(point.x) - point.y);
            }
            return res;
        }
    }

    class DifferenceNormL2 : IDifferentiableFunctional, ILeastSquaresFunctional
    {
        public List<(IVector x, double y)> points;
        // let us consider params number = residual functions number
        public double Value(IFunction function)
        {
            double res = 0.0;
            foreach (var point in points)
            {
                // NOTE: Убрал, как по мне, лишнюю локальную переменную vec

                res += (function.Value(point.x) - point.y) * (function.Value(point.x) - point.y);
            }
            return res;
        }
        public IVector Gradient(IFunction function)
        {
            if (function is IParametricFunction)
            {
                if (function is IDifferentiableFunction)
                {
                    var functional_grad = ((IDifferentiableFunction)function).Gradient(points[0].x);
                    for (int i = 0; i < points.Count; i++)
                        functional_grad[i] = 0.0;

                    foreach (var point in points)
                    {
                        double val = function.Value(point.x);
                        var grad = ((IDifferentiableFunction)function).Gradient(point.x);
                        for (int i = 0; i < functional_grad.Count; i++)
                            functional_grad[i] += 2.0 * grad[i] * (val - point.y);
                    }
                    return functional_grad;
                }
                else
                {
                    throw new NotImplementedException("Function is not differentiable via parameters.");
                }
            }
            else
            {
                throw new NotImplementedException("Non-parametric function given to be differentiated via parameters.");
            }
        }
        public IMatrix Jacobian(IFunction function)
        {
            // NOTE: Добавил проверку на то, реализует ли function IDifferentiableFunction

            if (!(function is IDifferentiableFunction))
            {
                throw new NotImplementedException("Function is not differentiable via parameters.");
            }

            var jacobi = new Matrix();
            for (int i = 0; i < points.Count; i++)
            {
                var vec = ((IDifferentiableFunction)function).Gradient(points[i].x);
                jacobi.Add(vec);
            }
            return jacobi;
        }
        public IVector Residual(IFunction function)
        {
            var vec = new Vector();
            for (int i = 0; i < points.Count; i++)
                vec.Add(points[i].y - function.Value(points[i].x));
            return vec;
        }
    }

    class DifferenceNormInf : IFunctional
    {
        // NOTE: Поменял тип points с List<double, double> на List<IVector, double> 

        public List<(IVector x, double y)> points;
        public double Value(IFunction function)
        {
            double res = 0.0;
            foreach (var point in points)
            {
                res = Math.Max(res, Math.Abs(function.Value(point.x) - point.y));
            }
            return res;
        }
    }

    // NOTE: Поменял метод интегрирования. Считается повторный интеграл по прямоугольнику

    class IntegralRectDomain : IFunctional
    {
        double X0 { get; set; }
        double X1 { get; set; }

        double Y0 { get; set; }
        double Y1 { get; set; }
        double H {  get; set; }
        double K { get; set; }

        public IntegralRectDomain(double x0, double x1, double y0, double y1, double h, double k)
        {
            X0 = x0;
            X1 = x1;
            Y0 = y0;
            Y1 = y1;
            H = h;
            K = k;
        }

        public double Value(IFunction function)
        {
            int nx, ny;

            double[,] z = new double[50, 50];
            double[] ax = new double[50];
            double answer;

            // Calculating the number of points
            // in x and y integral
            nx = (int)((X1 - X0) / H + 1);
            ny = (int)((Y1 - Y0) / H + 1);

            // Calculating the values of the table
            for (int i = 0; i < nx; ++i)
            {
                for (int j = 0; j < ny; ++j)
                {
                    IVector vec = new Vector() { X0 + i * H, Y0 + j * K };
                    z[i, j] = function.Value(vec);
                }
            }

            for (int i = 0; i < nx; ++i)
            {
                ax[i] = 0;
                for (int j = 0; j < ny; ++j)
                {
                    if (j == 0 || j == ny - 1)
                        ax[i] += z[i, j];
                    else if (j % 2 == 0)
                        ax[i] += 2 * z[i, j];
                    else
                        ax[i] += 4 * z[i, j];
                }
                ax[i] *= (K / 3);
            }

            answer = 0;

            for (int i = 0; i < nx; ++i)
            {
                if (i == 0 || i == nx - 1)
                    answer += ax[i];
                else if (i % 2 == 0)
                    answer += 2 * ax[i];
                else
                    answer += 4 * ax[i];
            }
            answer *= (H / 3);

            return answer;
        }
    }
}