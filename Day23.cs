using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day23
    {
        Dictionary<char, (int, int)> DirectionMap = new Dictionary<char, (int, int)>()
        {
            {'^',(-1,0)},
            {'v',(1,0)},
            {'<',(0,-1)},
            {'>',(0,1)},
        };

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
            String[] TrailMap = System.IO.File.ReadAllLines(inputName);

            (int r, int c) startingNode = (0, 1);
            (int r, int c) endNode = (TrailMap.Length - 1, TrailMap[0].Length - 2);

            long steps = 0;

            PriorityQueue<(int, int, int, (int, int)), int> frontier = new();

            (int r, int c, int d, (int r, int c) prevNode) currNode;

            frontier.Enqueue((startingNode.r, startingNode.c, 0, (0,0)), 0);
            while (frontier.Count != 0)
            {
                currNode = frontier.Dequeue();
                if (currNode.r == endNode.r && currNode.c == endNode.c)
                {
                    steps = Math.Max(steps, currNode.d);
                    continue;
                }
                foreach((int dr, int dc) in DirectionMap.Values)
                {
                    int r = currNode.r + dr;
                    int c = currNode.c + dc;
                    if (r < 0 || r >= TrailMap.Length || c < 0 || c >= TrailMap[0].Length || (currNode.prevNode.r == r && currNode.prevNode.c == c) ||TrailMap[r][c] == '#')
                    {
                        continue;
                    }
                    if (TrailMap[r][c] != '.' && DirectionMap[TrailMap[r][c]] != (dr,dc))
                    {
                        continue;
                    }
                    frontier.Enqueue((r, c, currNode.d + 1, (currNode.r, currNode.c)), currNode.d + 1);
                }
            }
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
            String[] TrailMap = System.IO.File.ReadAllLines(inputName);

            Dictionary<(int r, int c), Dictionary<(int r, int c), int>> nodeDistanceMap = new();

            (int r, int c) startingNode = (0, 1);
            (int r, int c) endNode = (TrailMap.Length - 1, TrailMap[0].Length - 2);

            long steps = 0;

            HashSet<(int, int)> visitedSet = new();
            Stack<(int, int, int, (int, int))> frontier = new();

            frontier.Push((startingNode.r, startingNode.c, 0, startingNode));
            while (frontier.Count != 0)
            {
                (int r, int c, int d, (int r, int c) decisionNode) currNode = frontier.Pop();
                if (visitedSet.Contains((currNode.r, currNode.c)))
                {
                    continue;
                }
                visitedSet.Add((currNode.r, currNode.c));
                foreach ((int dr, int dc) in DirectionMap.Values)
                {
                    int r = currNode.r + dr;
                    int c = currNode.c + dc;
                    (int r, int c) decisionNode = currNode.decisionNode;
                    int pathLength = currNode.d + 1;
                    if (r < 0 || r >= TrailMap.Length || c < 0 || c >= TrailMap[0].Length || TrailMap[r][c] == '#')
                    {
                        continue;
                    }
                    int numDirs = 0;
                    foreach ((int cr, int cc) in DirectionMap.Values)
                    {
                        int ur = r + cr;
                        int uc = c + cc;
                        if (ur < 0 || ur >= TrailMap.Length || uc < 0 || uc >= TrailMap[0].Length)
                        {
                            continue;
                        }
                        char trailChar = TrailMap[ur][uc];
                        if (trailChar != '.' && trailChar != '#')
                        {
                            numDirs++;
                        }
                    }
                    if (((r, c) == (endNode.r, endNode.c) || numDirs > 1) && currNode.decisionNode != (r, c))
                    {
                        decisionNode = (r, c);
                        if (!nodeDistanceMap.ContainsKey(currNode.decisionNode))
                        {
                            nodeDistanceMap.Add(currNode.decisionNode, new());
                        }
                        nodeDistanceMap[currNode.decisionNode][decisionNode] = pathLength;
                        if (!nodeDistanceMap.ContainsKey(decisionNode))
                        {
                            nodeDistanceMap.Add(decisionNode, new());
                        }
                        nodeDistanceMap[decisionNode][currNode.decisionNode] = pathLength;
                        pathLength = 0;
                    }
                    frontier.Push((r, c, pathLength, decisionNode));
                }
            }

            Queue<((int, int), int, HashSet<(int, int)>)> nodeFrontier = new();
            nodeFrontier.Enqueue((startingNode, 0, new()));
            while (nodeFrontier.Count != 0)
            {
                ((int r, int c) loc, int d, HashSet<(int, int)> visitedNodes) currNode = nodeFrontier.Dequeue();
                if (currNode.loc == endNode)
                {
                    steps = Math.Max(steps, currNode.d);
                    continue;
                }
                foreach ((int r, int c) adjacentNode in nodeDistanceMap[currNode.loc].Keys)
                {
                    if (currNode.visitedNodes.Contains(adjacentNode))
                    {
                        continue;
                    }
                    HashSet<(int, int)> visitedNodes = new HashSet<(int, int)>(currNode.visitedNodes);
                    visitedNodes.Add(currNode.loc);
                    ((int, int), int, HashSet<(int, int)>) newNode = (adjacentNode, currNode.d + nodeDistanceMap[currNode.loc][adjacentNode], visitedNodes);
                    nodeFrontier.Enqueue(newNode);
                }
            }

            return steps.ToString();
        }
    }
}
