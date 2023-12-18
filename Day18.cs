using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day18
    {
        Dictionary<char, (int, int)> DirectionMap = new Dictionary<char, (int, int)>()
        {
            {'U',(-1,0)},
            {'3',(-1,0)},
            {'D',(1,0)},
            {'1',(1,0)},
            {'L',(0,-1)},
            {'2',(0,-1)},
            {'R',(0,1)},
            {'0',(0,1)}
        };

        /// <summary>
        /// The original method for computing the answer for Part 1
        /// </summary>
        /// <param name="inputName">The address of the input file to take input from
        /// </param>
        /// <returns>
        /// Results the result as a string to be printed
        /// </returns>
        public string Part1Orig(string inputName)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);

            List<(int r, int c, int steps)> convertedInput = new();
            foreach (var input in RawInput)
            {
                (int, int) direction = DirectionMap[input[0]];
                int r = direction.Item1;
                int c = direction.Item2;
                int steps = int.Parse(input[2..4]);
                convertedInput.Add((r, c, steps));
            }

            Dictionary<int, List<int>> dugDict = new();
            int rLoc = 0;
            int cLoc = 0;
            foreach (var instruction in convertedInput)
            {
                for(int i = 0; i < instruction.steps; i++)
                {
                    rLoc += instruction.r;
                    cLoc += instruction.c;
                    if (!dugDict.ContainsKey(rLoc))
                    {
                        dugDict[rLoc] = new List<int>();
                    }
                    dugDict[rLoc].Add(cLoc);
                }
            }

            Queue<(int,int)> frontier = new Queue<(int,int)>();
            frontier.Enqueue((1, 1));
            //Floodfill into dictionary
            while(frontier.Count > 0 )
            {
                (int r,int c) currLoc = frontier.Dequeue();
                if (!dugDict.ContainsKey(currLoc.r))
                {
                    dugDict[currLoc.r] = new List<int>();
                }
                dugDict[currLoc.r].Add(currLoc.c);
                foreach((int r, int c) dir in DirectionMap.Values)
                {
                    (int r, int c) newLoc = (currLoc.r + dir.r, currLoc.c + dir.c);
                    if(frontier.Contains(newLoc) || dugDict.ContainsKey(newLoc.r) && dugDict[newLoc.r].Contains(newLoc.c))
                    {
                        continue;
                    }
                    frontier.Enqueue(newLoc);
                }
            }

            return dugDict.Values.Sum(innerDict => innerDict.Count).ToString();
        }

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

            List<(long r, long c, long steps)> convertedInput = new();
            foreach (var input in RawInput)
            {
                (int, int) direction = DirectionMap[input[0]];
                int r = direction.Item1;
                int c = direction.Item2;
                int steps = int.Parse(input[2..4]);
                convertedInput.Add((r, c, steps));
            }

            return CalculateArea(convertedInput).ToString();
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

            List<(long r, long c, long steps)> convertedInput = new();
            foreach (var input in RawInput)
            {
                string hexCode = input[4..].TrimStart()[2..^1];

                (long r, long c) direction = DirectionMap[hexCode[^1]];
                long steps = Convert.ToInt32(hexCode[..^1], 16);
                convertedInput.Add((direction.r, direction.c, steps));
            }
            return CalculateArea(convertedInput).ToString();
        }

        /// <summary>
        /// Calculates the area of a trench
        /// </summary>
        /// <param name="input"> The steps to dig the trench
        /// </param>
        /// <returns>
        /// The total area of the trench
        /// </returns>
        public long CalculateArea(List<(long r, long c, long steps)> input)
        {
            List<(long r, long c)> Verts = new();

            long rLoc = 0;
            long cLoc = 0;
            long numStraight = 0;
            foreach (var instruction in input)
            {
                rLoc += instruction.r * instruction.steps;
                cLoc += instruction.c * instruction.steps;
                numStraight += instruction.steps - 1;
                Verts.Add((rLoc, cLoc));
            }

            int numRight = Verts.Count / 2 + 2;
            int numLeft = Verts.Count / 2 - 2;

            int n = Verts.Count;

            long sum = 0;

            for(int i = 0; i < n; i++)
            {
                long r1 = Verts[i].r;
                long c1 = Verts[i].c;

                long r2 = Verts[(i+1)%n].r;
                long c2 = Verts[(i+1)%n].c;

                sum += r1 * c2 - r2 * c1;
            }
            return (long)(Math.Abs(sum) / 2 + numStraight * 0.5 + numRight * 0.75 + numLeft * 0.25);
        }
    }
}
