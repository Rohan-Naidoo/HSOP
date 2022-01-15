using HSOP.Helpers;
using HSOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSOP.Helpers
{
    class HSConstruction
    {
        public static Result constructNewSolution(List<Result> HM, double HMCR, double PAR, Instance instance, Random random)
        {
            List<int> route = new List<int>();
            route.Add(instance.noOfFirstVertex);
            double leftDistance = instance.maxDistance;
            List<int> verticesToVisit = Helper.generateVerticesWithoutFirstAndLast(instance);
            List<int> availableVertices = Helper.generateAvailableVertices(instance, leftDistance, route, verticesToVisit);
            while (availableVertices.Count > 0)
            {
                if (random.NextDouble() < HMCR)
                {
                    List<AvailableVertex> availableVerticesSuccessors = generateSuccessorVerticesSortedByObjectiveFunctionValue(route[route.Count - 1], HM, availableVertices, instance.distanceMatrix);
                    if (availableVerticesSuccessors.Count > 0)
                    {
                        int index = chooseIndexByRouletteWheel(random, availableVerticesSuccessors);
                        int chosenPoint = availableVerticesSuccessors[index].pointNumber;
                        route.Add(chosenPoint);
                    }
                    else
                    {
                        availableVerticesSuccessors = new List<AvailableVertex>();
                        for (int j = 0; j < availableVertices.Count; j++)
                        {
                            AvailableVertex vertex = new AvailableVertex();
                            vertex.pointNumber = availableVertices[j];
                            vertex.objectiveFunctionValue = instance.points[availableVertices[j] - 1] / instance.distanceMatrix[route[route.Count - 1] - 1, availableVertices[j] - 1];
                            availableVerticesSuccessors.Add(vertex);
                        }
                        availableVerticesSuccessors = availableVerticesSuccessors.OrderByDescending(w => w.objectiveFunctionValue).ToList();
                        for (int j = availableVerticesSuccessors.Count; j > HM.Count; j--)
                            availableVerticesSuccessors.RemoveAt(j - 1);
                        int index = chooseIndexByRouletteWheel(random, availableVerticesSuccessors);
                        int chosenPoint = availableVerticesSuccessors[index].pointNumber;
                        route.Add(chosenPoint);
                    }
                    if (random.NextDouble() < PAR)
                        route[route.Count - 1] = findClosestVerticesBasedOnPoints(route[route.Count - 2] - 1, availableVertices, instance, random, HM.Count);
                }
                else
                {
                    int indeks = random.Next(availableVertices.Count);
                    route.Add(availableVertices[indeks]);
                }
                leftDistance -= instance.distanceMatrix[route[route.Count - 2] - 1, route[route.Count - 1] - 1];
                verticesToVisit.Remove(route[route.Count - 1]);
                availableVertices = Helper.generateAvailableVertices(instance, leftDistance, route, verticesToVisit);
            }
            route.Add(instance.noOfLastVertex);
            int[] routeToAdd = route.ToArray();
            return new Result(routeToAdd, Helper.calculatePoints(routeToAdd, instance.points));
        }

        private new static List<AvailableVertex> generateSuccessorVerticesSortedByObjectiveFunctionValue(int previousVertex, List<Result> HM, List<int> availableVertices, double[,] distanceMatrix)
        {
            List<AvailableVertex> successorVertices = new List<AvailableVertex>();
            for (int i = 0; i < HM.Count; i++)
            {
                int previousIndex = Array.IndexOf(HM[i].getRoute(), previousVertex);
                if (previousIndex != -1)
                {
                    int pointNo = HM[i].getRoute()[previousIndex + 1];
                    if (availableVertices.Contains(pointNo))
                    {
                        AvailableVertex vertex = new AvailableVertex();
                        vertex.pointNumber = pointNo;
                        vertex.objectiveFunctionValue = HM[i].getObjectiveFunctionValue();
                        successorVertices.Add(vertex);
                    }
                }
            }
            return successorVertices.OrderBy(w => w.objectiveFunctionValue).ToList();
        }

        private static int chooseIndexByRouletteWheel(Random random, List<AvailableVertex> availableVerticesSuccessors)
        {
            double sum = availableVerticesSuccessors.Sum(w => w.objectiveFunctionValue == 0 ? 0 : w.objectiveFunctionValue);
            if (sum != 0)
                availableVerticesSuccessors = availableVerticesSuccessors.Select(w => { w.probabilityToChoose = w.objectiveFunctionValue == 0 ? 0 : w.objectiveFunctionValue / sum; return w; }).ToList();
            double probabilityToChoose = random.NextDouble();
            double chosenProbability = 0;
            int iterator = 0;
            while (chosenProbability < probabilityToChoose && iterator < availableVerticesSuccessors.Count)
            {
                chosenProbability += availableVerticesSuccessors[iterator].probabilityToChoose;
                iterator++;
            }
            if (iterator > 0)
                iterator--;
            return iterator;
        }

        private static int findClosestVerticesBasedOnPoints(int previousIndex, List<int> availableVertices, Instance instance, Random random, int HMS)
        {
            Coordinate[] coordinates = Helper.calculateCoordinatesForAvailableVertices(availableVertices, instance);
            Coordinate centerOfGravity = Helper.calculateCenterOfGravity(coordinates);
            Rang[] points = new Rang[coordinates.Length];
            Rang[] distancesToCenter = new Rang[coordinates.Length];
            Rang[] distancesToPrevious = new Rang[coordinates.Length];
            for (int i = 0; i < availableVertices.Count; i++)
            {
                points[i] = new Rang(i, instance.points[availableVertices[i] - 1]);
                double distanceToCenter = Helper.generateDistance(coordinates[i].x, coordinates[i].y, centerOfGravity.x, centerOfGravity.y);
                distancesToCenter[i] = new Rang(i, distanceToCenter);
                distancesToPrevious[i] = new Rang(i, instance.distanceMatrix[previousIndex, availableVertices[i] - 1]);
            }
            points = points.OrderByDescending(p => p.value).ToArray();
            distancesToCenter = distancesToCenter.OrderBy(p => p.value).ToArray();
            distancesToPrevious = distancesToPrevious.OrderBy(p => p.value).ToArray();
            Rang[] resultRangs = new Rang[coordinates.Length];
            for (int i = 0; i < resultRangs.Length; i++)
                resultRangs[i] = new Rang(-1, 0);
            var uniquePoints = points.GroupBy(p => p.value).Select(grp => new { value = grp.First(), counts = grp.Count() }).ToList();
            var uniqueToCenter = distancesToCenter.GroupBy(p => p.value).Select(grp => new { value = grp.First(), counts = grp.Count() }).ToList();
            var uniqueFromPrevious = distancesToPrevious.GroupBy(p => p.value).Select(grp => new { value = grp.First(), counts = grp.Count() }).ToList();
            int minimalNumberOfIndexes = uniquePoints.Count < uniqueToCenter.Count ? uniquePoints.Count : uniqueToCenter.Count;
            minimalNumberOfIndexes = uniqueFromPrevious.Count < minimalNumberOfIndexes ? uniqueFromPrevious.Count : minimalNumberOfIndexes;
            int diifference = minimalNumberOfIndexes - 1;

            for (int i = 0; i < resultRangs.Length; i++)
            {
                resultRangs[points[i].index].index = points[i].index;
                int index = uniquePoints.IndexOf(uniquePoints.First(p => p.value.value == points[i].value));
                double unit = (double)diifference / (uniquePoints.Count - 1);
                double result = (index == 0 ? 1 : 1 + index * unit);
                resultRangs[points[i].index].value += 0.7 * result;

                resultRangs[distancesToCenter[i].index].index = distancesToCenter[i].index;
                index = uniqueToCenter.IndexOf(uniqueToCenter.First(u => u.value.value == distancesToCenter[i].value));
                unit = (double)diifference / (uniqueToCenter.Count - 1);
                result = (index == 0 ? 1 : 1 + index * unit);
                resultRangs[distancesToCenter[i].index].value += 0.2 * result;

                resultRangs[distancesToPrevious[i].index].index = distancesToPrevious[i].index;
                index = uniqueFromPrevious.IndexOf(uniqueFromPrevious.First(u => u.value.value == distancesToPrevious[i].value));
                unit = (double)diifference / (uniqueFromPrevious.Count - 1);
                result = (index == 0 ? 1 : 1 + index * unit);
                resultRangs[distancesToPrevious[i].index].value += 0.1 * result;
            }
            resultRangs = resultRangs.OrderBy(r => r.value).ToArray();
            List<AvailableVertex> availableVerticesSuccessors = new List<AvailableVertex>();
            for (int j = 0; j < Math.Min(HMS, resultRangs.Length); j++)
            {
                AvailableVertex vertex = new AvailableVertex();
                vertex.objectiveFunctionValue = 1 / resultRangs[j].value;
                vertex.pointNumber = resultRangs[j].index;
                availableVerticesSuccessors.Add(vertex);
            }
            int resultIndex = chooseIndexByRouletteWheel(random, availableVerticesSuccessors);
            return availableVertices[availableVerticesSuccessors[resultIndex].pointNumber];
        }
    }
}
