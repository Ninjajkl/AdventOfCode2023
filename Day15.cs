using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day15
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
            String[] input = System.IO.File.ReadAllText(inputName).Split(',');
            int sum = input.Select(s => s.Aggregate(0, (cv, c) => (cv + c) * 17 % 256)).Sum();
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
            String[] input = System.IO.File.ReadAllText(inputName).Split(',');
            List<(string label, int focal)>[] boxes = new List<(string label, int focal)>[256];
            for(int i = 0; i < boxes.Length; i++)
            {
                boxes[i] = new List<(string label, int focal)>();
            }
            foreach (string s in input)
            {
                if (s[s.Length-2] == '=')
                {
                    string label = s.Substring(0, s.Length - 2);
                    int hash = label.Aggregate(0, (cv, c) => (cv + c) * 17 % 256);
                    int boxIndex = boxes[hash].FindIndex(tuple => tuple.label == label);
                    if (boxIndex == -1)
                    {
                        boxes[hash].Add((label, s[s.Length - 1] - '0'));
                    }
                    else
                    {
                        boxes[hash][boxIndex] = (label, s[s.Length - 1] - '0');
                    }
                }
                else
                {
                    string label = s.Substring(0, s.Length - 1);
                    int hash = label.Aggregate(0, (cv, c) => (cv + c) * 17 % 256);
                    int boxIndex = boxes[hash].FindIndex(tuple => tuple.label == label);
                    if (boxIndex != -1)
                    {
                        boxes[hash].RemoveAt(boxIndex);
                    }
                }
            }
            int sum = 0;
            for(int i = 0; i < boxes.Length; i++)
            {
                for(int j = 0; j < boxes[i].Count; j++)
                {
                    sum += (i + 1) * (j + 1) * boxes[i][j].focal;
                }
            }
            return sum.ToString();
        }
    }
}
