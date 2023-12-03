using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day03
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
            int sum = 0;
            for(int i = 0; i < RawInput.Length; i++)
            {
                string valueBuilder = "";
                bool valid = false;
                for(int j = 0; j < RawInput[i].Length; j++)
                {
                    if (char.IsDigit(RawInput[i][j]))
                    {
                        valueBuilder += RawInput[i][j];

                        //Check surrounding for symbols, if not already found
                        if (!valid)
                        {
                            for (int r = i - 1; r <= i + 1; r++)
                            {
                                for (int c = j - 1; c <= j + 1; c++)
                                {
                                    if (r >= 0 && r < RawInput.Length &&
                                       c >= 0 && c < RawInput[i].Length &&
                                       !(RawInput[r][c] == '.' ||
                                       RawInput[r][c] >= 48 && RawInput[r][c] <= 57))
                                    {
                                        valid = true;
                                    }
                                }
                            }
                        }
                    }
                    if((!char.IsDigit(RawInput[i][j]) || j == RawInput[i].Length-1) && valueBuilder.Length != 0) 
                    {
                        if (valid)
                        {
                            sum += int.Parse(valueBuilder);
                        }
                        valueBuilder = "";
                        valid = false;
                    }
                }
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
            Dictionary<Tuple<int, int>, List<int>> gearDict = new();
            int sum = 0;
            for (int i = 0; i < RawInput.Length; i++)
            {
                string valueBuilder = "";
                HashSet<Tuple<int,int>> gears = new();
                for (int j = 0; j < RawInput[i].Length; j++)
                {
                    if (char.IsDigit(RawInput[i][j]))
                    {
                        valueBuilder += RawInput[i][j];

                        //Check surrounding for gears, if not already found
                        for (int r = i - 1; r <= i + 1; r++)
                        {
                            for (int c = j - 1; c <= j + 1; c++)
                            {
                                if (r >= 0 && r < RawInput.Length &&
                                   c >= 0 && c < RawInput[i].Length &&
                                   RawInput[r][c] == '*')
                                {
                                    gears.Add(Tuple.Create(r, c));
                                }
                            }
                        }
                    }
                    if ((!char.IsDigit(RawInput[i][j]) || j == RawInput[i].Length - 1) && valueBuilder.Length != 0)
                    {
                        foreach(Tuple<int,int> gear in gears)
                        {
                            if (!gearDict.ContainsKey(gear))
                            {
                                gearDict.Add(gear, new List<int>());
                            }
                            gearDict[gear].Add(int.Parse(valueBuilder));
                        }
                        valueBuilder = "";
                        gears = new();
                    }
                }
            }
            foreach(List<int> gearList in gearDict.Values)
            {
                if(gearList.Count == 2)
                {
                    sum += gearList[0] * gearList[1];
                }
            }
            return sum.ToString();
        }
    }
}
