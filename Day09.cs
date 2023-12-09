using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day09
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

            List<List<int>> ProcessedInput = new();
            foreach (String Input in RawInput)
            {
                ProcessedInput.Add(Input.Split(' ').Select(int.Parse).ToList());
            }

            int sum = 0;
            foreach (List<int> Input in ProcessedInput)
            {
                sum += FindNextValue(Input);
            }

            return sum.ToString();
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

            List<List<int>> ProcessedInput = new();
            foreach (String Input in RawInput)
            {
                ProcessedInput.Add(Input.Split(' ').Select(int.Parse).ToList());
            }

            int sum = 0;
            foreach (List<int> Input in ProcessedInput)
            {
                sum += FindPreviousValue(Input);
            }

            return sum.ToString();
        }

        /// <summary>
        /// Recursive Function to find the next value in an a series
        /// </summary>
        /// <param name="origList"> The series to extrapolate
        /// </param>
        /// <returns>
        /// The next value in the series
        /// </returns>
        public int FindNextValue(List<int> origList)
        {
            //If each value of x is 0, return 0 (as its the next value)
            if(origList.All(x => x == 0))
            {
                return 0;
            }
            //Otherwise, find the difference as a list
            //I love LINQ
            List<int> differenceList = origList.Zip(origList.Skip(1), (a, b) => b - a).ToList();
            return origList.Last() + FindNextValue(differenceList);
        }

        /// <summary>
        /// Recursive Function to find the previous value in an a series
        /// </summary>
        /// <param name="origList"> The series to extrapolate
        /// </param>
        /// <returns>
        /// The previous value in the series
        /// </returns>
        public int FindPreviousValue(List<int> origList)
        {
            //If each value of x is 0, return 0 (as its the next value)
            if (origList.All(x => x == 0))
            {
                return 0;
            }
            //Otherwise, find the difference as a list
            //I love LINQ
            List<int> differenceList = origList.Zip(origList.Skip(1), (a, b) => b - a).ToList();
            return origList.First() - FindPreviousValue(differenceList);
        }
    }
}
