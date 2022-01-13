using HSOP.Helpers;
using HSOP.Models;
using System;
using System.Collections.Generic;

namespace HSOP.Algorithms
{
    class RandomSolution
    {
        public static Result solve(Instance instance, Random random)
        {
            List<int> route = new List<int>();
            route.Add(instance.noOfFirstVertex);
            double leftDistance = instance.maxDistance;
            double gatheredPoints = instance.points[instance.noOfFirstVertex - 1];
            List<int> verticesToVisit = Helper.generateVerticesWithoutFirstAndLast(instance);
            List<int> availableVertices = Helper.generateAvailableVertices(instance, leftDistance, route, verticesToVisit);
            while (availableVertices.Count > 0)
            {
                route.Add(availableVertices[random.Next(availableVertices.Count)]);
                leftDistance -= instance.distanceMatrix[route[route.Count - 2] - 1, route[route.Count - 1] - 1];
                gatheredPoints += instance.points[route[route.Count - 1] - 1];
                verticesToVisit.Remove(route[route.Count - 1]);
                availableVertices = Helper.generateAvailableVertices(instance, leftDistance, route, verticesToVisit);
            }
            route.Add(instance.noOfLastVertex);
            gatheredPoints += instance.points[instance.noOfLastVertex - 1];
            return new Result(route.ToArray(), gatheredPoints);
        }
    }
}
