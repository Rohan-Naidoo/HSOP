using HSOP.Algorithms;
using HSOP.Helpers;
using HSOP.Models;
using HSOP.Services;
using System;
using System.Collections.Generic;
using System.Linq;

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
}
