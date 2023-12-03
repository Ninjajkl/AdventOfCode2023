using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day02
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
            int idSum = 0;
            int maxRed = 12;
            int maxGreen = 13;
            int maxBlue = 14;
            for (int i = 0; i < RawInput.Length; i++)
            {
                string game = RawInput[i];
                bool possibleGame = true;
                for(int j = 0; j < game.Length - 4; j++)
                {
                    if (char.IsDigit(game[j]))
                    {
                        int num = (int)char.GetNumericValue(game[j]);
                        if (char.IsDigit(game[j + 1]))
                        {
                            num *= 10;
                            num += (int)char.GetNumericValue(game[j+1]);
                            j++;
                        }
                        if ((game[j+2] == 'r' && num > maxRed) ||
                            (game[j+2] == 'g' && num > maxGreen) ||
                            (game[j+2] == 'b' && num > maxBlue))
                        {
                            possibleGame = false;
                            break;
                        }
                    }
                }
                if(possibleGame) 
                {
                    idSum += i+1;
                }
            }
            return idSum.ToString();
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
            int idSum = 0;
            for (int i = 0; i < RawInput.Length; i++)
            {
                string game = RawInput[i];
                int minRed = 0;
                int minGreen = 0;
                int minBlue = 0;
                for (int j = 0; j < game.Length - 4; j++)
                {
                    if (char.IsDigit(game[j]))
                    {
                        int num = (int)char.GetNumericValue(game[j]);
                        if (char.IsDigit(game[j + 1]))
                        {
                            num *= 10;
                            num += (int)char.GetNumericValue(game[j + 1]);
                            j++;
                        }
                        if (game[j + 2] == 'r' && num > minRed)
                        {
                            minRed = num;
                        }
                        else if (game[j + 2] == 'g' && num > minGreen)
                        {
                            minGreen = num;
                        }
                        else if (game[j + 2] == 'b' && num > minBlue)
                        {
                            minBlue = num;
                        }
                    }
                }
                idSum += minRed * minGreen * minBlue;
            }
            return idSum.ToString();
        }
    }
}
