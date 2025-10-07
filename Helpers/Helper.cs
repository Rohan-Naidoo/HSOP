using HSOP.Models;
using System;
using System.Collections.Generic;

namespace HSOP.Helpers
{
    class Helper
    {
        public static Coordinate calculateCenterOfGravity(Coordinate[] coodinates)
        {
            double sum = 0;
            for (int i = 0; i < coodinates.Length; i++)
                sum += coodinates[i].weight;
            double x = 0;
            for (int i = 0; i < coodinates.Length; i++)
                x += coodinates[i].x * coodinates[i].weight;
            x = x / sum;
            double y = 0;
            for (int i = 0; i < coodinates.Length; i++)
                y += coodinates[i].y * coodinates[i].weight;
            y = y / sum;
            return new Coordinate(x, y);
        }

        public static List<int> generateAvailableVertices(Instance instance, double leftDistance, List<int> route, List<int> unvisitedVerticesWithoutLast)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < unvisitedVerticesWithoutLast.Count; i++)
                if (leftDistance >= instance.distanceMatrix[route[route.Count - 1] - 1, unvisitedVerticesWithoutLast[i] - 1] + instance.distanceMatrix[unvisitedVerticesWithoutLast[i] - 1, instance.noOfLastVertex - 1])
                    list.Add(unvisitedVerticesWithoutLast[i]);
            return list;
        }

        public static double calculateRouteLength(int[] route, double[,] distanceMatrix)
        {
            double distance = 0;
            for (int i = 0; i < route.Length - 1; i++)
                distance += distanceMatrix[route[i] - 1, route[i + 1] - 1];
            return distance;
        }

        public static double calculatePoints(int[] route, double[] points)
        {
            double gathered = 0;
            for (int i = 0; i < route.Length; i++)
                gathered += points[route[i] - 1];
            return gathered;
        }

        public static List<int> generateVerticesWithoutFirstAndLast(Instance instance)
        {
            List<int> vertices = new List<int>();
            for (int i = 0; i < instance.points.Length; i++)
                vertices.Add(i + 1);
            vertices.Remove(instance.noOfFirstVertex);
            vertices.Remove(instance.noOfLastVertex);
            return vertices;
        }

        public static Coordinate[] calculateCoordinatesForAvailableVertices(List<int> availableVertices, Instance instance)
        {
            Coordinate[] coordinates = new Coordinate[availableVertices.Count];
            for (int i = 0; i < availableVertices.Count; i++)
                coordinates[i] = new Coordinate(instance.coordinates[availableVertices[i] - 1].x, instance.coordinates[availableVertices[i] - 1].y, instance.coordinates[availableVertices[i] - 1].weight);
            return coordinates;
        }

        public static double generateDistance(double x1, double y1, double x2, double y2)
        {
            var xd = x1 - x2;
            var yd = y1 - y2;
            var r = Math.Sqrt((xd * xd + yd * yd));
            var t = Math.Round(r, MidpointRounding.AwayFromZero);
            return t < r ? t + 1 : t;
        }
    }
}
