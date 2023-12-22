using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day21
    {
        (int, int)[] directions = { (-1, 0), (1, 0), (0, 1), (0, -1) };

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
            (int r, int c)  startingPos = (RawInput.Length/2, RawInput[0].Length/2);
            return GardenPlotsFinder(RawInput, startingPos, 64).ToString();
        }

        /// <summary>
        /// Computes the answer for Part 1
        /// </summary>
        /// <param name="inputName">The address of the input file to take input from
        /// </param>
        /// <returns>
        /// Results the result as a string to be printed
        /// </returns>
        public string Part2(string inputName)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);
            Dictionary<(int r, int c), ((int r, int c) pos, (int steps, int odd, int even) stepsToFill)> startingMapPos = new();
            int startR = RawInput.Length-1;
            for (int r = -1; r <= 1; r++)
            {
                int startC = RawInput[0].Length-1;
                for(int c = -1; c <= 1; c++)
                {
                    startingMapPos[(r,c)] = ((startR, startC), StepsToFillPlot(RawInput,(startR, startC)));
                    startC -= RawInput[0].Length / 2;
                }
                startR -= RawInput.Length / 2;
            }

            long plots = 0;
            long steps = 26501365;

            //Straight Section
            long stepsAfterFirstStraight = steps - (RawInput.Length / 2 + 1);
            long straightRemainingSteps = stepsAfterFirstStraight % RawInput[0].Length;
            long straightPrevRemainingSteps = straightRemainingSteps + RawInput[0].Length;
            long straightNumSectionsFilled = stepsAfterFirstStraight / RawInput[0].Length - 1;
            List<(int r,int c)> straightDirLists = new List<(int r, int c)>() 
            {
                {(0,-1)},
                {(0,1)},
                {(1,0)},
                {(-1,0)}
            };
            foreach((int r, int c) in straightDirLists)
            {
                plots += GardenPlotsFinder(RawInput, startingMapPos[(r, c)].pos, straightRemainingSteps);
                if(stepsAfterFirstStraight >= RawInput[0].Length)
                {
                    plots += GardenPlotsFinder(RawInput, startingMapPos[(r, c)].pos, straightPrevRemainingSteps);
                }
            }

            long numEven;
            long numOdd;
            if(steps % 2 != 0)
            {
                numEven = Math.Max(0,straightNumSectionsFilled) / 2;
                numOdd = Math.Max(0, straightNumSectionsFilled) - numEven;
            }
            else
            {
                numOdd = Math.Max(0, straightNumSectionsFilled) / 2;
                numEven = Math.Max(0, straightNumSectionsFilled) - numOdd;
            }
            plots += startingMapPos[(1, 0)].stepsToFill.odd * numOdd * 4;
            plots += startingMapPos[(1, 0)].stepsToFill.even * numEven * 4;

            //Diagonal Section
            long stepsAfterFirstCorner = steps - (RawInput.Length + 1);
            long diagonalRemainingSteps = stepsAfterFirstCorner % (RawInput[0].Length * 2);
            long diagonalPrevRemainingSteps = diagonalRemainingSteps + RawInput[0].Length * 2;
            long diagonalNumSectionsFilled = stepsAfterFirstCorner / (RawInput[0].Length * 2) - 1;
            List<(int r, int c)> diagonalDirLists = new List<(int r, int c)>()
            {
                {(1,1)},
                {(1,-1)},
                {(-1,1)},
                {(-1,-1)}
            };
            foreach ((int r, int c) in diagonalDirLists)
            {
                plots += GardenPlotsFinder(RawInput, startingMapPos[(r, c)].pos, diagonalRemainingSteps) * (1 + (diagonalNumSectionsFilled + 1) * 2);
                if(stepsAfterFirstCorner >= RawInput[0].Length * 2)
                {
                    plots += GardenPlotsFinder(RawInput, startingMapPos[(r, c)].pos, diagonalPrevRemainingSteps) * (1 + diagonalNumSectionsFilled * 2);
                }
            }
            long diagonalPlots;
            if (steps % 2 != 0)
            {
                diagonalPlots = startingMapPos[(-1, -1)].stepsToFill.odd;
            }
            else
            {
                diagonalPlots = startingMapPos[(-1, -1)].stepsToFill.even;
            }
            plots += diagonalPlots * Math.Max(0, diagonalNumSectionsFilled) * Math.Max(0, diagonalNumSectionsFilled) * 4;

            //Other Diagonal Section
            long stepsAfterCorner = steps - (RawInput.Length * 2 + 1);
            long diagRemainingSteps = stepsAfterCorner % (RawInput[0].Length * 2);
            long diagPrevRemainingSteps = diagRemainingSteps + RawInput[0].Length * 2;
            long diagNumSectionsFilled = stepsAfterCorner / (RawInput[0].Length * 2) - 1;

            foreach ((int r, int c) in diagonalDirLists)
            {
                plots += GardenPlotsFinder(RawInput, startingMapPos[(r, c)].pos, diagRemainingSteps) * (2 + (diagNumSectionsFilled + 1) * 2);
                if(stepsAfterCorner >= RawInput.Length * 2)
                {
                    plots += GardenPlotsFinder(RawInput, startingMapPos[(r, c)].pos, diagPrevRemainingSteps) * (2 + diagNumSectionsFilled * 2);
                }
            }
            long diagPlots;
            if (steps % 2 != 0)
            {
                diagPlots = startingMapPos[(-1, -1)].stepsToFill.even;
            }
            else
            {
                diagPlots = startingMapPos[(-1, -1)].stepsToFill.odd;
            }
            plots += diagPlots * (Math.Max(0, diagNumSectionsFilled) * Math.Max(0, diagNumSectionsFilled) + Math.Max(0, diagNumSectionsFilled)) * 4;
            //Add center
            plots += GardenPlotsFinder(RawInput, startingMapPos[(0, 0)].pos, steps);
            return plots.ToString();
        }

        /// <summary>
        /// Finds the number of reachable from the given number of steps within the plot
        /// </summary>
        /// <param name="RawInput"> The map of the plots
        /// </param>
        /// <param name="startingPos"> The starting position within the plot
        /// </param>
        /// <param name="steps"> The number of steps to check
        /// </param>
        /// <returns>
        /// The number of reachable from the given number of steps within the plot
        /// </returns>
        public long GardenPlotsFinder(String[]RawInput, (int,int) startingPos, long steps)
        {
            int rL = RawInput.Length;
            int cL = RawInput[0].Length;

            long total = steps % 2 == 0 ? 1 : 0;
            Dictionary<(int r, int c), int> visitedDict = new();
            PriorityQueue<(int r, int c), int> frontier = new();
            visitedDict.Add(startingPos, 0);
            frontier.Enqueue(startingPos, 0);
            while (frontier.Count > 0)
            {
                frontier.TryDequeue(out (int r, int c) element, out int priority);
                if (priority >= steps)
                {
                    break;
                }
                foreach ((int r, int c) direction in directions)
                {
                    (int r, int c) = (element.r + direction.r, element.c + direction.c);
                    if (visitedDict.ContainsKey((r, c)) || r < 0 || r >= rL || c < 0 || c >= cL || RawInput[r][c] == '#')
                    {
                        continue;
                    }
                    total += (priority + 1) % 2 == steps % 2 ? 1 : 0;
                    visitedDict.Add((r, c), priority + 1);
                    frontier.Enqueue((r, c), priority + 1);
                }
            }
            return total;
        }

        /// <summary>
        /// Finds the number of steps needed to fill a plot
        /// </summary>
        /// <param name="RawInput"> The map of the plots
        /// </param>
        /// <param name="startingPos"> The starting position within the plot
        /// </param>
        /// <returns>
        /// The number of steps needed to fill a plot
        /// </returns>
        public (int steps, int odd, int even) StepsToFillPlot(String[] RawInput, (int, int) startingPos)
        {
            int rL = RawInput.Length;
            int cL = RawInput[0].Length;

            int steps = 0;
            Dictionary<(int r, int c), int> visitedDict = new();
            PriorityQueue<(int r, int c), int> frontier = new();
            visitedDict.Add(startingPos, 0);
            frontier.Enqueue(startingPos, 0);
            int totalodd = 0;
            int totaleven = 1;
            while (frontier.Count > 0)
            {
                frontier.TryDequeue(out (int r, int c) element, out int priority);
                foreach ((int r, int c) direction in directions)
                {
                    (int r, int c) = (element.r + direction.r, element.c + direction.c);
                    if (visitedDict.ContainsKey((r, c)) || r < 0 || r >= rL || c < 0 || c >= cL || RawInput[r][c] == '#')
                    {
                        continue;
                    };
                    if((priority + 1) % 2 == 0)
                    {
                        totaleven++;
                    }
                    else
                    {
                        totalodd++;
                    }

                    visitedDict.Add((r, c), priority + 1);
                    frontier.Enqueue((r, c), priority + 1);
                    steps = Math.Max(priority+1, steps);
                }
            }
            return (steps, totalodd, totaleven);
        }
    }
}
