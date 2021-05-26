using System;
using System.Collections.Generic;
using System.Linq;
using Gurobi;
using GurobiDotNetCore.Helpers;
using GurobiDotNetCore.Interfaces.Models;

namespace GurobiDotNetCore.Models
{
    public class SolveModel : ISolveModel
    {
        #region Model 1 (k = 2)

        public void Solve(GRBEnv env, IList<Student> students, out GRBModel model)
        {
            if (env == null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            if (students == null)
            {
                throw new ArgumentNullException(nameof(students));
            }

            model = new GRBModel(env);
            var nStudents = students.Count;

            // Add variables x[i][j]
            var x = new GRBVar[nStudents, nStudents];
            for (var i = 0; i < nStudents; ++i)
            {
                for (var j = 0; j < nStudents; ++j)
                {
                    x[i, j] = model.AddVar(0, 1, CalcHelper.CalculateCNum(students[i].Scores, students[j].Scores),
                        GRB.BINARY,
                        $"x-{i}-{j}");
                }
            }

            // Maximize
            model.ModelSense = GRB.MAXIMIZE;

            GRBLinExpr exp4 = 0;
            for (var i = 0; i < nStudents; i++)
            {
                GRBLinExpr exp1 = 0;
                GRBLinExpr exp2 = 0;
                for (var j = 0; j < nStudents; ++j)
                {
                    exp1.AddTerm(1, x[i, j]);
                    exp2.AddTerm(1, x[j, i]);

                    GRBLinExpr exp3 = 0;
                    exp3.AddTerm(1, x[i, j]);
                    model.AddConstr(exp3 == x[j, i], $"x[{i}, {j}] == x[{j}, {i}]");

                    if (i == j)
                    {
                        exp4.AddTerm(1, x[i, j]);
                    }
                }

                model.AddConstr(exp1 == 1, $"Pair of stu_{i}-x");
                model.AddConstr(exp2 == 1, $"Pair of stu_x-{i}");
            }

            model.AddConstr(exp4 <= 1, "Allow only a single student if any");

            model.Optimize();
        }

        public void PrintSolution(GRBModel model, IList<Student> students)
        {
            var nStudents = students.Count;
            var status = model.Status;

            if (status == GRB.Status.OPTIMAL)
            {
                Console.WriteLine("The optimal objective is " + model.ObjVal / 2);
                Console.WriteLine("Assignment array:");
                for (var i = 0; i < nStudents; ++i)
                {
                    for (var j = 0; j < nStudents; ++j)
                    {
                        var x = model.GetVarByName($"x-{i}-{j}");
                        if (x.X > 0 && i <= j)
                        {
                            Console.WriteLine($"[student-{i}]-[student-{j}]");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("FAILED!");
            }
        }

        #endregion

        #region Model 2 (k > 2)

        public void Solve(GRBEnv env, IList<Student> students, int kMin, int kMax, out GRBModel model)
        {
            if (env == null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            if (students == null)
            {
                throw new ArgumentNullException(nameof(students));
            }

            if (kMin > kMax)
            {
                throw new ArgumentException("kMin must be lesser than kMax!");
            }

            model = new GRBModel(env);
            var nStudents = students.Count;
            var nGroups = nStudents / kMax;

            if (nStudents % kMax != 0)
            {
                //throw new ArgumentException($"The student number can not dived into {k} group!");

                nGroups++;
            }

            var minNStudents = kMin * nGroups;
            if (nStudents < minNStudents)
            {
                throw new ArgumentException(
                    $"Cannot assigned groups for ${nStudents} students with at least {kMin} students in one group.");
            }

            var nScores = students.First().Scores.Count;
            for (var i = 0; i < nStudents; i++)
            {
                if (students[i].Scores.Count != nScores)
                {
                    throw new ArgumentException($"Invalid score array in Student[{i}]!");
                }
            }

            // x[i][j]
            var x = new GRBVar[nStudents, nGroups];
            for (var i = 0; i < nStudents; ++i)
            {
                for (var j = 0; j < nGroups; ++j)
                {
                    x[i, j] = model.AddVar(0, 1, 0, GRB.BINARY, $"x-{i}-{j}");
                }
            }

            // y: raw, not min value of c
            // z: min(y, 10)
            var y = new GRBVar[nGroups, nScores];
            var z = new GRBVar[nGroups, nScores];
            for (var i = 0; i < nGroups; ++i)
            {
                for (var j = 0; j < nScores; ++j)
                {
                    y[i, j] = model.AddVar(0, double.PositiveInfinity, 0, GRB.CONTINUOUS, $"y-{i}-{j}");
                    z[i, j] = model.AddVar(0, 10, 0, GRB.CONTINUOUS, $"z-{i}-{j}");
                }
            }

            model.ModelSense = GRB.MAXIMIZE;

            for (var s = 0; s < nStudents; s++)
            {
                GRBLinExpr expr = 0;
                for (var g = 0; g < nGroups; ++g)
                {
                    expr.AddTerm(1, x[s, g]);
                }

                model.AddConstr(expr == 1, $"Student {s} is assigned to only one group");
            }

            for (var g = 0; g < nGroups; g++)
            {
                GRBLinExpr expr = 0;
                for (var s = 0; s < nStudents; ++s)
                {
                    expr.AddTerm(1, x[s, g]);
                }

                model.AddConstr(expr <= kMax, $"There is maximum {kMax} students in group {g}");
                model.AddConstr(expr >= kMin, $"There is at least {kMin} students in group {g}");
            }

            GRBLinExpr cg = 0;
            for (var g = 0; g < nGroups; g++)
            {
                GRBLinExpr c = 0;
                for (var i = 0; i < nScores; i++)
                {
                    GRBLinExpr c1 = 0;
                    for (var s = 0; s < nStudents; ++s)
                    {
                        c1.AddTerm(students[s].Scores[i], x[s, g]);
                    }

                    model.AddConstr(c1 == y[g, i], $"tmp1-{g}-{i}");
                    model.AddGenConstrMin(z[g, i], new[] {y[g, i]}, 10, $"tmp2-{g}-{i}");
                    c.Add(z[g, i]);
                }

                cg.Add(c);
            }

            model.SetObjective(cg);

            model.Optimize();
        }

        public void PrintSolution(GRBModel model, IList<Student> students, int k)
        {
            var nStudents = students.Count;
            var status = model.Status;
            var nGroups = nStudents / k;

            if (nStudents % k != 0)
            {
                nGroups++;
            }

            if (status == GRB.Status.OPTIMAL)
            {
                Console.WriteLine("The optimal objective is " + model.ObjVal);
                Console.WriteLine("Assignment array:");
                for (var i = 0; i < nStudents; ++i)
                {
                    for (var j = 0; j < nGroups; ++j)
                    {
                        var x = model.GetVarByName($"x-{i}-{j}");
                        if (x.X > 0)
                        {
                            Console.WriteLine($"[student-{i}]-[group-{j}]");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("FAILED!");
            }
        }

        #endregion

        #region Matheuristic

        private List<string> CreateAssignmentArray(GRBModel model, IList<Student> students, int k, int preNStudent, int preNGroup)
        {
            var ret = new List<string>();
            var nStudents = students.Count;
            var status = model.Status;
            var nGroups = nStudents / k;

            if (nStudents % k != 0)
            {
                nGroups++;
            }

            if (status == GRB.Status.OPTIMAL)
            {
                for (var i = 0; i < nStudents; ++i)
                {
                    for (var j = 0; j < nGroups; ++j)
                    {
                        var x = model.GetVarByName($"x-{i}-{j}");
                        if (x.X > 0)
                        {
                            //Console.WriteLine($"[student-{i}]-[group-{j}]");
                            ret.Add($"[student-{preNStudent + i}]-[group-{preNGroup + j}]");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("FAILED!");
            }

            return ret;
        }

        private static List<List<Student>> Split(List<Student> students, int n, int kGroup)
        {
            var ret = new List<List<Student>>();
            var k = students.Count / n;
            if (students.Count % n != 0)
            {
                k++;
            }

            if (k % kGroup != 0)
            {
                k += kGroup - k % kGroup;
            }

            var startIndex = 0;
            for (var i = 0; i < n; i++)
            {
                if (startIndex + k <= students.Count)
                {
                    ret.Add(students.GetRange(startIndex, k));
                    startIndex += k;
                }
                else
                {
                    var remain = students.Count - startIndex;
                    if (remain > 0)
                    {
                        ret.Add(students.GetRange(startIndex, remain));
                    }

                    startIndex += remain;
                }
            }

            return ret;
        }

        public void Solve(GRBEnv env, List<Student> students, int nTrunks, out double cost,
            out List<string> assignmentArray)
        {
            assignmentArray = new List<string>();

            var trunks = Split(students, nTrunks, 2);
            var preNStudents = 0;
            var preNGroups = 0;
            cost = 0.0;
            for (var i = 0; i < trunks.Count; i++)
            {
                Console.WriteLine($"Solving with {trunks[i].Count} students...");
                Solve(env, trunks[i], out var model);
                cost += model.ObjVal;

                for (var l = 0; l < trunks[i].Count; l++)
                {
                    for (var m = 0; m < trunks[i].Count; m++)
                    {
                        var x = model.GetVarByName($"x-{l}-{m}");
                        if (x.X > 0 && l <= m)
                        {
                            assignmentArray.Add($"[student-{preNStudents+ l}]-[student-{preNStudents + m}]");
                        }
                    }
                }
                preNStudents += trunks[i].Count;
                //model.Dispose();
                Console.WriteLine($"DONE [{i + 1}/{trunks.Count}]------------------------");
            }
        }

        public void Solve(GRBEnv env, List<Student> students, int kMin, int kMax, int nTrunks, out double cost, out List<string> assignmentArray)
        {
            assignmentArray = new List<string>();

            var trunks = Split(students, nTrunks, kMax);
            var preNStudents = 0;
            var preNGroups = 0;
            cost = 0.0;
            for (var i = 0; i < trunks.Count; i++)
            {
                Console.WriteLine($"Solving with {trunks[i].Count} students...");
                Solve(env, trunks[i], kMin, kMax, out var model);
                cost += model.ObjVal;
                assignmentArray.AddRange(CreateAssignmentArray(model, trunks[i], kMax, preNStudents, preNGroups));
                preNStudents += trunks[i].Count;
                var nGroups = trunks[i].Count / kMax;

                if (trunks[i].Count % kMax != 0)
                {
                    //throw new ArgumentException($"The student number can not dived into {k} group!");

                    nGroups++;
                }

                preNGroups += nGroups;
                //model.Dispose();
                Console.WriteLine($"DONE [{i+1}/{trunks.Count}]------------------------");
            }

            //Console.WriteLine($"Objective value: ${cost}");
            //foreach (var line in assignmentArray)
            //{
            //    Console.WriteLine(line);
            //}
            //env.Dispose();
        }

        #endregion
    }
}