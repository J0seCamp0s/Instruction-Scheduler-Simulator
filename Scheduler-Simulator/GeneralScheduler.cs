using System;
using System.IO;
using System.Linq;
namespace InstructionScheduler 
{
    public abstract class GeneralScheduler: IScheduler
    {
        protected int[] registersReadFrom;
        protected int[] registersWrittenTo;
        //Item1 = temporary register index,
        //Item2 = normal register being renamed index
        protected List<Tuple<int, int>> renamingRules;  
        protected List<string> instructions = [];
        protected Dictionary<int, int> waits = [];
        protected Dictionary<string, int> instructionStatus = [];
        protected int instructionsDone = 0;
        protected int fetches;
        protected bool registerRenamingEnabled;
        public GeneralScheduler(int functionalUnitsNumber, int registerNumber)
        {
            registersWrittenTo = new int[registerNumber];
            registersReadFrom = new int[registerNumber];
            if(registerNumber > 8)
            {
                registerRenamingEnabled = true;
                renamingRules = [];
            }
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
        public Tuple<List<int>,char> DecodeInstruction(string instruction)
        {
            List<int> registerIndexes = new List<int>{};

            char operation = '\0';
            
            for(int i = 0; i < instruction.Length; i++)
            {
                if(instruction[i] == 'R'||instruction[i] == 'T')
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
                                //If regular register used
                                if(instruction[i] == 'R')
                                {
                                    registerIndexes.Add(registerIndex);
                                }
                                //If extra temporary regiter used
                                else
                                {
                                    registerIndexes.Add(registerIndex + 8);
                                }
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
            
            //First iteration
            if(waits.ContainsKey(-1))
            {
                waits.Clear();
                return;
            }
            List<int> keys = new List<int>(waits.Keys);
            keys.Sort();
            //Regular iteration
            foreach(int key in keys)
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
                    SetRegistersUsed(false, decodedInstruction);
                }
            }
            return;
        }

        public void SetRegistersUsed(bool operationType, Tuple<List<int>, char> decodedInstruction)
        {
            //Increase by default
            int change = 1;
            //Decrease if operationType == false
            if(!operationType)
            {
                change = -1;
            }
            //If instruction is Store instruction
            if(decodedInstruction.Item2 == 'S')
            {
                //Currently reading from register used
                registersReadFrom[decodedInstruction.Item1[0]] += change;
            }
            //Instruction is arithmetic operation or Load
            else
            {
                //Currently writting from register used
                registersWrittenTo[decodedInstruction.Item1[0]] += change;
            }
            //If arithmetic operation
            if(decodedInstruction.Item1.Count > 1)
            {
                //Set second and third registers as being read from
                for(int i = 1; i < decodedInstruction.Item1.Count; i++)
                {
                    registersReadFrom[decodedInstruction.Item1[i]] += change;
                }
            }
        }

        public void ScheduleInstructions()
        {
            int cycle = 0;

            Console.WriteLine("| Cycle # | Instruction Index | Instruction Scheduled | Instruction Done |");

            while(true) 
            {
                cycle ++;
                UpdateCycle(cycle);

                //If all instructions done
                if(instructionsDone == instructions.Count)
                {
                    //finish execution
                    break;
                }
                int availableFetches = fetches;
                //If instruction can be fetched
                FetchInstructions(availableFetches,cycle);
            }
            Console.WriteLine("Execution Done!");
            Console.WriteLine($"Total Number of Cycles: {cycle}");
        }

        public abstract void UpdateCycle(int cycle);
        public void FetchInstructions(int availableFetches, int cycle)
        {
            Tuple<List<int>, char> decodedInstruction;
            while(availableFetches > 0)
            {
                Tuple<int, int> selectedInstructionIndex = SelectInstruction();
                if(selectedInstructionIndex.Item1 == -2)
                {
                    //Error in instruction format
                    Console.WriteLine("Instruction Format Error!");
                    return;
                }
                //If instruction can be scheduled
                if(selectedInstructionIndex.Item1 > -1)
                {
                    //No dependencies
                    //Schedule next instruction 
                    decodedInstruction = DecodeInstruction(instructions[selectedInstructionIndex.Item1]);
                    int waitTime = SetWaitTime(decodedInstruction.Item2);
                    waits.Add(selectedInstructionIndex.Item1, waitTime);
                    SetRegistersUsed(true,decodedInstruction);

                    //Print instruction issue cycle
                    PrintCycle(cycle,instructions[selectedInstructionIndex.Item1], (selectedInstructionIndex.Item1 + 1).ToString(),"");
                }
                //Decrease available fetches by amount in selectedInstructionIndex.Item2, 
                //benched instructions will have a 0
                availableFetches -= selectedInstructionIndex.Item2;
            }
        }
        public bool CheckDependencies(Tuple<List<int>, char> decodedInstruction)
        {
            //Check for Write after Read and Write after Write dependencies
            int register = decodedInstruction.Item1[0];
            if(registersWrittenTo[register] > 0 || registersReadFrom[register] > 0)
            {
                return false;
            }
            //Check for Read after Write dependencies
            if(decodedInstruction.Item1.Count > 1)
            {
                for(int i = 1; i < decodedInstruction.Item1.Count; i++)
                {
                    register = decodedInstruction.Item1[i];
                    if(registersWrittenTo[register] > 0)
                    {
                        return false;
                    }
                }
            }
            //No dependencies
            return true; 
        }

        public void RenameRegister(Tuple<int, int> renamingRule, int instructionToRenameIndex)
        {
            //Look for instruction based on index
            //Replace R# register with T# register
            string instruction = instructions[instructionToRenameIndex];
            string renaming = 'T' + (renamingRule.Item1 - 8).ToString();
            string stringToReplace = 'R' + renamingRule.Item2.ToString();
            if(renamingRule.Item2 >= 8)
            {
                int temproraryIndex = renamingRule.Item2 - 8;
                stringToReplace = 'T' + temproraryIndex.ToString();
            }
            string renamedInstruction = instruction.Replace(stringToReplace, renaming);
            instructions[instructionToRenameIndex] = renamedInstruction;
        }

        public void UpdateInstructions(int instructionIndex)
        {
            //Decode instruction and store it in local variable
            Tuple<List<int>, char> decodedInstruction = DecodeInstruction(instructions[instructionIndex]);  

            //Remove unused renaming rules before checking      
            UpdateRenamingRules();

            //If there are renaming rules in place
            if(renamingRules.Count > 0)
            {
                for(int j = 0; j < decodedInstruction.Item1.Count; j++)
                {
                    bool renamingFound = true;
                    //Find last renaming possible for each register
                    while(renamingFound)
                    {
                        //Find a renaming rule already in place for current register
                        for(int i = 0; i < renamingRules.Count; i++)
                        {
                            //If renaming rule was found for current register
                            if(renamingRules[i].Item2 == decodedInstruction.Item1[j])
                            {
                                //Rename register at instructionIndex using renaming rule i
                                RenameRegister(renamingRules[i], instructionIndex);
                                break;
                            }
                        }
                        //Store previous decoded instruction
                        Tuple<List<int>, char> decodedInstructionPrevious = decodedInstruction;

                        //Decode instruction to update used registers
                        decodedInstruction = DecodeInstruction(instructions[instructionIndex]);

                        //If previous decoded instruction is same as current, no renaming rule was found
                        if(decodedInstruction.Item1.SequenceEqual(decodedInstructionPrevious.Item1))
                        {
                            renamingFound = false;
                        }
                    }
                }   
            }

            //Check for Write after Read and Write after Write dependencies
            int register = decodedInstruction.Item1[0];

            //Check for Read after Write dependencies
            //If instruction can't be scheduled regardless,
            //don't add new renaming rules
            if(decodedInstruction.Item1.Count > 1)
            {
                for(int i = 1; i < decodedInstruction.Item1.Count; i++)
                {
                    register = decodedInstruction.Item1[i];
                    if(registersWrittenTo[register] > 0)
                    {
                        return;
                    }
                }
            }
            //Reassign value of register to firt register used in case it was change
            register = decodedInstruction.Item1[0];

            if(registersWrittenTo[register] > 0 || registersReadFrom[register] > 0)
            {
                //If no renaming rule was found, add one if possible and needed
                //Find first temporary register that is not in use
                for(int i = 8; i < registersWrittenTo.Length; i++)
                {
                    //If current register is not in use
                    if(registersWrittenTo[i] == 0 && registersReadFrom[i] == 0)
                    {
                        //Add new renaming rule with current temporary register
                        Tuple<int, int> renamingRule = new Tuple<int, int> (i,decodedInstruction.Item1[0]);
                        renamingRules.Add(renamingRule);
                        //Rename register at instructionIndex using renamingRule
                        RenameRegister(renamingRule, instructionIndex);
                        break;
                    }
                }
            }
            
            return;
        }

        void UpdateRenamingRules()
        {
            for(int i = 8; i < registersWrittenTo.Length; i++)
            {
                //If current temporary register is not in use
                if(registersWrittenTo[i] == 0 && registersReadFrom[i] == 0)
                {
                    //If there are renaming rules in place
                    if(renamingRules.Count > 0)
                    {
                        for(int j = 0; j < renamingRules.Count; j++)
                        {
                            //If current renaming rule is for register i
                            if(renamingRules[j].Item1 == i)
                            {
                                //Remove it, no longer needed
                                renamingRules.Remove(renamingRules[j]);
                            }
                        }
                    }
                }
            }
        }
        public abstract Tuple<int, int> SelectInstruction();
    }
}