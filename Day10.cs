using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day10
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
            char[][] pipeMap = RawInput.Select(s => s.ToCharArray()).ToArray();
            GetLoopVertices(pipeMap, out int steps);

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
            char[][] pipeMap = RawInput.Select(s => s.ToCharArray()).ToArray();
            List<(int, int)> polygonVertices = GetLoopVertices(pipeMap, out int steps);
            int num = 0;
            for (int i = 0; i < pipeMap.Length; i++)
            {
                bool isIn = false;
                double pipeVal = 0;
                for (int j = 0; j < pipeMap[i].Length; j++)
                {
                    if (polygonVertices.Contains((i, j)))
                    {
                        switch (pipeMap[i][j])
                        {
                            case '|':
                                isIn = !isIn;
                                break;
                            case '-':
                                break;
                            case 'L':
                            case '7':
                                pipeVal -= 0.5;
                                break;
                            case 'J':
                            case 'F':
                                pipeVal += 0.5;
                                break;
                        }
                        if(Math.Abs(pipeVal) == 1)
                        {
                            isIn = !isIn;
                            pipeVal = 0;
                        }
                    }
                    else if(isIn)
                    {
                        num++;
                    }
                }
            }
            return num.ToString();
        }

        /// <summary>
        /// Searches through the loop in order to get all of the loop positions
        /// </summary>
        /// <param name="map"> The map of all the pipes
        /// </param>
        /// <param name="steps"> The number of steps needed to go halfway through the loop
        /// </param>
        /// <returns>
        /// The list of all the loop pipe positions
        /// </returns>
        public List<(int, int)> GetLoopVertices(char[][] map, out int steps)
        {
            (int, int) startPos = (-1,-1);

            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    if (map[i][j] == 'S')
                    {
                        startPos = (i, j);
                        map[i][j] = WhatIsS((i, j), map);
                        break;
                    }
                }
                if(startPos != (-1, -1)) 
                { 
                    break; 
                }
            }

            List<(int, int)> polygonVertices = new List<(int, int)>();
            polygonVertices.Add(startPos);

            //Arbitarily choose to grab the first one
            (int, int) prevPosDir = startPos;
            (int, int) nextPosDir = FindAdjacentPipes(map, startPos).Item1;
            steps = 1;
            while (nextPosDir != startPos)
            {
                polygonVertices.Add(nextPosDir);
                ((int, int), (int, int)) DirAdj = FindAdjacentPipes(map, nextPosDir);
                (int, int) tempPrev = nextPosDir;
                nextPosDir = DirAdj.Item1 != prevPosDir ? DirAdj.Item1 : DirAdj.Item2;
                prevPosDir = tempPrev;
                steps++;
            }
            steps /= 2;

            return polygonVertices;
        }

        /// <summary>
        /// Finds the adjacent pipes
        /// </summary>
        /// <param name="map"> The map of all pipes
        /// </param>
        /// <param name="position"> The position of the pipe to find the adjecent of
        /// </param>
        /// <returns>
        /// The adjacent pipe locations
        /// </returns>
        public ((int, int), (int, int)) FindAdjacentPipes(char[][] map, (int, int) position)
        {
            if (position.Item1 < 0 || position.Item2 < 0 || position.Item1 >= map.Length || position.Item2 >= map[0].Length)
            {
                return ((-1, -1), (-1, -1));
            }
            switch (map[position.Item1][position.Item2])
            {
                case '|':
                    return ((position.Item1 + 1, position.Item2), (position.Item1 - 1, position.Item2));
                case '-':
                    return ((position.Item1, position.Item2 + 1), (position.Item1, position.Item2 - 1));
                case 'L':
                    return ((position.Item1 - 1, position.Item2), (position.Item1, position.Item2 + 1));
                case 'J':
                    return ((position.Item1 - 1, position.Item2), (position.Item1, position.Item2 - 1));
                case '7':
                    return ((position.Item1 + 1, position.Item2), (position.Item1, position.Item2 - 1));
                case 'F':
                    return ((position.Item1 + 1, position.Item2), (position.Item1, position.Item2 + 1));
            }
            return ((-1, -1), (-1, -1));
        }

        /// <summary>
        /// Finds what kind of pipe S is
        /// </summary>
        /// <param name="position"> The position of S
        /// </param>
        /// <param name="map"> The map of all pipes
        /// </param>
        /// <returns>
        /// The char representing the pipe S is
        /// </returns>
        public char WhatIsS((int, int) position, char[][] map)
        {
            ((int, int), (int, int)) sAdjacentPipes = ((-2, -2), (-2, -2));
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    ((int, int), (int, int)) adjacentPipes = FindAdjacentPipes(map, (position.Item1 + i, position.Item2 + j));
                    if (adjacentPipes.Item1 == position || adjacentPipes.Item2 == position)
                    {
                        if (sAdjacentPipes.Item1 == (-2, -2))
                        {
                            sAdjacentPipes.Item1 = (i, j);
                        }
                        else
                        {
                            sAdjacentPipes.Item2 = (i, j);
                            switch (sAdjacentPipes)
                            {
                                case ((0, -1), (-1, 0)):
                                case ((-1, 0), (0, -1)):
                                    return 'J';
                                case ((0, -1), (0, 1)):
                                case ((0, 1), (0, -1)):
                                    return '-';
                                case ((0, -1), (1, 0)):
                                case ((1, -0), (1, 0)):
                                    return '7';
                                case ((-1, 0), (0, 1)):
                                case ((0, 1), (-1, 0)):
                                    return 'L';
                                case ((-1, 0), (1, 0)):
                                case ((1, 0), (-1, 0)):
                                    return '|';
                                case ((0, 1), (1, 0)):
                                case ((1, 0), (0, 1)):
                                    return 'F';
                            }
                        }
                    }
                }
            }
            return '@';
        }




        //Not gonna heavily document this section

        public string Part2Orig(string inputName)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);

            List<(int, int)> polygonVertices = GetLoopVerticesOrig(RawInput, out int steps);

            int num = 0;
            for (int i = 0; i < RawInput.Length; i++)
            {
                for (int j = 0; j < RawInput[i].Length; j++)
                {
                    if (IsPointInPolygon((i, j), polygonVertices))
                    {
                        num++;
                    }
                }
            }
            return num.ToString();
        }

        public List<(int, int)> GetLoopVerticesOrig(String[] map, out int steps)
        {
            (int, int) startPos = new();

            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    if (map[i][j] == 'S')
                    {
                        startPos = (i, j);
                        break;
                    }
                }
            }

            List<(int, int)> polygonVertices = new List<(int, int)>();
            polygonVertices.Add(startPos);

            //Arbitarily choose to grab the first one
            (int, int) prevPosDir = startPos;
            (int, int) nextPosDir = FindAdjacentPipes(map, startPos).Item1;
            steps = 1;
            while (nextPosDir != startPos)
            {
                if (map[nextPosDir.Item1][nextPosDir.Item2] != '|' && map[nextPosDir.Item1][nextPosDir.Item2] != '-')
                {
                    polygonVertices.Add(nextPosDir);
                }
                ((int, int), (int, int)) DirAdj = FindAdjacentPipes(map, nextPosDir);
                (int, int) tempPrev = nextPosDir;
                nextPosDir = DirAdj.Item1 != prevPosDir ? DirAdj.Item1 : DirAdj.Item2;
                prevPosDir = tempPrev;
                steps++;
            }
            steps /= 2;

            return polygonVertices;
        }

        public ((int, int), (int, int)) FindAdjacentPipes(String[] map, (int, int) position)
        {
            if (position.Item1 < 0 || position.Item2 < 0 || position.Item1 >= map.Length || position.Item2 >= map[0].Length)
            {
                return ((-1, -1), (-1, -1));
            }
            switch (map[position.Item1][position.Item2])
            {
                case '|':
                    return ((position.Item1 + 1, position.Item2), (position.Item1 - 1, position.Item2));
                case '-':
                    return ((position.Item1, position.Item2 + 1), (position.Item1, position.Item2 - 1));
                case 'L':
                    return ((position.Item1 - 1, position.Item2), (position.Item1, position.Item2 + 1));
                case 'J':
                    return ((position.Item1 - 1, position.Item2), (position.Item1, position.Item2 - 1));
                case '7':
                    return ((position.Item1 + 1, position.Item2), (position.Item1, position.Item2 - 1));
                case 'F':
                    return ((position.Item1 + 1, position.Item2), (position.Item1, position.Item2 + 1));
                case 'S':
                    ((int, int), (int, int)) sAdjacentPipes = ((-1, -1), (-1, -1));
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (i == j)
                            {
                                continue;
                            }
                            ((int, int), (int, int)) adjacentPipes = FindAdjacentPipes(map, (position.Item1 + i, position.Item2 + j));
                            if (adjacentPipes.Item1 == position || adjacentPipes.Item2 == position)
                            {
                                if (sAdjacentPipes.Item1 == (-1, -1))
                                {
                                    sAdjacentPipes.Item1 = (position.Item1 + i, position.Item2 + j);
                                }
                                else
                                {
                                    sAdjacentPipes.Item2 = (position.Item1 + i, position.Item2 + j);
                                    return sAdjacentPipes;
                                }
                            }
                        }
                    }
                    return sAdjacentPipes;
            }
            return ((-1, -1), (-1, -1));
        }

        public bool IsPointInPolygon((int, int) point, List<(int, int)> polygon)
        {
            if (polygon.Contains(point))
            {
                return false;
            }
            int intersectCount = 0;

            for (int i = 0; i < polygon.Count; i++)
            {
                int next = (i + 1) % polygon.Count;

                if ((polygon[i].Item2 > point.Item2) != (polygon[next].Item2 > point.Item2) &&
                    point.Item1 < (polygon[next].Item1 - polygon[i].Item1) * (point.Item2 - polygon[i].Item2) /
                                 (polygon[next].Item2 - polygon[i].Item2) + polygon[i].Item1)
                {
                    intersectCount++;
                }
            }

            return intersectCount % 2 == 1;
        }

    }
}
