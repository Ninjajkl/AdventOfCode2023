using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day08
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
            string directionString = RawInput[0];

            Dictionary<string, (string, string)> networkDictionary = new();

            for(int i = 2; i < RawInput.Length; i++)
            {
                networkDictionary.Add(RawInput[i].Substring(0, 3), (RawInput[i].Substring(7, 3), RawInput[i].Substring(12, 3)));
            }

            int steps = 0;
            string currNode = "AAA";
            do
            {
                currNode = directionString[steps%directionString.Length] == 'L' ? networkDictionary[currNode].Item1 : networkDictionary[currNode].Item2;
                steps++;
            } while (currNode != "ZZZ");

            return steps.ToString();
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
            string directionString = RawInput[0];
            List<string> startingNodes = new();

            Dictionary<string, (string, string)> networkDictionary = new();

            for (int i = 2; i < RawInput.Length; i++)
            {
                string key = RawInput[i].Substring(0, 3);
                (string, string) stepPair = (RawInput[i].Substring(7, 3), RawInput[i].Substring(12, 3));
                networkDictionary.Add(key, stepPair);
                //Add all of the "XXA" strings into a list for starting
                if (key[2] == 'A')
                {
                    startingNodes.Add(key);
                }
            }

            //List to hold the number of steps to get from A to Z for each starting node
            List<long> minMultiples = new();
            foreach (string key in startingNodes)
            {
                int steps = 0;
                string currNode = key;
                do
                {
                    currNode = directionString[steps % directionString.Length] == 'L' ? networkDictionary[currNode].Item1 : networkDictionary[currNode].Item2;
                    steps++;
                } while (currNode[2] != 'Z');
                minMultiples.Add(steps);
            }

            //To find where every ghost is on a Z, find the Least Common Multiple of the steps from A to Z for each Node
            return LCM(minMultiples).ToString();
        }

        //This LCM was originally written by Rodigo Lop
        //ez on Stack Overflow
        //http://stackoverflow.com/a/29717490/68936
        //I was going to write my own LCM calculator, but this one is perfect
        static long LCM(List<long> numbers)
        {
            return numbers.Aggregate(lcm);
        }
        static long lcm(long a, long b)
        {
            return Math.Abs(a * b) / GCD(a, b);
        }
        static long GCD(long a, long b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }
    }
}
