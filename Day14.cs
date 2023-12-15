using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day14
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

            int loadSum = 0;
            for (int c = 0; c < RawInput[0].Length; c++)
            {
                string column = new string(RawInput.Select(s => s[c]).ToArray());
                int rockStop = 0;
                for (int r = 0; r < RawInput.Length; r++)
                {
                    switch (RawInput[r][c])
                    {
                        case '#':
                            rockStop = r + 1;
                            break;
                        case 'O':
                            loadSum += RawInput.Length - rockStop;
                            rockStop++;
                            break;
                        default:
                            break;
                    }
                }
            }

            return loadSum.ToString();
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
            char[][] rockMap = RawInput.Select(s => s.ToCharArray()).ToArray();

            //PrintCharArray(rockMap);
            int loadSum = 0;
            int startIndex = 200;
            List<int> pattern = new();
            List<int> buffer = new();
            for (int n = 1; n <= 1000000000; n++)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int c = 0; c < rockMap[0].Length; c++)
                    {
                        string column = new string(rockMap.Select(s => s[c]).ToArray());
                        int rockStop = 0;
                        for (int r = 0; r < rockMap.Length; r++)
                        {
                            switch (rockMap[r][c])
                            {
                                case '#':
                                    rockStop = r + 1;
                                    break;
                                case 'O':
                                    rockMap[r][c] = '.';
                                    rockMap[rockStop][c] = 'O';
                                    rockStop++;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    rockMap = RotateArray(rockMap);
                }
                //PrintCharArray(rockMap);
                //Skip First 200 to allow looping pattern to be developed
                if(n >= 200)
                {
                    loadSum = 0;
                    for (int r = 0; r < rockMap.Length; r++)
                    {
                        foreach (char c in rockMap[r])
                        {
                            if (c == 'O')
                            {
                                loadSum += rockMap.Length - r;
                            }
                        }
                    }
                    if(pattern.Count < 4)
                    {
                        pattern.Add(loadSum);
                    }
                    else
                    {
                        if(buffer.Count == 4)
                        {
                            if (pattern.SequenceEqual(buffer))
                            {
                                int patternLength = n - startIndex - 4;

                                n = 1000000000 - ((1000000000 - n) % patternLength);
                            }
                            buffer.RemoveAt(0);
                        }
                        buffer.Add(loadSum);
                    }
                }
            }

            return loadSum.ToString();
        }
        
        /// <summary>
        /// Rotates a given char[][] by 90 degress clockwise
        /// </summary>
        /// <param name="array"> the char[][] map to rotate
        /// </param>
        /// <returns>
        /// the rotated char[][] map
        /// </returns>
        static char[][] RotateArray(char[][] array)
        {
            int rowSize = array.Length;
            int colSize = array[0].Length;

            char[][] rotatedArray = new char[rowSize][];
            for (int i = 0; i < rowSize; i++)
            {
                rotatedArray[i] = new char[colSize];
                for (int j = 0; j < colSize; j++)
                {
                    rotatedArray[i][j] = array[j][i];
                }

                rotatedArray[i] = rotatedArray[i].Reverse().ToArray();
            }

            return rotatedArray;
        }
    }
}
