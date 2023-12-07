using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day06
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

            List<double> times = RawInput[0].Substring(9).Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(double.Parse).ToList();
            List<double> distances = RawInput[1].Substring(9).Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(double.Parse).ToList();
            List<(double, double)> combinedInput = times.Zip(distances, (item1, item2) => (item1, item2)).ToList();

            return GetNumValidSolutions(combinedInput).ToString();
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

            List<double> times = RawInput[0].Substring(9).Replace(" ", "").Split(' ').Select(double.Parse).ToList();
            List<double> distances = RawInput[1].Substring(9).Replace(" ", "").Split(' ').Select(double.Parse).ToList();
            List<(double, double)> combinedInput = times.Zip(distances, (item1, item2) => (item1, item2)).ToList();

            return GetNumValidSolutions(combinedInput).ToString();
        }

        /// <summary>
        /// Returns the product of the number of valid solutions for each item in the list
        /// </summary>
        /// <param name="combinedInput"> A list of 2-item tuples, with the first item the time, the second time the distance
        /// </param>
        /// <returns>
        /// Returns the product of all of the valid solutions for each race
        /// </returns>
        public double GetNumValidSolutions(List<(double, double)> combinedInput)
        {
            double product = 1;
            foreach ((double, double) race in combinedInput)
            {
                double possibleWins = 0;
                for (double i = Math.Floor((race.Item1 / 2.0) + 0.5); i <= race.Item1; i++)
                {
                    if (i * (race.Item1 - i) <= race.Item2)
                    {
                        break;
                    }
                    possibleWins++;
                }
                possibleWins *= 2;
                possibleWins -= race.Item1 % 2 == 0 ? 1 : 0;
                product *= possibleWins;
            }
            return product;
        }
    }
}
