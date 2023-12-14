using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day12
    {
        Dictionary<(string possField, int size, string brokeSpringString), long> founds = new();

        public string Part1(string inputName)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);

            List<(string, List<int>)> damagedSprings = RawInput.Select(input => (input.Split(' ')[0], input.Split(' ')[1].Split(',').Select(int.Parse).ToList())).ToList();

            long sum = 0;
            foreach (var dm in damagedSprings)
            {
                sum += recursiveFunction(dm.Item1, 0, 0, dm.Item2, 0);
            }
            return sum.ToString();
        }

        public string Part2(string inputName)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);

            List<(string, List<int>)> damagedSprings = RawInput.Select(input => (string.Join("?", Enumerable.Repeat(input.Split(' ')[0], 5)),
            Enumerable.Repeat(input.Split(' ')[1].Split(',').Select(int.Parse).ToList(), 5).SelectMany(x => x).ToList())).ToList();

            return (-1).ToString(); // For now
            long sum = 0;
            foreach (var dm in damagedSprings)
            {
                sum += recursiveFunction(dm.Item1, 0, 0, dm.Item2, 0);
            }

            return sum.ToString();
        }

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
    }
}
