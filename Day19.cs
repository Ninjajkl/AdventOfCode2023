using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day19
    {
        /// <summary>
        /// Computes the answer for Part 1
        /// </summary>
        /// <param name="inputName">The address of the input file to take input from
        /// </param>
        /// <returns>
        /// Results the result as a string to be printed
        /// </returns>
        public string Part1(string inputName)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);
            Workflow.workflows.Clear();
            bool isWorkflows = true;
            int partTotal = 0; 
            foreach (string input in RawInput)
            {
                if(input.Length == 0) 
                {
                    isWorkflows = false;
                    continue;
                }
                if (isWorkflows)
                {
                    new Workflow(input);
                }
                else
                {
                    int[] pR = input[1..^1].Split(',').Select(x => int.Parse(x[2..])).ToArray();
                    (int x, int m, int a, int s) partRatings = (pR[0], pR[1], pR[2], pR[3]);
                    if (Workflow.workflows["in"].EvaluatePart(partRatings))
                    {
                        partTotal += partRatings.x + partRatings.m + partRatings.a + partRatings.s;
                    }
                }
            }

            return partTotal.ToString();
        }

        /// <summary>
        /// Computes the answer for Part 2
        /// </summary>
        /// <param name="inputName">The address of the input file to take input from
        /// </param>
        /// <returns>
        /// Results the result as a string to be printed
        /// </returns>
        public string Part2(string inputName)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);
            Workflow.workflows.Clear();
            foreach (string input in RawInput)
            {
                if (input.Length == 0)
                {
                    break;
                }
                new Workflow(input);
            }

            List<(int, int)> inputRanges = new List<(int, int)>
            {
                {(1,4000)},
                {(1,4000)},
                {(1,4000)},
                {(1,4000)}
            };

            return Workflow.workflows["in"].CalcDistinctPossibilities(inputRanges).ToString();
        }
    }

    public class Workflow
    {
        /// <summary>
        /// Stores all of the workflows
        /// </summary>
        public static Dictionary<string, Workflow> workflows = new Dictionary<string, Workflow>();

        List<(char cat, char oper, int rating, string dest)> checks;
        string defaultLoc;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="input"> The input string to be processed into a workflow
        /// </param>
        public Workflow(string input) 
        {
            checks = new();
            int startingIndex = input.IndexOf('{');
            string[] operations = input[(startingIndex + 1)..^1].Split(",");
            foreach (string op in operations)
            {
                if (!op.Contains(':'))
                {
                    defaultLoc = op;
                }
                else
                {
                    char cat = op[0];
                    char oper = op[1];
                    int colonIndex = op.IndexOf(':');
                    int rating = int.Parse(op[2..colonIndex]);
                    string dest = op[(colonIndex + 1)..];
                    checks.Add((cat, oper, rating, dest));
                }
            }
            workflows.Add(input[..startingIndex], this);
        }

        /// <summary>
        /// Recursive function to find validity of given part
        /// </summary>
        /// <param name="partRatings"> The tuple containing the part ratings in each category
        /// <param>
        /// <returns>
        /// A bool stating if the part is valid
        /// </returns>
        public bool EvaluatePart((int x, int m, int a, int s) partRatings)
        {
            var categoryMapping = new Dictionary<char, int>()
            {
                { 'x', partRatings.x },
                { 'm', partRatings.m },
                { 'a', partRatings.a },
                { 's', partRatings.s }
            };

            foreach ((char cat, char oper, int rating, string dest) check in checks)
            {
                int valToCompare = categoryMapping[check.cat];
                int comparison = valToCompare.CompareTo(check.rating);
                if (check.oper == '>' && comparison > 0 || check.oper == '<' && comparison < 0)
                {
                    if (check.dest == "A")
                    {
                        return true;
                    }
                    else if (check.dest == "R")
                    {
                        return false;
                    }
                    return workflows[check.dest].EvaluatePart(partRatings);
                }
            }
            if (defaultLoc == "A")
            {
                return true;
            }
            else if (defaultLoc == "R")
            {
                return false;
            }
            return workflows[defaultLoc].EvaluatePart(partRatings);
        }
        
        /// <summary>
        /// Recursive function to find the number of possible valid parts in a given range
        /// </summary>
        /// <param name="partRatingsRanges"> A tuple containing the ranges of the part's categories
        /// </param>
        /// <returns>
        /// The number of valid possiblies in the given ranges
        /// </returns>
        public long CalcDistinctPossibilities(List<(int min, int max)> partRatingsRanges)
        {
            var categoryMapping = new Dictionary<char, int>()
            {
                { 'x', 0 },
                { 'm', 1 },
                { 'a', 2 },
                { 's', 3 }
            };

            long possibilities = 0;

            foreach ((char cat, char oper, int rating, string dest) check in checks)
            {
                int inputIndex = categoryMapping[check.cat];
                (int min, int max) rangeToCompare = partRatingsRanges[inputIndex];
                (int min, int max) lessThanRange = check.oper == '>' ? (rangeToCompare.min, check.rating) : (rangeToCompare.min, check.rating-1);
                (int min, int max) greaterThanRange = check.oper == '>' ? (check.rating+1, rangeToCompare.max) : (check.rating, rangeToCompare.max);

                List<(int min, int max)> succRange = new List<(int min, int max)>(partRatingsRanges);
                succRange[inputIndex] = check.oper == '<' ? lessThanRange : greaterThanRange;
                partRatingsRanges[inputIndex] = check.oper != '<' ? lessThanRange : greaterThanRange;
                if (check.dest == "R")
                {
                    //Do nothing
                }
                else if (check.dest == "A")
                {
                    possibilities += succRange.Aggregate(1L, (acc, range) => acc * (range.max - range.min + 1));
                }
                else
                {
                    possibilities += workflows[check.dest].CalcDistinctPossibilities(succRange);
                }
            }

            if (defaultLoc == "R")
            {
                // Do nothing
            }
            else if (defaultLoc == "A")
            {
                possibilities += partRatingsRanges.Aggregate(1L, (acc, range) => acc * (range.max - range.min + 1));
            }
            else
            {
                possibilities += workflows[defaultLoc].CalcDistinctPossibilities(partRatingsRanges);
            }
            return possibilities;
        }
    }
}
