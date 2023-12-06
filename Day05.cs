using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace AdventOfCode2023
{
    internal class Day05
    {
        /// <summary>
        /// Computes the answer for Part 1 (Original Method)
        /// </summary>
        /// <param name="inputName">The address of the input file to take input from
        /// </param>
        /// <returns>
        /// Results the result as a string to be printed
        /// </returns>
        public string Part1Orig(string inputName)
        {
            List<double> Seeds = new();
            //Create an array of length 7 (One for each map)
            List<(double, double, double)>[] maps = new List<(double, double, double)>[7];
            //Get the Seeds and Maps from the input
            ProcessInput(inputName, out Seeds, out maps);

            //Check the seed value of every seed to see which is the smallest
            double minLocation = double.MaxValue;
            foreach (double seed in Seeds)
            {
                double nextValue = getSeedValue(maps, seed);
                if(nextValue < minLocation)
                {
                    minLocation = nextValue;
                }
            }
            return minLocation.ToString();
        }

        /// <summary>
        /// Computes the answer for Part 1 (Faster method developed after Part2)
        /// </summary>
        /// <param name="inputName">The address of the input file to take input from
        /// </param>
        /// <returns>
        /// Results the result as a string to be printed
        /// </returns>
        public string Part1(string inputName)
        {
            List<double> originalSeeds;
            List<(double, double)> Seeds = new();
            //Create an array of length 7 (One for each map)
            List<(double, double, double)>[] maps = new List<(double, double, double)>[7];
            ProcessInput(inputName, out originalSeeds, out maps);

            //Coverts the original seed input into the seed ranges, then sorts it (ascending order)
            for (int j = 0; j < originalSeeds.Count; j ++)
            {
                Seeds.Add((originalSeeds[j], 1));
            }
            Seeds.Sort(new FEComparer2());

            //For each map, makes sure it has access to the full range of values
            Array.ForEach(maps, CheckAndModifyMaps);

            //Use recursion to find the answer
            //The first positive value found is the smallest (due to our sorting)
            foreach ((double, double, double) range in maps[6])
            {
                double result = RecursiveCheck(Seeds, maps, 5, range);
                if (result != -1)
                {
                    return getSeedValue(maps, result).ToString();
                }
            }
            //Something amazing has gone wrong ¯\_(ツ)_/¯
            return (-1).ToString();
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
            List<double> originalSeeds;
            List<(double, double)> Seeds = new();
            //Create an array of length 7 (One for each map)
            List<(double, double, double)>[] maps = new List<(double, double, double)>[7];
            ProcessInput(inputName, out originalSeeds, out maps);

            //Coverts the original seed input into the seed ranges, then sorts it (ascending order)
            for (int j = 0; j < originalSeeds.Count; j += 2)
            {
                Seeds.Add((originalSeeds[j], originalSeeds[j + 1]));
            }
            Seeds.Sort(new FEComparer2());

            //For each map, makes sure it has access to the full range of values
            Array.ForEach(maps, CheckAndModifyMaps);

            //Use recursion to find the answer
            //The first positive value found is the smallest (due to our sorting)
            foreach ((double, double, double) range in maps[6])
            {
                double result = RecursiveCheck(Seeds, maps, 5, range);
                if (result != -1)
                {
                    return getSeedValue(maps, result).ToString();
                }
            }
            //Something amazing has gone wrong ¯\_(ツ)_/¯
            return (-1).ToString();
        }

        /// <summary>
        /// Processes the Input into the seeds and maps lists
        /// </summary>
        /// <param name="inputName">The address of the input file to take input from
        /// </param>
        /// <param name="Seeds">The list of doubles representing seeds (or unprocessed seed ranges for part2)
        /// </param>
        /// <param name="maps">The array of lists representing the conversions for each map
        /// </param>
        public void ProcessInput(string inputName, out List<double> Seeds, out List<(double, double, double)>[] maps)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);

            Seeds = new();
            maps = new List<(double, double, double)>[7];

            //Initialize each list in maps
            for (int i = 0; i < 7; i++)
            {
                maps[i] = new List<(double, double, double)>();
            }

            //Each map is a number from 0-6, with -1 representing the seeds
            int currMap = -1;
            for (int i = 0; i < RawInput.Length; i++)
            {
                string currLine = RawInput[i];
                if (i == 0)
                {
                    //For just the first line, cut out "seeds: " and turn the numbers into a list of ints
                    Seeds = currLine.Substring(7).Split(' ').Select(double.Parse).ToList();
                }
                else if (currLine.Length == 0)
                {
                    //Change the index of the currentMap when given an empty line
                    currMap++;
                }
                //Check if not a label, then is map values
                else if (char.IsDigit(currLine[0]))
                {
                    List<double> values = currLine.Split(' ').Select(double.Parse).ToList();
                    maps[currMap].Add((values[0], values[1], values[2]));
                }
            }
        }

        /// <summary>
        /// Takes a seed value and goes through the maps to convert it into a location
        /// </summary>
        /// <param name="maps"> The maps that hold the conversions for the seeds
        /// </param>
        /// <param name="seed"> The seed value to be converted
        /// </param>
        /// <returns>
        /// The location mapped to the given seed value
        /// </returns>
        public static double getSeedValue(List<(double, double, double)>[] maps, double seed)
        {
            //Sort (descending) the lists by the source range start value
            Array.ForEach(maps, list => list.Sort(new SEComparer()));

            double nextValue = seed;
            for (int currMap = 0; currMap < maps.Length; currMap++)
            {
                for (int j = 0; j < maps[currMap].Count; j++)
                {
                    if (nextValue < maps[currMap][j].Item2)
                    {
                        continue;
                    }
                    else if (nextValue < maps[currMap][j].Item2 + maps[currMap][j].Item3)
                    {
                        nextValue = maps[currMap][j].Item1 + nextValue - maps[currMap][j].Item2;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return nextValue;
        }

        /// <summary>
        /// Recursive function that continuously splits ranges of values and runs them backwards through the maps
        /// </summary>
        /// <param name="seeds"> The ranges of valid seeds
        /// </param>
        /// <param name="maps"> The maps that hold the conversions for the seeds
        /// </param>
        /// <param name="currMap"> The current layer of the maps array
        /// </param>
        /// <param name="rangeToCheck"> The range of values to process with lower-level maps
        /// </param>
        /// <returns></returns>
        static double RecursiveCheck(List<(double, double)> seeds, List<(double, double, double)>[] maps, int currMap, (double, double, double) rangeToCheck) 
        {
            //If currMap < 0, we are at the seed layer and see if any of the seeds in our range are valid
            //If so, it will be the smallest location value
            if (currMap < 0)
            {
                foreach ((double, double) seed in seeds) 
                {
                    if(rangeToCheck.Item2 + rangeToCheck.Item3 < seed.Item1)
                    {
                        return -1; //Past the range of needed seeds
                    }
                    //Find the maximum of the starting points
                    double start = Math.Max(rangeToCheck.Item2, seed.Item1);

                    //Find the minimum of the end points
                    double end = Math.Min(rangeToCheck.Item2 + rangeToCheck.Item3, seed.Item1 + seed.Item2);

                    //Check if there is an intersection
                    if (start <= end)
                    {
                        return start; //Smallest number 🎉
                    }
                }
                return -1;
                //Made it to the end, means no seeds was found and to continue
            }
            while (rangeToCheck.Item3 > 0)
            {
                foreach ((double, double, double) range in maps[currMap])
                {
                    if (rangeToCheck.Item2 >= range.Item1 && rangeToCheck.Item2 <= range.Item1 + range.Item3)
                    {
                        //Destination range start difference
                        double DESD = rangeToCheck.Item2 - range.Item1;
                        //The quantity to split off for the newRange
                        double usedQuantity = Math.Min(rangeToCheck.Item3, range.Item3 - DESD);
                        //This'll be used for more recursion
                        (double, double, double) newRange = (rangeToCheck.Item2, range.Item2 + DESD, usedQuantity);
                        double recursiveResult = RecursiveCheck(seeds, maps, currMap - 1, newRange);
                        if(recursiveResult != -1) 
                        {
                            return recursiveResult;
                        }
                        //Update rangeToCheck to split off into other sections
                        rangeToCheck = (rangeToCheck.Item1+usedQuantity, rangeToCheck.Item2+usedQuantity, rangeToCheck.Item3-usedQuantity);
                    }
                }
            }
            return -1;
        }
        
        /// <summary>
        /// This method guarentees that the range of the maps ranges goes from 0-double.MaxValue
        /// </summary>
        /// <param name="list"> The List of ranges for each map, unsorted
        /// </param>
        static void CheckAndModifyMaps(List<(double, double, double)> list)
        {
            //Sort the list by the First Element of the tuple (ascending)
            list.Sort(new FEComparer());
            
            //If there is no range covering the first numbers (0,1,2,3,...), add it
            if (list[0].Item1 != 0.0)
            {
                list.Insert(0, (0.0, 0.0, list[0].Item1));
            }
            /*
             * This section checks that the 1st and 3rd element of each tuple in the list
             * Is equal to the 1st element of the next tuple in the list
             * If not, that means there is a gap in the range of numbers, which we fill
            */
            for (int i = 0; i < list.Count - 1; i++)
            {
                double sum = list[i].Item1 + list[i].Item3;
                double nextElementValue = list[i + 1].Item1;

                if (sum != nextElementValue)
                {
                    //Insert the new tuple inbetween
                    list.Insert(i + 1, (sum, sum, list[i + 1].Item1 - sum));
                }
            }
            //Add a section to handle the last numbers in the range to the maximum double (this essentually give us the full range of numbers to handle)
            double highestValue = list[list.Count - 1].Item1 + list[list.Count - 1].Item3;
            list.Add((highestValue, highestValue, double.MaxValue));
        }
    }

    /// <summary>
    /// Custom comparer to sort 3-element tuples based on the second element (Sorts in decsending order)
    /// </summary>
    public class SEComparer : IComparer<(double, double, double)>
    {
        public int Compare((double, double, double) x, (double, double, double) y)
        {
            return y.Item2.CompareTo(x.Item2);
        }
    }

    /// <summary>
    /// Custom comparer to sort 3-element tuples based on the first element (Sorts in ascending order)
    /// </summary>
    public class FEComparer : IComparer<(double, double, double)>
    {
        public int Compare((double, double, double) x, (double, double, double) y)
        {
            return x.Item1.CompareTo(y.Item1);
        }
    }

    /// <summary>
    /// Custom comparer to sort 2-element tuples based on the first element (Sorts in ascending order)
    /// </summary>
    public class FEComparer2 : IComparer<(double, double)>
    {
        public int Compare((double, double) x, (double, double) y)
        {
            return x.Item1.CompareTo(y.Item1);
        }
    }

}
