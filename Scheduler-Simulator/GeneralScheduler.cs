using System;
using System.IO;
namespace InstructionScheduler 
{
    public abstract class GeneralScheduler: IScheduler
    {
        protected int[] registersReadFrom;
        protected int[] registersWrittenTo;
        protected List<string> instructions = [];
        protected Dictionary<int, int> waits = [];
        protected Dictionary<string, int> instructionStatus = [];
        protected int instructionsDone = 0;
        protected int fetches;
        public GeneralScheduler(int functionalUnitsNumber, int registerNumber)
        {
            registersWrittenTo = new int[registerNumber];
            registersReadFrom = new int[registerNumber];
            foreach(string instruction in instructions)
            {
                instructionStatus.Add(instruction,0);
            }
           //Set initial value for waiting times
            waits.Add(-1,1);

            fetches = functionalUnitsNumber;
        }
        public void ReadInstrcutions() 
        {
            Console.WriteLine("Please enter the path of the instructions file:");

            // Read file path from the user
            string filePath = Console.ReadLine();

            // Validate if the file exists
            if (File.Exists(filePath))
            {
                try
                {
                    // Read and display the file content
                    string rawInstructions = File.ReadAllText(filePath);
                    //Rempve bad chars
                    rawInstructions = rawInstructions.Replace("\r","");
                    rawInstructions = rawInstructions.Replace(" ","");
                    rawInstructions = rawInstructions.Trim();
                    rawInstructions = rawInstructions.Trim('\n');
                    instructions = [.. (rawInstructions.Split("\n"))];
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading the file: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("The specified file does not exist. Please check the path and try again.");
            }
        }
        public abstract void ScheduleInstructions();
        public Tuple<List<int>,char> DecodeInstruction(string instruction)
        {
            List<int> registerIndexes = new List<int>{};

            char operation = '\0';
            
            for(int i = 0; i < instruction.Length; i++)
            {
                if(instruction[i] == 'R')
                {
                    try
                        {
                            if(!int.TryParse(char.ToString(instruction[i+1]), out int registerIndex))
                            {
                                Console.WriteLine("Instruction Format Error!");
                                registerIndexes.Clear();
                                return new Tuple<List<int>, char>(registerIndexes,operation);
                            }
                            else
                            {
                                registerIndexes.Add(registerIndex);
                            }
                        }
                        catch(IndexOutOfRangeException)
                        {
                            Console.WriteLine("Instruction Format Error!");
                            registerIndexes.Clear();
                            return new Tuple<List<int>, char>(registerIndexes,operation);
                        }
                }
                else if(
                    instruction[i] == '+'||
                    instruction[i] == '-'||
                    instruction[i] == '*'||
                    instruction[i] == 'L'||
                    instruction[i] == 'S')
                {
                    operation = instruction[i];
                }
            }
            return new Tuple<List<int>, char>(registerIndexes, operation);
        }
        public void PrintCycle(int cycleIndex, string instruction, string instructionIndex, string instructionDoneIndex)
        {
            const int columnWidth = 15;

            // Adjust the instruction to fit the column width
            string formattedInstruction;
            if (instruction.Length > columnWidth)
            {
                // Truncate and append dots if too long
                formattedInstruction = instruction.Substring(0, columnWidth - 3) + "...";
            }
            else
            {
                // Pad with spaces if too short
                formattedInstruction = instruction.PadRight(columnWidth);
            }

            // Print formatted output
            string cycleResult = $"| Cycle: {cycleIndex,-5} | {instructionIndex,-5} | {formattedInstruction} | {instructionDoneIndex,-5} |";
            Console.WriteLine(cycleResult);
            Console.WriteLine(new string('-', cycleResult.Length));
        }

        public int SetWaitTime(char operation)
        {
            //Instruction is addition or substraction
            if(operation == '+'|| operation == '-')
            {
                return 2;
            }
            //Instruction is multiplication
            else if(operation == '*')
            {
                return 3;
            }
            //Instruction is load or store
            else if(operation == 'S'|| operation =='L')
            {
                return 3;
            }
            //Instruction weren't in appropiate format
            else
            {
                Console.WriteLine("Instruction Format Error!");
                return -1;
            }
        }
        public void DecreaseWaits(int cycle)
        {
            int freeWaits = 0;
            //First iteration
            if(waits.ContainsKey(-1))
            {
                freeWaits = waits[-1];
                waits.Clear();
                return;
            }
            //Regular iteration
            foreach(int key in waits.Keys)
            {
                //Decrease each wait
                waits[key] -= 1;
                if(waits[key] == 0)
                {
                    //Instruction done

                    //Increase number of instructions completed
                    instructionsDone += 1;
                    PrintCycle(cycle,"","",(key+1).ToString());
                }
                if(waits[key] == -1)
                {
                    //Registers ready for release
                    //Release 1 cycle after instructions are done
                    //to force next instruction to execute until next cycle

                    //Release registers used
                    Tuple<List<int>, char> decodedInstruction = DecodeInstruction(instructions[key]);
                    SetRegistersUsed(false, decodedInstruction.Item1);
                }
            }
            return;
        }

        public void SetRegistersUsed(bool operationType, List<int> registersUsed)
        {
            //Increase by default
            int change = 1;
            //Decrease if operationType == false
            if(!operationType)
            {
                change = -1;
            }
            registersWrittenTo[registersUsed[0]] += change;
            if(registersUsed.Count > 1)
            {
                for(int i = 0; i < registersUsed.Count; i++)
                {
                    registersReadFrom[registersUsed[i]] += change;
                }
            }
        }
    }
}