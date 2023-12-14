using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day13
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
            List<String[]> input = InterpretInput(RawInput);

            int sum = 0;
            foreach (String[] mirrorMap in input)
            {
                int reflectionIndex = FindVerticalMirror(mirrorMap);
                if(reflectionIndex == -1)
                {
                    reflectionIndex = 100*FindHorizontalMirror(mirrorMap);
                }
                sum += reflectionIndex;
                //Console.WriteLine(sum);
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
            List<String[]> input = InterpretInput(RawInput);

            int sum = 0;
            foreach (String[] mirrorMap in input)
            {
                int reflectionIndex = FindVerticalSmudge(mirrorMap);
                if (reflectionIndex == -1)
                {
                    reflectionIndex = 100 * FindHorizontalSmudge(mirrorMap);
                }
                sum += reflectionIndex;
                //Console.WriteLine(sum);
            }

            return sum.ToString();
        }

        /// <summary>
        /// Converts the Input into separate maps of mirrors
        /// </summary>
        /// <param name="RawInput"> String[] that contains the raw maps of mirrors
        /// </param>
        /// <returns>
        /// List<String[]> that contains the list of mirror maps
        /// </returns>
        public List<String[]> InterpretInput(String[] RawInput)
        {
            List<string[]> result = new List<string[]>();
            List<string> currentSegment = new List<string>();

            foreach (var item in RawInput)
            {
                if (item.Length == 0)
                {
                    result.Add(currentSegment.ToArray());
                    currentSegment.Clear();
                }
                else
                {
                    currentSegment.Add(item);
                }
            }
            result.Add(currentSegment.ToArray());

            return result;
        }
    
        /// <summary>
        /// Find the Vertical reflection point
        /// </summary>
        /// <param name="mirrorMap"> The map to find the reflection point of
        /// </param>
        /// <returns>
        /// If failed, -1. Id succeeded, the reflection point value
        /// </returns>
        public int FindVerticalMirror(String[] mirrorMap)
        {
            for(int c = 1; c < mirrorMap[0].Length; c++)
            {
                bool isMirrorIndex = true;
                for(int r = 0; r < mirrorMap.Length; r++)
                {
                    string firstHalf = mirrorMap[r].Substring(0, c);
                    string secondHalf = mirrorMap[r].Substring(c);

                    string reversedFirst = new string(firstHalf.Reverse().ToArray());

                    int minLength = Math.Min(reversedFirst.Length, secondHalf.Length);
                    isMirrorIndex = reversedFirst.Substring(0, minLength).Equals(secondHalf.Substring(0, minLength));
                    if (!isMirrorIndex)
                    {
                        break;
                    }
                    if(r == mirrorMap.Length - 1)
                    {
                        return c;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Find the Horizontal reflection point
        /// </summary>
        /// <param name="mirrorMap"> The map to find the reflection point of
        /// </param>
        /// <returns>
        /// If failed, -1. Id succeeded, the reflection point value
        /// </returns>
        public int FindHorizontalMirror(String[] mirrorMap)
        {
            for (int r = 1; r < mirrorMap.Length; r++)
            {
                bool isMirrorIndex = true;
                for (int c = 0; c < mirrorMap[0].Length; c++)
                {
                    string firstHalf = new string(mirrorMap.Select(s => s[c]).Take(r).ToArray());
                    string secondHalf = new string(mirrorMap.Select(s => s[c]).Skip(r).ToArray());

                    string reversedFirst = new string(firstHalf.Reverse().ToArray());

                    int minLength = Math.Min(reversedFirst.Length, secondHalf.Length);

                    isMirrorIndex = reversedFirst.Substring(0, minLength).Equals(secondHalf.Substring(0, minLength));
                    if (!isMirrorIndex)
                    {
                        break;
                    }
                    if (c == mirrorMap[0].Length - 1)
                    {
                        return r;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Find the Vertical reflection point
        /// </summary>
        /// <param name="mirrorMap"> The map to find the smudged reflection point of
        /// </param>
        /// <returns>
        /// If failed, -1. Id succeeded, the smudged reflection point value
        /// </returns>
        public int FindVerticalSmudge(String[] mirrorMap)
        {
            for (int c = 1; c < mirrorMap[0].Length; c++)
            {
                bool isMirrorIndex = true;
                int numWrong = 0;
                for (int r = 0; r < mirrorMap.Length; r++)
                {
                    string firstHalf = mirrorMap[r].Substring(0, c);
                    string secondHalf = mirrorMap[r].Substring(c);

                    string reversedFirst = new string(firstHalf.Reverse().ToArray());

                    int minLength = Math.Min(reversedFirst.Length, secondHalf.Length);
                    isMirrorIndex = reversedFirst.Substring(0, minLength).Equals(secondHalf.Substring(0, minLength));
                    if (!isMirrorIndex)
                    {
                        numWrong++;
                        if(numWrong > 1)
                        {
                            break;
                        }
                    }
                    if (r == mirrorMap.Length - 1 && numWrong == 1)
                    {
                        return c;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Find the Horizontal reflection point
        /// </summary>
        /// <param name="mirrorMap"> The map to find the smudged reflection point of
        /// </param>
        /// <returns>
        /// If failed, -1. Id succeeded, the smudged reflection point value
        /// </returns>
        public int FindHorizontalSmudge(String[] mirrorMap)
        {
            for (int r = 1; r < mirrorMap.Length; r++)
            {
                bool isMirrorIndex = true;
                int numWrong = 0;
                for (int c = 0; c < mirrorMap[0].Length; c++)
                {
                    string firstHalf = new string(mirrorMap.Select(s => s[c]).Take(r).ToArray());
                    string secondHalf = new string(mirrorMap.Select(s => s[c]).Skip(r).ToArray());

                    string reversedFirst = new string(firstHalf.Reverse().ToArray());

                    int minLength = Math.Min(reversedFirst.Length, secondHalf.Length);

                    isMirrorIndex = reversedFirst.Substring(0, minLength).Equals(secondHalf.Substring(0, minLength));
                    if (!isMirrorIndex)
                    {
                        numWrong++;
                        if (numWrong > 1)
                        {
                            break;
                        }
                    }
                    if (c == mirrorMap[0].Length - 1 && numWrong == 1)
                    {
                        return r;
                    }
                }
            }
            return -1;
        }

    }
}
