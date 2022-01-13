using HSOP.Helpers;
using HSOP.Models;

namespace HSOP.Algorithms
{
    class _2opt
    {
        public static Result solve(double[,] distanceMatrix, double[] points, Result baseSolution)
        {
            int[] previousMatrix = baseSolution.getRoute();
            double beginningDistance = Helper.calculateRouteLength(previousMatrix, distanceMatrix);
            double receivedDistance;
            bool changed;
            do
            {
                changed = false;
                for (int i = 1; i < previousMatrix.Length - 2; i++)
                {
                    for (int j = i + 1; j < previousMatrix.Length - 1; j++)
                    {
                        int[] generatePreviousMatrix = changePoints(previousMatrix, i, j);
                        receivedDistance = Helper.calculateRouteLength(generatePreviousMatrix, distanceMatrix);
                        if (receivedDistance < beginningDistance)
                        {
                            previousMatrix = generatePreviousMatrix;
                            beginningDistance = receivedDistance;
                            changed = true;
                            break;
                        }
                    }
                    if (changed)
                        break;
                }
            } while (changed);
            return new Result(previousMatrix, Helper.calculatePoints(previousMatrix, points));
        }

        private static int[] changePoints(int[] route, int firstPointIndex, int secondPointIndex)
        {
            int[] routeTmp = new int[route.Length];
            for (int i = 0; i < firstPointIndex; i++)
                routeTmp[i] = route[i];
            for (int i = firstPointIndex; i <= secondPointIndex; i++)
                routeTmp[i] = route[firstPointIndex + secondPointIndex - i];
            for (int i = secondPointIndex + 1; i < route.Length; i++)
                routeTmp[i] = route[i];
            return routeTmp;
        }
    }
}
