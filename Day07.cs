using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day07
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

            (string, int)[] hands = RawInput.Select(input => (input.Split(' ')[0], int.Parse(input.Split(' ')[1]))).ToArray();

            Array.Sort(hands, new CardComparer());

            return hands.Select((tuple, index) => (index + 1) * tuple.Item2).Sum().ToString();
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

            (string, int)[] hands = RawInput.Select(input => (input.Split(' ')[0], int.Parse(input.Split(' ')[1]))).ToArray();

            Array.Sort(hands, new JokerComparer());

            return hands.Select((tuple, index) => (index + 1) * tuple.Item2).Sum().ToString();
        }
    }

    public class CardComparer : IComparer<(string, int)>
    {
        public int Compare((string, int) x, (string, int) y)
        {
            int xType = GetType(x.Item1);
            int yType = GetType(y.Item1);

            //If x has higher type
            if(xType < yType)
            {
                return 1;
            }
            //If y has higher type
            if(xType > yType)
            {
                return -1;
            }

            //Otherwise, compare per-value
            string rankOrder = "AKQJT98765432";

            for (int i = 0; i < Math.Min(x.Item1.Length, y.Item1.Length); i++)
            {
                int compareResult = rankOrder.IndexOf(x.Item1[i]).CompareTo(rankOrder.IndexOf(y.Item1[i]));

                if (compareResult != 0)
                {
                    return -compareResult;
                }
            }

            return 0;
        }

        public int GetType(string hand)
        {
            //Groups each char by value
            var groups = hand.GroupBy(c => c);
            //If there is only 1 group, then all of the chars are the same
            //Five of a kind
            if (groups.Count() == 1)
            {
                return 1;
            }
            //Checks if there is a group of size 4
            //Four of a kind
            else if (groups.Any(group => group.Count() == 4))
            {
                return 2;
            }
            //Checks if there is a group of size 3
            else if (groups.Any(group => group.Count() == 3))
            {
                //Check if there is a group of size 2 too
                //Full house
                if (groups.Any(group => group.Count() == 2))
                {
                    return 3;
                }
                //Otherwise, three of a kind
                return 4;
            }
            //Check if there are exactly two groups of size 2
            //Two pair
            else if (groups.Count(group => group.Count() == 2) == 2)
            {
                return 5;
            }
            //Check if there are not 5 groups (means at least 1 pair)
            //One Pair
            else if (groups.Count() != 5)
            {
                return 6;
            }
            //Otherwise, no pairs
            //High card
            return 7;
        }
    }

    public class JokerComparer : IComparer<(string, int)>
    {
        public int Compare((string, int) x, (string, int) y)
        {
            int xType = GetType(x.Item1);
            int yType = GetType(y.Item1);

            //If x has higher type
            if (xType < yType)
            {
                return 1;
            }
            //If y has higher type
            if (xType > yType)
            {
                return -1;
            }

            //Otherwise, compare per-value
            string rankOrder = "AKQT98765432J";

            for (int i = 0; i < Math.Min(x.Item1.Length, y.Item1.Length); i++)
            {
                int compareResult = rankOrder.IndexOf(x.Item1[i]).CompareTo(rankOrder.IndexOf(y.Item1[i]));

                if (compareResult != 0)
                {
                    return -compareResult;
                }
            }

            return 0;
        }

        public int GetType(string hand)
        {
            //Groups each char by value
            var groups = hand.GroupBy(c => c);

            // Find the 'J' group, if it exists
            var jGroup = groups.FirstOrDefault(group => group.Key == 'J');
            //If there is no J, then use normal
            if (jGroup == null)
            {
                //If there is only 1 group, then all of the chars are the same
                //Five of a kind
                if (groups.Count() == 1)
                {
                    return 1;
                }
                //Checks if there is a group of size 4
                //Four of a kind
                else if (groups.Any(group => group.Count() == 4))
                {
                    return 2;
                }
                //Checks if there is a group of size 3
                else if (groups.Any(group => group.Count() == 3))
                {
                    //Check if there is a group of size 2 too
                    //Full house
                    if (groups.Any(group => group.Count() == 2))
                    {
                        return 3;
                    }
                    //Otherwise, three of a kind
                    return 4;
                }
                //Check if there are exactly two groups of size 2
                //Two pair
                else if (groups.Count(group => group.Count() == 2) == 2)
                {
                    return 5;
                }
                //Check if there are not 5 groups (means at least 1 pair)
                //One Pair
                else if (groups.Count() != 5)
                {
                    return 6;
                }
                //Otherwise, no pairs
                //High card
                return 7;
            }
            //Otherwise...
            else
            {
                int jSize = jGroup.Count();
                //If there is only 1 group, then all of the chars J
                //OR, if there are 2 groups, with one of them being the 'J's, then the J's pretend to be the other group
                //Five of a kind
                if (groups.Count() == 1 || groups.Count() == 2)
                {
                    return 1;
                }
                //Checks if there is a group of size 4
                //OR if there is a group of size 3
                //OR if there are two group of size 2, with one of the groups being the j's
                //Four of a kind
                else if (groups.Any(group => group.Count() == 3) ||
                    (groups.Count(group => group.Count() == 2) == 2 && jSize == 2))
                {
                    return 2;
                }
                //The only sitiuation you can get a full house with a J is a 2-pair with a single J
                else if (groups.Count(group => group.Count() == 2) == 2 && jSize == 1)
                {
                    return 3;
                }
                //Either 1 J and a pair, or 2 Js and no other pairs
                //Three of a kind
                else if (groups.Any(group => group.Count() == 2) && jSize == 1 || 
                    (groups.Count(group => group.Count() == 2) == 1) && jSize == 2)
                {
                    return 4;
                }
                //Two-Pair not possible
                //Must be at least 1 J, so High card not possible
                //That leaves One Pair for everything else
                return 6;
            }
        }
    }
}
