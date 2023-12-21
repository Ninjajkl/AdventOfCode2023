using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    internal class Day20
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
            Dictionary<string, BModule> moduleDict = ProcessInput(inputName);

            Queue<(string input, string destination, Pulse pulse)> pulseQueue = new();

            long lowCounter = 0;
            long highCounter = 0;
            for(int button = 1; button <= 1000; button++)
            {
                pulseQueue.Enqueue(("null","broadcaster", Pulse.Low));
                while(pulseQueue.Count > 0)
                {
                    (string input, string destination, Pulse pulse) = pulseQueue.Dequeue();
                    if (pulse == Pulse.High) highCounter++; 
                    else lowCounter++;
                    moduleDict[destination].HandlePulse(input, pulse).ForEach(pulseQueue.Enqueue);
                }
            }
            return (lowCounter*highCounter).ToString();
        }

        /// <summary>
        /// Computes the answer for Part 2
        /// </summary>
        /// <param name="inputName">The address of the input file to take input from
        /// </param>
        /// <returns>
        /// Results the result as a string to be printed
        /// </returns>
        /// <remarks>
        /// Part 1 was done entirely by me. I got stuck at Part 2 and looked at the forms for help
        /// I found u/Totherex's solution, which was remarkably similar to mine, who found the cycles pattern in the input I could not find
        /// All lines of code taken/inspired from u/Totherex is marked with an empty comment, all else was entirely mine
        /// His solution can be found at https://github.com/rtrinh3/AdventOfCode/blob/db566c04c2fb0be381e88ce23141d0b4356059be/Aoc2023/Day20.cs#L159
        /// </remarks>
        public string Part2(string inputName)
        {
            Dictionary<string, BModule> moduleDict = ProcessInput(inputName);
            Queue <(string input, string destination, Pulse pulse)> pulseQueue = new();
            string rxParent = moduleDict["rx"].inputs.Single().Key; //
            string[] rxGParents = moduleDict[rxParent].inputs.Keys.ToArray(); //
            Dictionary<string, long> cycles = new(); //
            for(long button = 1; button > 0; button++)
            {
                pulseQueue.Enqueue(("null", "broadcaster", Pulse.Low));
                while (pulseQueue.Count > 0)
                {
                    (string input, string destination, Pulse pulse) = pulseQueue.Dequeue();
                    if (destination == rxParent && pulse == Pulse.High) //
                    {
                        cycles.Add(input, button); //
                        if (rxGParents.All(cycles.ContainsKey)) //
                        {
                            return cycles.Values.Aggregate(1L, (acc, val) => acc * val).ToString(); //
                        }
                    }
                    moduleDict[destination].HandlePulse(input, pulse).ForEach(pulseQueue.Enqueue);
                }
            }


            return "broke out of impossible to break out of loop";
        }

        /// <summary>
        /// Converts the input into a series of Modules
        /// </summary>
        /// <param name="inputName"> The address of the input file to take input from
        /// </param>
        /// <returns>
        /// Dictionary<string, BModule> of all of the modules
        /// </returns>
        public Dictionary<string, BModule> ProcessInput(string inputName)
        {
            String[] RawInput = System.IO.File.ReadAllLines(inputName);
            Dictionary<string, BModule> moduleDict = new();
            foreach (string inp in RawInput)
            {
                char type = inp[0];
                BModule newModule;
                switch (type)
                {
                    case '%':
                        newModule = new FFModule(inp);
                        break;
                    case '&':
                        newModule = new CModule(inp);
                        break;
                    default:
                        newModule = new BModule(inp);
                        break;
                }
                if (moduleDict.ContainsKey(newModule.moduleName))
                {
                    newModule.AddInputs(moduleDict[newModule.moduleName].inputs);
                }
                moduleDict[newModule.moduleName] = newModule;
                foreach (string destination in newModule.destinations)
                {
                    if (moduleDict.ContainsKey(destination))
                    {
                        moduleDict[destination].AddInput(newModule.moduleName);
                    }
                    else
                    {
                        moduleDict.Add(destination, new BModule(destination, newModule.moduleName));
                    }
                }
            }
            return moduleDict;
        }
    }

    //Broadcast (base) module
    class BModule
    {
        public string moduleName;
        public char type;
        public Dictionary<string, Pulse> inputs;
        public List<string> destinations;

        /// <summary>
        /// Constructor used when not found yet, but is the destination of another module
        /// </summary>
        /// <param name="mName"> The name of the module
        /// </param>
        /// <param name="input"> A caller of the module
        /// </param>
        public BModule(string mName, string input)
        {
            moduleName = mName;
            type = 'b';
            inputs = new() { { input, Pulse.Low } };
            destinations = new();
        }

        /// <summary>
        /// Constructor used when full module found
        /// </summary>
        /// <param name="inp"> The raw input string to convert into a module
        /// </param>
        public BModule(string inp) 
        {
            type = inp[0];
            moduleName = inp[1..inp.IndexOf(' ')];
            if (type == 'b')
            {
                moduleName = 'b' + moduleName;
            }
            string destinationString = inp[(inp.IndexOf('>') + 2)..];
            destinations = destinationString.Split(',').Select(x => x.Trim()).ToList();
            inputs = new();
        }

        /// <summary>
        /// Handles the pulse as a broadcast module, overridable
        /// </summary>
        /// <param name="input"> The sender of the pulse
        /// </param>
        /// <param name="pulse"> The type of pulse received
        /// </param>
        /// <returns>
        /// A list of the pulses to send
        /// </returns>
        public virtual List<(string input, string destination, Pulse pulse)> HandlePulse(string input, Pulse pulse)
        {
            List<(string input, string destination, Pulse pulse)> newPulses = new();
            destinations.ForEach(dest => newPulses.Add((moduleName, dest, pulse)));
            return newPulses;
        }

        /// <summary>
        /// Adds a caller to the input
        /// </summary>
        /// <param name="inputName"> The caller's name
        /// </param>
        public void AddInput(string inputName)
        {
            inputs.Add(inputName, Pulse.Low);
        }

        /// <summary>
        /// Adds a list of callers to the input
        /// </summary>
        /// <param name="inps"> The callers' name
        /// </param>
        public void AddInputs(Dictionary<string, Pulse> inps)
        {
            inputs = new Dictionary<string, Pulse>(inps);
        }
    }

    //Flip-Flop module
    class FFModule : BModule
    {
        private bool isOn;

        /// <summary>
        /// Constructor used when full Flip-Flop module found
        /// </summary>
        /// <param name="inp"> The raw input string to convert into a module
        /// </param>
        public FFModule(string inp) : base(inp)
        {
            isOn = false;
        }

        /// <summary>
        /// Handles the pulse as a Flip-Flop module
        /// </summary>
        /// <param name="input"> The sender of the pulse
        /// </param>
        /// <param name="pulse"> The type of pulse received
        /// </param>
        /// <returns>
        /// A list of the pulses to send
        /// </returns>
        public override List<(string input, string destination, Pulse pulse)> HandlePulse(string input, Pulse pulse)
        {
            List<(string input, string destination, Pulse pulse)> newPulses = new();
            if(pulse == Pulse.High)
            {
                return newPulses;
            }
            Pulse sendPulse = isOn ? Pulse.Low : Pulse.High;
            isOn = !isOn;
            destinations.ForEach(dest => newPulses.Add((moduleName, dest, sendPulse)));
            return newPulses;
        }
    }

    //Conjunction module
    class CModule : BModule
    {
        /// <summary>
        /// Constructor used when full Conjunction module found
        /// </summary>
        /// <param name="inp"> The raw input string to convert into a module
        /// </param>
        public CModule(string inp) : base(inp)
        {
        }

        /// <summary>
        /// Handles the pulse as a Conjunction module
        /// </summary>
        /// <param name="input"> The sender of the pulse
        /// </param>
        /// <param name="pulse"> The type of pulse received
        /// </param>
        /// <returns>
        /// A list of the pulses to send
        /// </returns>
        public override List<(string input, string destination, Pulse pulse)> HandlePulse(string input, Pulse pulse)
        {
            inputs[input] = pulse;
            List<(string input, string destination, Pulse pulse)> newPulses = new();
            Pulse sendPulse;
            if (inputs.Values.All(x => x == Pulse.High))
            {
                sendPulse = Pulse.Low;
            }
            else
            {
                sendPulse = Pulse.High;
            }

            destinations.ForEach(dest => newPulses.Add((moduleName, dest, sendPulse)));
            return newPulses;
        }
    }

    public enum Pulse
    {
        High,
        Low
    }
}
