using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AdventOfCode2023
{
    class DayManager
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Run Everything? (y/n): ");
                string userInput = Console.ReadLine().ToLower();

                if (userInput != "y" && userInput != "")
                {
                    break; // Exit the loop if the user doesn't enter "y" or press Enter
                }

                RunAllDays();
            }

            while (true)
            {
                Console.Write("\nRun a Part? (y/n): ");
                string quitInput = Console.ReadLine().ToLower();
                if (quitInput != "y" && quitInput != "")
                {
                    return;
                }

                Console.Write("Give the day number to run (1-25): ");
                string userDayNumberInput = Console.ReadLine();
                if (!int.TryParse(userDayNumberInput, out int dayNumber))
                {
                    Console.WriteLine("Not a number.");
                    continue;
                }

                string dayToRun = $"Day{dayNumber:D2}";

                //Get the DayXX Type
                Type dayType = GetDayType(dayToRun);

                if (dayType == null)
                {
                    Console.WriteLine("Invalid day.");
                    continue;
                }

                while (true)
                {
                    Console.Write("What Part to run? (1,2): ");
                    string userPartInput = Console.ReadLine();

                    if (!RunDayPart(dayType, userPartInput))
                    {
                        Console.WriteLine($"Part{userPartInput} not found for {dayType.Name}.");
                    }

                    Console.Write("Run another Part for this day? (y/n): ");
                    quitInput = Console.ReadLine().ToLower();
                    if (quitInput != "y")
                    {
                        break;
                    }
                }
            }
        }

        static void RunAllDays()
        {
            //Get all types in the current assembly
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<Type> dayTypes = assembly.GetTypes()
                .Where(type => type.IsClass && type.Name.StartsWith("Day") && type.Name.Length == 5)
                .ToList();

            //Create instances and run methods
            foreach (Type dayType in dayTypes)
            {
                Console.WriteLine($"\n{dayType.Name}:");
                // Create an instance of the DayXX class
                object? dayInstance = Activator.CreateInstance(dayType);

                // Check for and invoke Part1 method with default string argument
                RunDayPart(dayType, "1", "Input");

                // Check for and invoke Part2 method with default string argument
                RunDayPart(dayType, "2", "Input");
            }
        }

        static Type GetDayType(string dayToRun)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetTypes()
                .FirstOrDefault(type => type.IsClass && type.Name.Equals(dayToRun));
        }

        static bool RunDayPart(Type dayType, string userPartInput)
        {
            // Check for and invoke Part1 or Part2 method based on user input
            MethodInfo? partMethod = dayType.GetMethod($"Part{userPartInput}", BindingFlags.Instance | BindingFlags.Public);
            if (partMethod != null)
            {
                string userStringArgument;
                do
                {
                    // Get user input for the string argument
                    Console.Write("Enter the Input Name: ");
                    userStringArgument = Console.ReadLine().Trim();

                    // Handle special values for the string argument
                    if (IsSpecialStringArgument(userStringArgument, out string specialValue))
                    {
                        userStringArgument = specialValue;
                    }
                    else
                    {
                        // Check if the specified file exists
                        string filePath = $"..\\..\\..\\Inputs\\{dayType.Name}{userStringArgument}.txt";
                        if (!File.Exists(filePath))
                        {
                            Console.WriteLine($"File not found: {filePath}. Please provide a valid input.");
                            userStringArgument = null; // Set to null to repeat the loop
                        }
                    }
                } while (userStringArgument == null);

                Console.WriteLine($"\n  Running Part{userPartInput}:");
                // Create an instance of the specified DayXX class
                object? dayInstance = Activator.CreateInstance(dayType);

                var stopwatchPart = Stopwatch.StartNew();
                var answer = partMethod.Invoke(dayInstance, new object[] { $"..\\..\\..\\Inputs\\{dayType.Name + userStringArgument}.txt" });
                stopwatchPart.Stop();
                Console.WriteLine($"\n    Result: {answer}");
                Console.WriteLine($"\n    Time Elapsed: {stopwatchPart.Elapsed.TotalSeconds} seconds");
                return true;
            }
            return false;
        }

        static bool RunDayPart(Type dayType, string userPartInput, string defaultStringArgument)
        {
            //Check for and invoke Part1 or Part2 method based on user input
            MethodInfo? partMethod = dayType.GetMethod($"Part{userPartInput}", BindingFlags.Instance | BindingFlags.Public);
            if (partMethod != null)
            {
                Console.WriteLine($"\n  Running Part{userPartInput}:");

                var stopwatchPart = Stopwatch.StartNew();
                var answer = partMethod.Invoke(Activator.CreateInstance(dayType), new object[] { $"..\\..\\..\\Inputs\\{dayType.Name + defaultStringArgument}.txt" });
                stopwatchPart.Stop();

                Console.WriteLine($"\n    Result: {answer}");
                Console.WriteLine($"\n    Time Elapsed: {stopwatchPart.Elapsed.TotalSeconds} seconds");
                return true;
            }
            return false;
        }

        static bool IsSpecialStringArgument(string userInput, out string specialValue)
        {
            specialValue = string.Empty;

            switch (userInput.ToLower())
            {
                case "i":
                case "":
                case "input":
                case "default":
                    specialValue = "Input";
                    return true;
                default:
                    return false;
            }
        }
    }
}
