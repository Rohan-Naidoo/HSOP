using HSOP.Helpers;
using HSOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSOP.Algorithms
{
    class Improve
    {
        public static Result changeVertex(Instance instance, int[] route, List<int> univistedVertices, int numberOfPointsToRemove)
        {
            List<int> routeList = route.ToList();
            for (int i = 0; i < numberOfPointsToRemove; i++)
            {
                int indexOfNodeInRoute = calculateIndexWithMaximalPointsIncreaseByDistance(route.ToArray(), instance);
                univistedVertices.Add(route[indexOfNodeInRoute]);
                routeList.Remove(route[indexOfNodeInRoute]);
            }

            double leftDistance = instance.maxDistance - Helper.calculateRouteLength(routeList.ToArray(), instance.distanceMatrix);

            if (leftDistance < 0)
                return new Result(route, Helper.calculatePoints(route, instance.points));

            routeList = addPointWithMaximalPointsIncreaseByDistance(routeList, instance, leftDistance, ref univistedVertices);
            route = routeList.ToArray();
            return new Result(route, Helper.calculatePoints(route, instance.points));
        }

        private static int calculateIndexWithMaximalPointsIncreaseByDistance(int[] route, Instance instance)
        {
            int index = 1;
            double minimalRatio = double.MaxValue;
            for (int i = 1; i < route.Length - 1; i++)
            {
                double pointForDistance = calculateNumberOfPointsPerDistanceUnit(instance.distanceMatrix, instance.points, route[i - 1], route[i], route[i + 1]);
                if (pointForDistance < 0)
                    continue;
                if (pointForDistance < minimalRatio)
                {
                    index = i;
                    minimalRatio = pointForDistance;
                }
            }
            return index;
        }

        private static double calculateNumberOfPointsPerDistanceUnit(double[,] distanceMatrix, double[] points, int noOfPrevious, int noOfMiddle, int noOfSuccessor)
        {
            return points[noOfMiddle - 1] / generateDistanceProfit(distanceMatrix, noOfPrevious, noOfMiddle, noOfSuccessor);
        }

        private static double generateDistanceProfit(double[,] distanceMatrix, int noOfPrevious, int noOfMiddle, int noOfSuccessor)
        {
            double savedDistance = distanceMatrix[noOfPrevious - 1, noOfSuccessor - 1];
            double distanceToAdd = distanceMatrix[noOfPrevious - 1, noOfMiddle - 1] + distanceMatrix[noOfMiddle - 1, noOfSuccessor - 1];
            return distanceToAdd - savedDistance;
        }
        private static List<int> addPointWithMaximalPointsIncreaseByDistance(List<int> route, Instance instance, double leftDistance, ref List<int> unvisitedVertices)
        {
            do
            {

            } while (addedPoint(ref route, instance, ref leftDistance, ref unvisitedVertices));
            return route;
        }

        private static bool addedPoint(ref List<int> route, Instance instance, ref double leftDistance, ref List<int> unvisitedVertices)
        {
            int indexToAddAfter = -1;
            int vertexToAdd = -1;
            double minimalRatio = double.MinValue;
            for (int j = 0; j < route.Count - 1; j++)
            {
                List<int> availableVertices = generateAvailableVertices(instance.distanceMatrix, leftDistance, route[j] - 1, route[j + 1] - 1, unvisitedVertices);
                for (int i = 0; i < availableVertices.Count; i++)
                {
                    double pointForDistance = calculateNumberOfPointsPerDistanceUnit(instance.distanceMatrix, instance.points, route[j], availableVertices[i], route[j + 1]);
                    if (pointForDistance < 0)
                        continue;
                    if (pointForDistance > minimalRatio)
                    {
                        vertexToAdd = availableVertices[i];
                        indexToAddAfter = j;
                        minimalRatio = pointForDistance;
                    }
                }
            }
            if (indexToAddAfter == -1)
                return false;

            leftDistance -= generateDistanceProfit(instance.distanceMatrix, route[indexToAddAfter], vertexToAdd, route[indexToAddAfter + 1]);
            route.Insert(indexToAddAfter + 1, vertexToAdd);
            unvisitedVertices.Remove(vertexToAdd);
            return true;
        }

        private static List<int> generateAvailableVertices(double[,] distanceMatrix, double leftDistance, int indexOfPreviousVertex, int indexOfSuccessor, List<int> unvisitedVertices)
        {
            List<int> availableVertices = new List<int>();
            double savedDistance = distanceMatrix[indexOfPreviousVertex, indexOfSuccessor];
            leftDistance += savedDistance;
            for (int i = 0; i < unvisitedVertices.Count; i++)
                if (leftDistance >= distanceMatrix[indexOfPreviousVertex, unvisitedVertices[i] - 1] + distanceMatrix[unvisitedVertices[i] - 1, indexOfSuccessor])
                    availableVertices.Add(unvisitedVertices[i]);
            return availableVertices;
        }
    }
}
