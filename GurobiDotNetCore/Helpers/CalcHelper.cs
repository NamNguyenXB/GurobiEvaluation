using System;
using System.Collections.Generic;

namespace GurobiDotNetCore.Helpers
{
    public static class CalcHelper
    {
        public static double CalculateCNum(IList<double> studentScores1, IList<double> studentScore2)
        {
            if (studentScores1 == null)
            {
                throw new ArgumentNullException(nameof(studentScores1));
            }

            if (studentScore2 == null)
            {
                throw new ArgumentNullException(nameof(studentScore2));
            }

            var k = studentScores1.Count;
            if (k != studentScore2.Count)
            {
                throw new ArgumentException("Cannot Calculate the cNum! The 2 score arrays have different size!");
            }

            double result = 0;
            for (var i = 0; i < k; i++)
            {
                result += Math.Min(10, studentScores1[i] + studentScore2[i]);
            }

            return result;
        }
    }
}