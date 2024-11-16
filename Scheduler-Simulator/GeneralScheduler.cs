using System;
using System.IO;
namespace InstructionScheduler 
{
    public abstract class GeneralScheduler: IScheduler
    {
        protected string[] instructions = new string[]{};

        public void ReadInstrcutions(string path) 
        {
            string rawInstructions = File.ReadAllText(path);
            instructions = rawInstructions.Split("\n");
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
        public void PrintCycle(int cycleIndex, string instruction, int instructionIndex, int instructionDoneIndex)
        {
            int paddingSize = 16 - instruction.Length;
            string formattedInstruction;
            if(paddingSize < 0)
            {
                instruction = instruction.Substring(0,instruction.Length-(Math.Abs(paddingSize)*2));
                formattedInstruction = String.Concat(instruction,Enumerable.Repeat(".",Math.Abs(paddingSize)));
            }
            else
            {
                formattedInstruction = String.Concat(instruction,Enumerable.Repeat(" ", paddingSize));
            }
            Console.WriteLine($"| Cycle:{cycleIndex} | {instructionIndex} |{formattedInstruction} | {instructionDoneIndex} |");
        }
    }
}