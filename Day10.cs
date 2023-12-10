using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace AdventOfCode2023
{
    internal class Day10
    {
        public string Part1(string inputName)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);
            GetLoopVertices(RawInput, out int steps);

            return steps.ToString();
        }

        public string Part2(string inputName)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);

            List<(int, int)> polygonVertices = GetLoopVertices(RawInput, out int steps);

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

        public List<(int,int)> GetLoopVertices(String[] map, out int steps)
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

        public ((int, int), (int, int)) FindAdjacentPipes(String[] map, (int,int) position)
        {
            if(position.Item1 < 0 || position.Item2 < 0 || position.Item1 >= map.Length || position.Item2 >= map[0].Length)
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
                        for(int j = -1; j <= 1; j++)
                        {
                            if (i == j)
                            {
                                continue;
                            }
                            ((int, int), (int, int)) adjacentPipes = FindAdjacentPipes(map, (position.Item1 + i, position.Item2 + j));
                            if(adjacentPipes.Item1 == position || adjacentPipes.Item2 == position)
                            {
                                if(sAdjacentPipes.Item1 == (-1, -1))
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
