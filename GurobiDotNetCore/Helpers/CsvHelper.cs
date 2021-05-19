using System;
using System.Collections.Generic;
using System.Linq;
using GurobiDotNetCore.Models;
using Microsoft.VisualBasic.FileIO;

namespace GurobiDotNetCore.Helpers
{
    public static class CsvHelper
    {
        public static void ParseStudent(string path, Dictionary<string, Student> studentDict, int scoreCnt)
        {
            using var parser = new TextFieldParser(path) {TextFieldType = FieldType.Delimited};
            parser.SetDelimiters(",");
            var isHead = true;
            var scoreIndex = -1;
            var firstNameIndex = -1;
            var lastNameIndex = -1;

            //var scoreCnt = 1;
            while (!parser.EndOfData)
            {
                //Processing row
                var fields = parser.ReadFields();
                if (isHead)
                {
                    for (var i = 0; i < fields.Length; i++)
                    {
                        if (fields[i].Contains("Điểm"))
                        {
                            scoreIndex = i;
                        }
                        else if (fields[i].Contains("Tên"))
                        {
                            firstNameIndex = i;
                        }
                        else if (fields[i].Contains("Họ"))
                        {
                            lastNameIndex = i;
                        }
                    }

                    if (scoreIndex < 0)
                    {
                        throw new Exception("Not found the \"Điểm\" column");
                    }

                    if (firstNameIndex < 0)
                    {
                        throw new Exception("Not found the \"Tên\" column");
                    }

                    if (lastNameIndex < 0)
                    {
                        throw new Exception("Not found the \"Họ\" column");
                    }

                    isHead = false;
                }
                else
                {
                    var isRowValid = true;
                    var firstName = fields[firstNameIndex];
                    var lastName = fields[lastNameIndex];
                    isRowValid &= double.TryParse(fields[scoreIndex].Replace(',', '.'), out var score);
                    if (score > 10)
                    {
                        throw new Exception("Invalid score! The score should be lesser than 10");
                    }

                    isRowValid &= !string.IsNullOrWhiteSpace(firstName);
                    isRowValid &= !string.IsNullOrWhiteSpace(lastName);
                    if (!isRowValid)
                    {
                        continue;
                    }

                    var name = lastName + " " + firstName;
                    Student student;
                    if (!studentDict.ContainsKey(name))
                    {
                        student = new Student
                        {
                            FirstName = fields[firstNameIndex],
                            LastName = fields[lastNameIndex],
                            Scores = new List<double>()
                        };
                        AddMissingScores(student.Scores, scoreCnt);
                        studentDict.Add(name, student);
                    }
                    else
                    {
                        student = studentDict[name];
                        AddMissingScores(student.Scores, scoreCnt);
                    }

                    if (student.Scores.Count > scoreCnt)
                    {
                        student.Scores[scoreCnt] = score;
                    }
                    else
                    {
                        student.Scores.Add(score);
                    }
                }
            }
        }

        private static int AddMissingScores(ICollection<double> scores, int scoreCnt)
        {
            var cnt = 0;
            if (scores.Count >= scoreCnt)
            {
                return cnt;
            }

            var nMissedScores = scoreCnt - scores.Count;
            for (var i = 0; i < nMissedScores; i++)
            {
                scores.Add(0);
                cnt++;
            }

            return cnt;
        }

        public static int UpdateMissingScores(ICollection<Student> students, int scoreCnt)
        {
            return students.Count(student => AddMissingScores(student.Scores, scoreCnt) > 0);
        }
    }
}