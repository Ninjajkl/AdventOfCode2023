using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day16
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

            Dictionary<(int, int), List<(int, int)>> energizedDictionary = new();
            MoveBeam(RawInput, energizedDictionary, (0,-1), (0, 1));
            return (energizedDictionary.Keys.Count-1).ToString();
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

            int maxVal = 0;
            for(int i = 0; i < RawInput.Length; i++)
            {
                Dictionary<(int, int), List<(int, int)>> energizedDictionary = new();
                MoveBeam(RawInput, energizedDictionary, (i, -1), (0, 1));
                maxVal = Math.Max(maxVal, energizedDictionary.Keys.Count - 1);
                energizedDictionary = new();
                MoveBeam(RawInput, energizedDictionary, (i, RawInput.Length), (0, -1));
                maxVal = Math.Max(maxVal, energizedDictionary.Keys.Count - 1);
                energizedDictionary = new();
                MoveBeam(RawInput, energizedDictionary, (-1, i), (1, 0));
                maxVal = Math.Max(maxVal, energizedDictionary.Keys.Count - 1);
                energizedDictionary = new();
                MoveBeam(RawInput, energizedDictionary, (RawInput.Length, i), (-1, 0));
                maxVal = Math.Max(maxVal, energizedDictionary.Keys.Count - 1);
            }
            return maxVal.ToString();
        }

        /// <summary>
        /// Recursive function to move beam
        /// </summary>
        /// <param name="mirrorMap"> The Map of the mirrors
        /// </param>
        /// <param name="energizedDictionary"> The dictionary of visited nodes
        /// </param>
        /// <param name="location"> The location of the beam on the map
        /// </param>
        /// <param name="direction"> The direction the beam is moving
        /// </param>
        public void MoveBeam(String[] mirrorMap, Dictionary<(int, int), List<(int, int)>> energizedDictionary, (int, int) location, (int, int) direction)
        {
            if(energizedDictionary.ContainsKey(location))
            {
                if (energizedDictionary[location].Contains(direction))
                {
                    return;
                }
                energizedDictionary[location].Add(direction);
            }
            else
            {
                energizedDictionary.Add(location, new List<(int, int)>() {direction});
            }
            (int, int) newLocation = (location.Item1 + direction.Item1, location.Item2 + direction.Item2);

            if (newLocation.Item1 < 0 || newLocation.Item1 >= mirrorMap.Length || newLocation.Item2 < 0 || newLocation.Item2 >= mirrorMap[0].Length)
            {
                return;
            }

            char nextEncounter = mirrorMap[newLocation.Item1][newLocation.Item2];
            if (nextEncounter == '/')
            {
                (int, int) newDirection = (-direction.Item2,-direction.Item1);
                MoveBeam(mirrorMap, energizedDictionary, newLocation, newDirection);
                return;
            }
            else if (nextEncounter == '\\')
            {
                (int, int) newDirection = (direction.Item2, direction.Item1);
                MoveBeam(mirrorMap, energizedDictionary, newLocation, newDirection);
                return;
            }
            else if (nextEncounter == '.' ||
                nextEncounter == '|' && (direction == (-1, 0) || direction == (1, 0)) ||
                nextEncounter == '-' && (direction == (0, -1) || direction == (0, 1)))
            {
                MoveBeam(mirrorMap, energizedDictionary, newLocation, direction);
                return;
            }
            else
            {
                MoveBeam(mirrorMap, energizedDictionary, newLocation, (direction.Item2, direction.Item1));
                MoveBeam(mirrorMap, energizedDictionary, newLocation, (-direction.Item2, -direction.Item1));
                return;
            }
        }
    }
}
