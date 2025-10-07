using HSOP.Algorithms;
using HSOP.Helpers;
using HSOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
namespace HSOP
{
    class HS
    {
        public static Result solve(Instance instance)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            const double HMCR = 0.98;
            const double PAR = 0.1;
            const byte HMS = 5;
            const int R = 500;
            const int maxIterations = 1000000;
            List<Result> HM = new List<Result>();   

            for (int i = 0; i < HMS; i++)
                HM.Add(RandomSolution.solve(instance, random));
            HM = HM.OrderByDescending(w => w.getObjectiveFunctionValue()).ToList();
            Result bestSolution = new Result(HM[0].getRoute(), HM[0].getObjectiveFunctionValue());
            int noOfIterationsFromLastReplacement = 0;
            int numberOfIterations = 1;

            while (numberOfIterations <= maxIterations)
            {
                Result newSolution = HSConstruction.constructNewSolution(HM, HMCR, PAR, instance, random);
                if (HM[HM.Count - 1].getObjectiveFunctionValue() < newSolution.getObjectiveFunctionValue())
                {
                    newSolution = _2opt.solve(instance.distanceMatrix, instance.points, newSolution);

                    List<int> unvisitedVertices = Helper.generateVerticesWithoutFirstAndLast(instance);
                    for (int j = 1; j < newSolution.getRoute().Length - 1; j++)
                        unvisitedVertices.Remove(newSolution.getRoute()[j]);
                    Result improvedSolution = Improve.changeVertex(instance, newSolution.getRoute(), unvisitedVertices, 1);
                    double pointsIncrease = improvedSolution.getObjectiveFunctionValue() - newSolution.getObjectiveFunctionValue();
                    if (pointsIncrease > 0)
                    {
                        newSolution = improvedSolution;
                        newSolution = _2opt.solve(instance.distanceMatrix, instance.points, newSolution);
                    }
                    HM[HM.Count - 1] = newSolution;
                    HM = HM.OrderByDescending(w => w.getObjectiveFunctionValue()).ToList();
                    noOfIterationsFromLastReplacement = 0;
                }
                else
                    noOfIterationsFromLastReplacement++;

                if (HM[0].getObjectiveFunctionValue() > bestSolution.getObjectiveFunctionValue())
                {
                    bestSolution = new Result(HM[0].getRoute(), HM[0].getObjectiveFunctionValue());
                }

                if (noOfIterationsFromLastReplacement == R)
                {
                    for (int i = 1; i < HMS; i++)
                        HM[i] = RandomSolution.solve(instance, random);
                    HM = HM.OrderByDescending(w => w.getObjectiveFunctionValue()).ToList();
                    noOfIterationsFromLastReplacement = 0;
                }

                numberOfIterations++;
            }

            return new Result(bestSolution.getRoute(), bestSolution.getObjectiveFunctionValue());
        }
    }
    public static class Program
    {
        public static void Main(string[] args)
        {
        
            var path = args.Length > 0 ? args[0] : 
            "tests/Tsiligirides_1/tsiligirides_problem_1_budget_85.txt";
            if (!File.Exists(path))
            {
                Console.WriteLine($"File not found: {path}");
                return;
            }

            var lines = File.ReadAllLines(path)
                            .Select(l => l.Trim())
                            .Where(l => !string.IsNullOrWhiteSpace(l))
                            .ToArray();

            // Expect first line: "<maxDistance> <noOfFirstVertex>"
            var header = lines[0].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (header.Length < 2)
            {
                Console.WriteLine("Header must contain: <maxDistance> <noOfFirstVertex>");
                return;
            }

            double maxDistance = double.Parse(header[0], CultureInfo.InvariantCulture);
            int noOfFirstVertex = 1;

            var coordLines = lines.Skip(1).ToArray();
            int n = coordLines.Length;
            var coords = new Coordinate[n];
            for (int i = 0; i < n; i++)
            {
                var parts = coordLines[i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                double x = double.Parse(parts[0], CultureInfo.InvariantCulture);
                double y = double.Parse(parts[1], CultureInfo.InvariantCulture);
                double w = parts.Length >= 3 ? double.Parse(parts[2], CultureInfo.InvariantCulture) : 0.0;
                coords[i] = new Coordinate(x, y, w);
            }

            // build distance matrix (symmetric)
            var distanceMatrix = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    distanceMatrix[i, j] = Helper.generateDistance(coords[i].x, coords[i].y, coords[j].x, coords[j].y);
                }
            }

            var points = coords.Select(c => c.weight).ToArray();
            int noOfLastVertex = 2; // assume last vertex index = 2
            var instance = new Instance(coords, distanceMatrix, points, maxDistance, noOfFirstVertex, noOfLastVertex);

            Console.WriteLine("Running HS solver...");
            var result = HS.solve(instance);

            Console.WriteLine("Route: [" + string.Join(" , ", result.getRoute()) +"]");
            Console.WriteLine("Points: " + result.getObjectiveFunctionValue());
            double routeLength = Helper.calculateRouteLength(result.getRoute(), instance.distanceMatrix);
            Console.WriteLine($"Route length: {routeLength}");
            Console.WriteLine($"maxDistance: {instance.maxDistance}");
            // Console.WriteLine($"noOfFirstVertex: {instance.noOfFirstVertex}");
            // Console.WriteLine($"noOfLastVertex: {instance.noOfLastVertex}");

            // if (instance.distanceMatrix != null)
            // {
            //     int r = instance.distanceMatrix.GetLength(0), ccount = instance.distanceMatrix.GetLength(1);
            //     Console.WriteLine($"distanceMatrix: {r}x{ccount}");
            //     // print first few rows for brevity
            //     int rowsToShow = Math.Min(5, r);
            //     for (int i = 0; i < rowsToShow; i++)
            //     {
            //         var row = new double[ccount];
            //         for (int j = 0; j < ccount; j++) row[j] = instance.distanceMatrix[i, j];
            //         Console.WriteLine(string.Join(", ", row.Select(v => v.ToString(CultureInfo.InvariantCulture))));
            //     }
            // }

            // Console.WriteLine("Instance coordinates:");
            // for (int i = 0; i < instance.coordinates.Length; i++)
            // {
            //     var c = instance.coordinates[i];
            //     Console.WriteLine($"{i + 1}: x={c.x.ToString(CultureInfo.InvariantCulture)}, y={c.y.ToString(CultureInfo.InvariantCulture)}, w={c.weight.ToString(CultureInfo.InvariantCulture)}");
            // }


        }
    }
}




