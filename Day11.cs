using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day11
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
            return SumGalaxyDistances(RawInput, 1).ToString();
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
            return SumGalaxyDistances(RawInput, 999999).ToString();
        }

        /// <summary>
        /// Computes the sum of the distance between each galaxy
        /// </summary>
        /// <param name="galaxyMap"> The map of the galaxies
        /// </param>
        /// <param name="expandAmount"> The amount to expand by when given an empty rol/col
        /// </param>
        /// <returns>
        /// A double containing a sum of the distances between every pair of galaxies
        /// </returns>
        public double SumGalaxyDistances(String[] galaxyMap, double expandAmount)
        {
            //Get the rows to expand
            List<int> expRows = new List<int>();
            for (int r = 0; r < galaxyMap.Length; r++)
            {
                if (!galaxyMap[r].Contains('#'))
                {
                    expRows.Add(r);
                }
            }
            //Get the cols to expand
            List<int> expCols = new List<int>();
            for (int c = 0; c < galaxyMap[0].Length; c++)
            {
                bool found = false;
                for (int r = 0; r < galaxyMap.Length; r++)
                {
                    if (galaxyMap[r][c] == '#')
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    expCols.Add(c);
                }
            }

            //Get the position of each galaxy, including offsets for the expanded parts of the map
            List<(double, double)> galaxies = new List<(double, double)>();
            double rOffset = 0;
            for (int i = 0; i < galaxyMap.Length; i++)
            {
                if (expRows.Contains(i))
                {
                    rOffset += expandAmount;
                }
                double cOffset = 0;
                for (int j = 0; j < galaxyMap[i].Length; j++)
                {
                    if (expCols.Contains(j))
                    {
                        cOffset += expandAmount;
                    }
                    if (galaxyMap[i][j] == '#')
                    {
                        galaxies.Add((i + rOffset, j + cOffset));
                    }
                }
            }

            //Sum up the manhattan distance of each galaxy to each other
            double sum = 0;
            for (int i = 0; i < galaxies.Count - 1; i++)
            {
                for (int j = i; j < galaxies.Count; j++)
                {
                    sum += Math.Abs(galaxies[i].Item1 - galaxies[j].Item1) + Math.Abs(galaxies[i].Item2 - galaxies[j].Item2);
                }
            }
            return sum;
        }
    }
}
