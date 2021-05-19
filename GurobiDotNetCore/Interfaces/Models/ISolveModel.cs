using System.Collections.Generic;
using Gurobi;
using GurobiDotNetCore.Models;

namespace GurobiDotNetCore.Interfaces.Models
{
    public interface ISolveModel
    {
        void Solve(GRBEnv env, IList<Student> students, out GRBModel model);
        void PrintSolution(GRBModel model, IList<Student> students);

        void Solve(GRBEnv env, IList<Student> students, int kMin, int kMax, out GRBModel model);
        void PrintSolution(GRBModel model, IList<Student> students, int kMax);
    }
}