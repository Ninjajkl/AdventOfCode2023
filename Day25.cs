using Microsoft.Z3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day25
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

            List<string> vertices = new List<string>();
            List<(string, string)> edges = new List<(string, string)>();
            foreach (string input in RawInput)
            {
                string primary = input.Split(':')[0];
                string[] others = input.Split(":")[1].Trim().Split(" ");
                if (!vertices.Contains(primary))
                {
                    vertices.Add(primary);
                }
                foreach (string oth in others)
                {
                    if (!vertices.Contains(oth))
                    {
                        vertices.Add(oth);
                    }
                    edges.Add((primary, oth));
                }
            }

            //Karger's algorithm
            List<List<string>> subsets;
            do
            {
                subsets = new List<List<string>>();

                foreach (string vertex in vertices)
                {
                    subsets.Add(new List<string>() { vertex });
                }

                int i;
                List<string> subset1 = new();
                List<string> subset2 = new();

                while (subsets.Count > 2)
                {
                    i = new Random().Next() % edges.Count;

                    foreach (var subset in subsets)
                    {
                        if (subset.Contains(edges[i].Item1))
                        {
                            subset1 = subset;
                            break;
                        }
                    }

                    foreach (var subset in subsets)
                    {
                        if (subset.Contains(edges[i].Item2))
                        {
                            subset2 = subset;
                            break;
                        }
                    }

                    if (subset1 == subset2) continue;

                    subsets.Remove(subset2);
                    subset1.AddRange(subset2);
                }

            } while (CountCuts(subsets, edges) != 3);

            return subsets.Aggregate(1, (p, s) => p * s.Count).ToString();
        }

        public int CountCuts(List<List<string>> subsets, List<(string, string)> edges)
        {
            int cuts = 0;
            for (int i = 0; i < edges.Count; ++i)
            {
                List<string> subset1 = new();
                List<string> subset2 = new();

                foreach (var subset in subsets)
                {
                    if (subset.Contains(edges[i].Item1))
                    {
                        subset1 = subset;
                        break;
                    }
                }
                foreach (var subset in subsets)
                {
                    if (subset.Contains(edges[i].Item2))
                    {
                        subset2 = subset;
                        break;
                    }
                }
                if (subset1 != subset2) ++cuts;
            }

            return cuts;
        }
    }
}
