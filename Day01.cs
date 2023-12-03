using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day01
    {

        /// <summary>
        /// Dictionary to convert from word to digit for Part 2
        /// </summary>
        Dictionary<string, char> wordToDigitMap = new Dictionary<string, char>
        {
            {"one", '1'},
            {"two", '2'},
            {"three", '3'},
            {"four", '4'},
            {"five", '5'},
            {"six", '6'},
            {"seven", '7'},
            {"eight", '8'},
            {"nine", '9'}
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
            String[] RawInput = System.IO.File.ReadAllLines(inputName);
            int sum = 0;
            foreach (String Input in RawInput)
            {
                string stringNum = "";
                int index;
                for(index = 0; index <= Input.Length; index++)
                {
                    char value = Input[index];
                    if (char.IsDigit(value))
                    {
                        stringNum += value;
                        break;
                    }
                }
                for (index = Input.Length-1; index >= 0; index--)
                {
                    char value = Input[index];
                    if (char.IsDigit(value))
                    {
                        stringNum += value;
                        break;
                    }
                }
                sum += int.Parse(stringNum);
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
            int sum = 0;
            foreach (String Input in RawInput)
            {
                string stringNum = "";
                int index;
                for (index = 0; index <= Input.Length; index++)
                {
                    char value = CheckForNumber(Input, index);
                    if(value != '0')
                    {
                        stringNum += value;
                        break;
                    }
                }
                for (index = Input.Length - 1; index >= 0; index--)
                {
                    char value = CheckForNumber(Input, index);
                    if (value != '0')
                    {
                        stringNum += value;
                        break;
                    }
                }
                sum += int.Parse(stringNum);
            }
            return sum.ToString();
        }

        /// <summary>
        /// Checks for a digit in the specified substring of the input string.
        /// </summary>
        /// <param name="inputString">The input string to search for a digit.</param>
        /// <param name="index">The starting index in the input string.</param>
        /// <returns>
        /// Returns the digit as char if found. On failure, returns '0'.
        /// </returns>
        public char CheckForNumber(String inputString, int index)
        {

            //Check if the substring starts with an actual digit
            if (char.IsDigit(inputString[index]))
            {
                return inputString[index];
            }

            //Check if the substring starts with any typed digit
            foreach (var entry in wordToDigitMap)
            {
                if (inputString.Length-index >= entry.Key.Length && inputString.Substring(index, entry.Key.Length).Contains(entry.Key))
                {
                    return entry.Value;
                }
            }

            return '0';
        }
    }
}
