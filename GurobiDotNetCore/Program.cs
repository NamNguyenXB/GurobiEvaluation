using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Gurobi;
using GurobiDotNetCore.Helpers;
using GurobiDotNetCore.Models;

namespace GurobiDotNetCore
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var studentDict = new Dictionary<string, Student>();
            var subFolders = Directory.GetDirectories(@"test\");
            var scoreCnt = 0;
            foreach (var subFolder in subFolders)
            {
                var files = Directory.GetFiles(subFolder, "*.csv").ToList();
                foreach (var filePath in files)
                {
                    CsvHelper.ParseStudent(filePath, studentDict, scoreCnt);
                    CsvHelper.UpdateMissingScores(studentDict.Values, scoreCnt + 1);
                }

                scoreCnt++;
            }

            Console.WriteLine(studentDict.Count);

            try
            {
                // Create an empty environment, set options and start
                var env = new GRBEnv(true);
                env.Set("LogFile", "solver.log");
                env.Start();

                //var students = studentDict.Values.ToList().GetRange(0, 200);
                var students = studentDict.Values.ToList();
                var solveModel = new SolveModel();

                var stopwatch = new Stopwatch();
                const int demoCode = 1;
                const int kMax = 3;
                const int kMin = 2;
                const int limitStudent = 100;

                // Pair 2 - all students.
                if (demoCode == 0)
                {
                    stopwatch.Start();
                    solveModel.Solve(env, students, out var model);
                    stopwatch.Stop();
                    solveModel.PrintSolution(model, students);
                    Console.WriteLine("Execution Time is {0} ms", stopwatch.ElapsedMilliseconds);
                }

                // Matheuristic kMin = 2; kMax = 3; nSubClassMax = 20
                if (demoCode == 1)
                {
                    stopwatch.Start();
                    solveModel.Solve(env, students, kMin, kMax, 20, out var cost, out var assignmentArray);
                    stopwatch.Stop();
                    Console.WriteLine($"Objective value: ${cost}");
                    foreach (var line in assignmentArray)
                    {
                        Console.WriteLine(line);
                    }

                    env.Dispose();
                    Console.WriteLine("Execution Time is {0} ms", stopwatch.ElapsedMilliseconds);
                }

                // kMin = 2; kMax = 3 with only 100 students.
                if (demoCode == 2)
                {
                    Console.WriteLine($"Run solver with the first {limitStudent} students");
                    var testStudents = students.GetRange(0, limitStudent);
                    stopwatch.Start();
                    solveModel.Solve(env, testStudents, kMin, kMax, out var model);
                    stopwatch.Stop();
                    solveModel.PrintSolution(model, testStudents, kMax);
                    Console.WriteLine("Execution Time is {0} ms", stopwatch.ElapsedMilliseconds);
                }

                // Matheuristic kMin = 2; kMax = 3; nSubClassMax = 5 with only 100 students.
                else if (demoCode == 3)
                {
                    Console.WriteLine($"Run matheuristic with the first {limitStudent} students");
                    var testStudents = students.GetRange(0, limitStudent);
                    stopwatch.Start();
                    solveModel.Solve(env, testStudents, kMin, kMax, 5, out var cost, out var assignmentArray);
                    stopwatch.Stop();

                    Console.WriteLine($"Objective value: ${cost}");
                    foreach (var line in assignmentArray)
                    {
                        Console.WriteLine(line);
                    }

                    env.Dispose();

                    Console.WriteLine("Execution Time is {0} ms", stopwatch.ElapsedMilliseconds);
                }

                else if (demoCode == 4)
                {
                    stopwatch.Start();
                    solveModel.Solve(env, students, kMin, kMax, out var model);
                    stopwatch.Stop();
                    solveModel.PrintSolution(model, students, kMax);
                    Console.WriteLine("Execution Time is {0} ms", stopwatch.ElapsedMilliseconds);
                }

                if (demoCode == 5)
                {
                    stopwatch.Start();
                    solveModel.Solve(env, students, 20, out var cost, out var assignmentArray);
                    stopwatch.Stop();

                    foreach (var line in assignmentArray)
                    {
                        Console.WriteLine(line);
                    }

                    Console.WriteLine($"Objective value: ${cost/2}");
                    
                    env.Dispose();

                    Console.WriteLine("Execution Time is {0} ms", stopwatch.ElapsedMilliseconds);
                }
            }
            catch (GRBException e)
            {
                Console.WriteLine("Error code: " + e.ErrorCode + ". " + e.Message);
            }
        }
    }
}