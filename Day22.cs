using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day22
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
            Dictionary<int,HashSet<Brick>> BrickDictionary = ProcessInput(inputName);
            
            List<int> brickZList = BrickDictionary.Keys.ToList();

            DropBricks(brickZList, BrickDictionary);

            long numDisintegratable = 0;
            foreach (int zkey in brickZList)
            {
                foreach (Brick brick in BrickDictionary[zkey])
                {
                    if (zkey != brick.LowestPoint)
                    {
                        continue;
                    }
                    bool disintegratable = true;
                    foreach(Brick aboveBrick in brick.AboveBricks)
                    {
                        if(aboveBrick.BelowBricks.Count == 1) 
                        {
                            disintegratable = false;
                            break;
                        }
                    }
                    numDisintegratable += disintegratable ? 1 : 0;
                }
            }

            return numDisintegratable.ToString();
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
            Dictionary<int, HashSet<Brick>> BrickDictionary = ProcessInput(inputName);

            List<int> brickZList = BrickDictionary.Keys.ToList();

            DropBricks(brickZList, BrickDictionary);

            long fallSum = 0;
            foreach (int zkey in brickZList)
            {
                foreach (Brick brick in BrickDictionary[zkey])
                {
                    if (zkey != brick.LowestPoint)
                    {
                        continue;
                    }
                    fallSum += brick.GetHoldStrength(BrickDictionary, brickZList, zkey);
                }
            }

            return fallSum.ToString();
        }

        /// <summary>
        /// Converts the given ranges into a map of bricks
        /// </summary>
        /// <param name="inputName"> The address of the input file to take input from
        /// </param>
        /// <returns>
        /// The map of all the bricks
        /// </returns>
        public Dictionary<int, HashSet<Brick>> ProcessInput(string inputName)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);

            Dictionary<int, HashSet<Brick>> BrickDictionary = new();

            foreach (string input in RawInput)
            {
                string firstH = input.Split('~')[0];
                string secondH = input.Split('~')[1];
                int[] rangeStartArr = firstH.Split(',').Select(int.Parse).ToArray();
                int[] rangeEndArr = secondH.Split(',').Select(int.Parse).ToArray();
                Brick newBrick = new Brick(rangeStartArr, rangeEndArr);

                if (!BrickDictionary.ContainsKey(newBrick.LowestPoint))
                {
                    BrickDictionary[newBrick.LowestPoint] = new HashSet<Brick>();
                }
                if (!BrickDictionary.ContainsKey(newBrick.HighestPoint))
                {
                    BrickDictionary[newBrick.HighestPoint] = new HashSet<Brick>();
                }
                BrickDictionary[newBrick.LowestPoint].Add(newBrick);
                BrickDictionary[newBrick.HighestPoint].Add(newBrick);
            }
            return BrickDictionary;
        }
    
        /// <summary>
        /// Moves each brick in the map as far as they can go
        /// </summary>
        /// <param name="brickZList"> The list of each z-value in the map with a Brick top/bottom
        /// </param>
        /// <param name="BrickDictionary"> The map of the bricks
        /// </param>
        public void DropBricks(List<int> brickZList, Dictionary<int, HashSet<Brick>> BrickDictionary)
        {
            brickZList.Sort();
            foreach (int zkey in brickZList)
            {
                HashSet<Brick> fallSet = new HashSet<Brick>(BrickDictionary[zkey]);
                foreach (Brick brick in fallSet)
                {
                    brick.Brickdrop(BrickDictionary, zkey);
                }
            }
        }
    }

    public class Brick
    {
        public ((int x, int y, int z) start, (int x, int y, int z) end) Range;
        public int HighestPoint => Math.Max(Range.start.z, Range.end.z);
        public int LowestPoint => Math.Min(Range.start.z, Range.end.z);

        public HashSet<Brick> AboveBricks = new();
        public HashSet<Brick> BelowBricks = new();

        /// <summary>
        /// The constructor for a Brick
        /// </summary>
        /// <param name="rangeStartArr"> The beginning range values
        /// </param>
        /// <param name="rangeEndArr"> The end range values
        /// </param>
        public Brick(int[] rangeStartArr, int[] rangeEndArr)
        {
            (int x, int y, int z) rangeStart = (rangeStartArr[0], rangeStartArr[1], rangeStartArr[2]);
            (int x, int y, int z) rangeEnd = (rangeEndArr[0], rangeEndArr[1], rangeEndArr[2]);
            Range = (rangeStart, rangeEnd);
        }

        /// <summary>
        /// Drops the brick as far as it can in the map and notes what it lands on
        /// </summary>
        /// <param name="BrickDictionary"> The map of the bricks
        /// </param>
        /// <param name="zkey"> The starting z-value of the lowest point of the brick
        /// </param>
        public void Brickdrop(Dictionary<int, HashSet<Brick>> BrickDictionary, int zkey)
        {
            //If the key is the brick's highest point, then it has already fell
            if(zkey != LowestPoint)
            {
                return;
            }
            (int x, int y, int z) newStart = Range.start;
            (int x, int y, int z) newEnd = Range.end;
            for(int newZ = zkey; newZ > 0; newZ--)
            {
                newStart = (newStart.x, newStart.y, newStart.z - 1);
                newEnd = (newEnd.x, newEnd.y, newEnd.z - 1);
                bool overlap = false;
                if (BrickDictionary.ContainsKey(newZ - 1))
                {
                    foreach (Brick otherBrick in BrickDictionary[newZ - 1])
                    {
                        if (otherBrick.Brickoverlap((newStart, newEnd)))
                        {
                            overlap = true;
                            otherBrick.AboveBricks.Add(this);
                            BelowBricks.Add(otherBrick);
                        }
                    }
                }
                if (overlap || newZ == 1)
                {
                    //Move the brick in the map from the old position to the new one
                    BrickDictionary[HighestPoint].Remove(this);
                    BrickDictionary[LowestPoint].Remove(this);
                    Range = ((newStart.x, newStart.y, newStart.z + 1), (newEnd.x, newEnd.y, newEnd.z + 1));

                    if (!BrickDictionary.ContainsKey(LowestPoint))
                    {
                        BrickDictionary[LowestPoint] = new HashSet<Brick>();
                    }
                    if (!BrickDictionary.ContainsKey(HighestPoint))
                    {
                        BrickDictionary[HighestPoint] = new HashSet<Brick>();
                    }
                    BrickDictionary[LowestPoint].Add(this);
                    BrickDictionary[HighestPoint].Add(this);
                    break;
                }
            }
        }

        /// <summary>
        /// Calculates if a brick is overlapping this one
        /// </summary>
        /// <param name="otherBrickRange"> The range of the other brick
        /// </param>
        /// <returns>
        /// If a brick is overlapping this one
        /// </returns>
        public bool Brickoverlap(((int x, int y, int z) start, (int x, int y, int z) end) otherBrickRange)
        {
            bool overlapX = OverlapsInDimension(Range.start.x, Range.end.x, otherBrickRange.start.x, otherBrickRange.end.x);
            bool overlapY = OverlapsInDimension(Range.start.y, Range.end.y, otherBrickRange.start.y, otherBrickRange.end.y);
            bool overlapZ = OverlapsInDimension(Range.start.z, Range.end.z, otherBrickRange.start.z, otherBrickRange.end.z);

            return overlapX && overlapY && overlapZ;
        }

        /// <summary>
        /// Checks if two ranges in the same dimension are overlapping
        /// </summary>
        /// <param name="start1"> The start value of the first range
        /// </param>
        /// <param name="end1"> The end value of the first range
        /// </param>
        /// <param name="start2"> The start value of the second range
        /// </param>
        /// <param name="end2"> The end value of the second range
        /// </param>
        /// <returns>
        /// If the ranges overlap
        /// </returns>
        private bool OverlapsInDimension(int start1, int end1, int start2, int end2)
        {
            return Math.Max(start1,start2) <= Math.Min(end1,end2);
        }

        /// <summary>
        /// Gets the number of bricks that would fall if this one was disintegrated
        /// </summary>
        /// <param name="BrickDictionary"> The map of the bricks
        /// </param>
        /// <param name="brickZList"> The list of each z-value in the map with a Brick top/bottom
        /// </param>
        /// <param name="zkey"> The starting z-value of the lowest point of the brick
        /// </param>
        /// <returns></returns>
        public int GetHoldStrength(Dictionary<int, HashSet<Brick>> BrickDictionary, List<int> brickZList, int zkey)
        {
            HashSet<Brick> collapsedBricks = new();
            collapsedBricks.Add(this);
            foreach (int zk in brickZList)
            {
                if(zk <= zkey)
                {
                    continue;
                }
                foreach(Brick nextBrick in BrickDictionary[zk])
                {
                    if (zk != nextBrick.LowestPoint)
                    {
                        continue;
                    }
                    if (nextBrick.BelowBricks.IsSubsetOf(collapsedBricks))
                    {
                        collapsedBricks.Add(nextBrick);
                    }
                }
            }
            return collapsedBricks.Count-1;
        }
    }
}
