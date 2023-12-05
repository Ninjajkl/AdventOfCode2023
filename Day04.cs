using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode2023
{
    internal class Day04
    {
        public string Part1(string inputName)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);

            int sum = 0;
            foreach (string card in RawInput) 
            {
                //Add the 
                sum += (int)Math.Pow(2, NumCardMatches(card) - 1);
            }
            return sum.ToString();
        }

        public string Part2(string inputName)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);

            //For the keys of 1 through RawInput.length, set the value to 1 (as we have 1 card for each at the start)
            Dictionary<int, int> cardDictationary = Enumerable.Range(1, RawInput.Length).ToDictionary(key => key, value => 1);
            for (int i = 1; i <= RawInput.Length; i++)
            {
                string card = RawInput[i-1];
                for (int j = 1; j <= NumCardMatches(card); j++)
                {
                    cardDictationary[i + j] += cardDictationary[i];
                }
            }
            return cardDictationary.Values.Sum().ToString();
        }

        public int NumCardMatches(string card)
        {
            const string winningPattern = @"(?<=:)\s*(\d+(?:\s+\d+)*)";
            const string myNumbersPattern = @"(?<=\|)\s*(\d+(?:\s+\d+)*)";

            List<int> winningList = Regex.Match(card, winningPattern).Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            List<int> myNumbersList = Regex.Match(card, myNumbersPattern).Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            return winningList.Intersect(myNumbersList).Count();
        }
    }
}
