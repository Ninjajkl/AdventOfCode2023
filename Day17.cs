using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day17
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

            LavaNode lastNode = LavaAStar(RawInput, (0, 0), (RawInput.Length - 1, RawInput[0].Length - 1));

            return lastNode.HeatLoss.ToString();
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

            LavaNode lastNode = LavaAStar(RawInput, (0, 0), (RawInput.Length - 1, RawInput[0].Length - 1), true);

            return lastNode.HeatLoss.ToString();
        }

        /// <summary>
        /// AStar for the magma crucible
        /// </summary>
        /// <param name="lavaMap"> The map of magma
        /// </param>
        /// <param name="startLocation"> The starting location of the search
        /// </param>
        /// <param name="endLocation"> The goal location of the search
        /// </param>
        /// <param name="ULTRACRUCIBLE"> Whether the crucible is an ULTRACRUCIBLE
        /// </param>
        /// <returns></returns>
        public LavaNode LavaAStar(String[] lavaMap, (int,int) startLocation, (int,int) endLocation, bool ULTRACRUCIBLE = false)
        {
            HashSet<((int,int),(int,int), int)> visitedSet = new();
            PriorityQueue<LavaNode, int> frontier = new();

            LavaNode startingNode = new LavaNode(startLocation, 0, endLocation,(0,1),0,null);
            frontier.Enqueue(startingNode, startingNode.FVal);
            while (frontier.Count != 0) 
            {
                LavaNode currLNode = frontier.Dequeue();
                if (visitedSet.Contains((currLNode.Location, currLNode.Direction, currLNode.DistInDirection)))
                {
                    continue;
                }
                visitedSet.Add((currLNode.Location, currLNode.Direction, currLNode.DistInDirection));
                if (currLNode.Location.Item1 == endLocation.Item1 && currLNode.Location.Item2 == endLocation.Item2 && (!ULTRACRUCIBLE || currLNode.DistInDirection >= 4))
                {
                    return currLNode;
                }
                frontier.EnqueueRange(GenerateFrontier(lavaMap, currLNode, endLocation, visitedSet, ULTRACRUCIBLE).Select(tuple => (tuple.Item1, tuple.Item2)));
            }
            return null;
        }

        /// <summary>
        /// Generates the frontier for the current LavaNode
        /// </summary>
        /// <param name="lavaMap"> The map of magma
        /// </param>
        /// <param name="currLNode"> The current Laval Node
        /// </param>
        /// <param name="endLocation">The goal location of the search
        /// </param>
        /// <param name="visitedTiles"> The HashSet of visited tiles
        /// </param>
        /// <param name="ULTRACRUCIBLE"> Whether the crucible is an ULTRACRUCIBLE
        /// </param>
        /// <returns>
        /// The found list of valid frontiers, as well as their priorities
        /// </returns>
        public List<(LavaNode, int)> GenerateFrontier(String[] lavaMap, LavaNode currLNode, (int,int) endLocation, HashSet<((int, int), (int, int), int)> visitedTiles, bool ULTRACRUCIBLE)
        {
            List<(LavaNode, int)> localFrontier = new();
            (int, int) currLocation = currLNode.Location;

            List<(int, int)> possibleDirs = new();
            if (currLNode.DistInDirection >= 4 || currLNode.Location == (0, 0) || !ULTRACRUCIBLE)
            {
                possibleDirs.Add((currLNode.Direction.Item2, currLNode.Direction.Item1));
                possibleDirs.Add((-currLNode.Direction.Item2, -currLNode.Direction.Item1));
            }

            if (currLNode.DistInDirection < 10 && ULTRACRUCIBLE || currLNode.DistInDirection < 3)
            {
                possibleDirs.Add(currLNode.Direction);
            }

            foreach( (int r, int c) in possibleDirs)
            {
                (int, int) locC = (r + currLocation.Item1, c + currLocation.Item2);
                int newDistInDirection = (r, c) == currLNode.Direction ? currLNode.DistInDirection + 1 : 1;
                if (visitedTiles.Contains((locC, (r,c), newDistInDirection)) || locC.Item1 < 0 || locC.Item1 >= lavaMap.Length || locC.Item2 < 0 || locC.Item2 >= lavaMap[0].Length)
                {
                    continue;
                }
                LavaNode newLNode = new LavaNode(locC, currLNode.HeatLoss + lavaMap[locC.Item1][locC.Item2] - '0', endLocation, (r, c), newDistInDirection, currLNode);
                localFrontier.Add((newLNode, newLNode.FVal));
            }
            return localFrontier;
        }
    }

    public class LavaNode
    {
        public (int, int) Location;
        public int HeatLoss;
        public int HVal;
        public int FVal => HeatLoss+ HVal*2;
        public (int, int) Direction;
        public int DistInDirection;
        public LavaNode? PrevNode;

        /// <summary>
        /// The constructor for the LavaNode
        /// </summary>
        /// <param name="location"> The location of the node
        /// </param>
        /// <param name="heatLoss"> The amount of heatLoss to this node
        /// </param>
        /// <param name="goalLocation"> The target location of the search
        /// </param>
        /// <param name="direction"> The direction of this node
        /// </param>
        /// <param name="distInDirection"> How long have been going in the same direction
        /// </param>
        /// <param name="prevNode"> The previous node in the search
        /// </param>
        public LavaNode((int, int) location, int heatLoss, (int,int) goalLocation, (int, int) direction, int distInDirection, LavaNode prevNode)
        {
            Location = location;
            HeatLoss = heatLoss;
            HVal = Math.Abs(location.Item1 - goalLocation.Item1) + Math.Abs(location.Item2 - goalLocation.Item2);
            Direction = direction;
            DistInDirection = distInDirection;
            PrevNode = prevNode;
        }
    }
}
