using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day12
    {
        Dictionary<string, long> founds = new();

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

            List<(string, List<int>)> damagedSprings = RawInput.Select(input => (input.Split(' ')[0], input.Split(' ')[1].Split(',').Select(int.Parse).ToList())).ToList();

            long sum = 0;
            foreach (var dm in damagedSprings)
            {
                sum += optRec(dm.Item1, dm.Item2);
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

            List<(string, List<int>)> damagedSprings = RawInput.Select(input => (string.Join("?", Enumerable.Repeat(input.Split(' ')[0], 5)),
            Enumerable.Repeat(input.Split(' ')[1].Split(',').Select(int.Parse).ToList(), 5).SelectMany(x => x).ToList())).ToList();

            long sum = 0;
            foreach (var dm in damagedSprings)
            {
                sum += optRec(dm.Item1, dm.Item2);
            }

            return sum.ToString();
        }

        /// <summary>
        /// The original recursive function
        /// </summary>
        /// <param name="possField"> The full string of springs
        /// </param>
        /// <param name="index"> The index of the next spring to check
        /// </param>
        /// <param name="count"> The number of arrangements
        /// </param>
        /// <param name="deciSprings"> The list of decimal broken springs
        /// </param>
        /// <param name="deciIndex"> The index of the current decimal broken spring
        /// </param>
        /// <returns>
        /// The number of arrangements
        /// </returns>
        public long recursiveFunction(string possField, int index, int count, List<int> deciSprings, int deciIndex)
        {
            if (count > 0)
            {
                if (deciIndex >= deciSprings.Count || count > deciSprings[deciIndex])
                {
                    return 0;
                }
            }
            if (index >= possField.Length)
            {
                if (count > 0 && count != deciSprings[deciIndex] ||
                    count == 0 && deciIndex < deciSprings.Count ||
                    deciIndex < deciSprings.Count - 1)
                {
                    return 0;
                }
                return 1;
            }
            switch (possField[index])
            {
                case '#':
                    count++;
                    return recursiveFunction(possField, index + 1, count, deciSprings, deciIndex);
                case '.':
                    if (count > 0)
                    {
                        if (deciIndex >= deciSprings.Count || deciSprings[deciIndex] != count)
                        {
                            return 0;
                        }
                        count = 0;
                        deciIndex++;
                    }
                    return recursiveFunction(possField, index + 1, count, deciSprings, deciIndex);
                case '?':
                    long brokeAmount = recursiveFunction(possField, index + 1, count + 1, deciSprings, deciIndex);
                    if (count > 0)
                    {
                        if (deciIndex >= deciSprings.Count || deciSprings[deciIndex] != count)
                        {
                            return brokeAmount;
                        }
                        count = 0;
                        deciIndex++;
                    }
                    return brokeAmount + recursiveFunction(possField, index + 1, count, deciSprings, deciIndex);
            }
            return -9999999999999999;
        }

        /// <summary>
        /// Recursive function that returns the number of arrangements left
        /// </summary>
        /// <param name="remainingSprings"> The string of remaining springs to check
        /// </param>
        /// <param name="decibrokeSprings"> The list of spring groups that still need to be found
        /// </param>
        /// <returns>
        /// The number of arrangements
        /// </returns>
        public long optRec(string remainingSprings, List<int> decibrokeSprings)
        {
            string foundsKey = $"{remainingSprings},{string.Join(',', decibrokeSprings)}";
            if (founds.TryGetValue(foundsKey, out long foundsVal))
            {
                return foundsVal;
            }
            while (true)
            {
                if(decibrokeSprings.Count == 0)
                {
                    long val = remainingSprings.Contains('#') ? 0 : 1;
                    founds[foundsKey] = val;
                    return val;
                }
                else if(remainingSprings.Length == 0)
                {
                    founds[foundsKey] = 0;
                    return 0;
                }
                switch (remainingSprings[0])
                {
                    case '.':
                        remainingSprings = remainingSprings.Trim('.');
                        continue;
                    case '?':
                        long val = optRec("." + remainingSprings.Substring(1), decibrokeSprings) + optRec("#" + remainingSprings.Substring(1), decibrokeSprings);
                        founds[foundsKey] = val;
                        return val;
                    case '#':
                        if (decibrokeSprings.Count == 0 ||
                        remainingSprings.Length < decibrokeSprings[0] ||
                        remainingSprings.Substring(0, decibrokeSprings[0]).Contains('.') ||
                        (decibrokeSprings.Count > 1 && (remainingSprings.Length < decibrokeSprings[0] + 1 || remainingSprings[decibrokeSprings[0]] == '#')))
                        {
                            founds[foundsKey] = 0;
                            return 0;
                        }
                        remainingSprings = decibrokeSprings.Count > 1 ? remainingSprings.Substring(decibrokeSprings[0] + 1) : remainingSprings.Substring(decibrokeSprings[0]);
                        decibrokeSprings = decibrokeSprings.Skip(1).ToList();
                        continue;
                }
            }
        }
    }
}
