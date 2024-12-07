using System;
using System.IO;
using System.Linq;
namespace InstructionScheduler 
{
    public abstract class GeneralScheduler: IScheduler
    {
        //Stores which registers are being read from
        protected int[] registersReadFrom;

        //Stores which registers are being written to
        protected int[] registersWrittenTo;

        //Stores renaming rules
        //Item1 = temporary register index,
        //Item2 = normal register being renamed index
        protected List<Tuple<int, int>> renamingRules; 

        //Stores the instructions read from instruction file 
        protected List<string> instructions = [];

        //Stores the current instructions scheduled and the wait time until they are finished
        protected Dictionary<int, int> waits = [];

        //Stores current number of instructions completed
        protected int instructionsDone = 0;

        //Stores number of available instruction fethces per cycle
        protected int fetches;

        //Indicates whether register renaming is enabled or not
        protected bool registerRenamingEnabled;

        public GeneralScheduler(int functionalUnitsNumber, int registerNumber)
        {
            //Define the size of the registers arrays
            registersWrittenTo = new int[registerNumber];
            registersReadFrom = new int[registerNumber];
            //If more than 8 registers are being used
            if(registerNumber > 8)
            {
                //Register renaming is enabled
                registerRenamingEnabled = true;
                renamingRules = [];
            }
           //Set initial value for waiting times
            waits.Add(-1,1);

            //Define the number of fetches based on the number of functional units
            fetches = functionalUnitsNumber;
        }

        //Retreives instructions from file
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
                //Handle exception when reading files
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading the file: {ex.Message}");
                }
            }
            //File not found
            else
            {
                Console.WriteLine("The specified file does not exist. Please check the path and try again.");
            }
        }
        //Retrieve a list of registers indices used in the instruction as well as the operation
        public Tuple<List<int>,char> DecodeInstruction(string instruction)
        {
            //List to append register indices used
            List<int> registerIndexes = new List<int>{};

            //Default operation as null
            char operation = '\0';
            
            //Iterate through instruction's chars to retrieve important information
            for(int i = 0; i < instruction.Length; i++)
            {
                //If a register was found (name always contains an 'R' or a 'T')
                if(instruction[i] == 'R'||instruction[i] == 'T')
                {
                    //Try retrieving register index from next char
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
                    //Catch IndexOutofRangeException, there is an error in instruction format
                    //Regitser must always have a name
                    //'R' and 'T' are reserved for names of registers
                    catch(IndexOutOfRangeException)
                    {
                        //Return empty list and operation char
                        Console.WriteLine("Instruction Format Error!");
                        registerIndexes.Clear();
                        return new Tuple<List<int>, char>(registerIndexes,operation);
                    }
                }
                //Operation was found
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
            //Return list of registers used with the operation char
            return new Tuple<List<int>, char>(registerIndexes, operation);
        }
        //Print current cycle
        public void PrintCycle(int cycleIndex, string instruction, string instructionIndex, string instructionDoneIndex)
        {
            //Define a set witdh for each column
            const int columnWidth = 15;

            //Adjust the instruction to fit the column width
            string formattedInstruction;
            if (instruction.Length > columnWidth)
            {
                //Truncate and append dots if too long
                formattedInstruction = instruction.Substring(0, columnWidth - 3) + "...";
            }
            else
            {
                //Pad with spaces if too short
                formattedInstruction = instruction.PadRight(columnWidth);
            }

            //Print formatted output
            string cycleResult = $"| Cycle: {cycleIndex,-5} | {instructionIndex,-5} | {formattedInstruction} | {instructionDoneIndex,-5} |";
            Console.WriteLine(cycleResult);
            Console.WriteLine(new string('-', cycleResult.Length));
        }
        //Set wait times for each specific instruction based on operation type
        public int SetWaitTime(char operation)
        {
            //Instruction is addition or substraction
            if(operation == '+'|| operation == '-')
            {
                return 1;
            }
            //Instruction is multiplication
            else if(operation == '*')
            {
                return 2;
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
        //Decrease the wait times of currently scheduled instructions
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
                    //to force dependent instruction to execute until next cycle

                    //Release registers used
                    Tuple<List<int>, char> decodedInstruction = DecodeInstruction(instructions[key]);
                    SetRegistersUsed(false, decodedInstruction);
                }
            }
            return;
        }
        //Increase or decrease the number of instruction currently using the registers in decodedInstruction.Item1
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
        //Loop to schedule instructions
        public void ScheduleInstructions()
        {
            //Initial value of cycle
            int cycle = 0;

            Console.WriteLine("| Cycle # | Instruction Index | Instruction Scheduled | Instruction Done |");

            //Loop until the number of instructions completed == number of instructions
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
        //Fetch instructions to schedule based on the number of available fetches
        public void FetchInstructions(int availableFetches, int cycle)
        {
            //Tuple to store decoded instruction
            Tuple<List<int>, char> decodedInstruction;

            //Loop to schedule instructions while fetches are still available
            while(availableFetches > 0)
            {
                //Select an instruction and store its index and the amount it will reduce the number of fetches by
                Tuple<int, int> selectedInstructionIndex = SelectInstruction();

                //If instruction index == -2
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
        //Check if the registers used have dependencies of any type
        //Return true if the instruction can be scheduled
        //Return false if unresolvable dependencies are in place
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
        //Rename registers in instruction
        public void RenameRegister(Tuple<int, int> renamingRule, int instructionToRenameIndex)
        {
            //Look for instruction based on index
            //Replace R# register with T# register by default
            string instruction = instructions[instructionToRenameIndex];
            string renaming = 'T' + (renamingRule.Item1 - 8).ToString();
            string stringToReplace = 'R' + renamingRule.Item2.ToString();

            //If register is one of the temporary registers
            if(renamingRule.Item2 >= 8)
            {
                //Replace T# register with another T# register instead
                int temproraryIndex = renamingRule.Item2 - 8;
                stringToReplace = 'T' + temproraryIndex.ToString();
            }
            //Edit instruction to replace registers with new renaming
            string renamedInstruction = instruction.Replace(stringToReplace, renaming);
            instructions[instructionToRenameIndex] = renamedInstruction;
        }

        //Update instructions if necesary with renaming rules in place for its registers
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

            //Check for Read after Write dependencies
            //If instruction can't be scheduled regardless,
            //don't add new renaming rules
            int register;
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

            //Reassign value of register to firt register used in case it was changed
            register = decodedInstruction.Item1[0];

            //Check for Write after Read and Write after Write dependencies
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
        //Remove unused renaming rules from renaming rules list
        public void UpdateRenamingRules()
        {
            //Iterate through all temporary registers
            for(int i = 8; i < registersWrittenTo.Length; i++)
            {
                //If current temporary register is not in use
                if(registersWrittenTo[i] == 0 && registersReadFrom[i] == 0)
                {
                    //If there are renaming rules in place
                    if(renamingRules.Count > 0)
                    {
                        //Iterate through all renaming rules
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
        //Update cycle by decreasing wait times and any other necesary operations
        public abstract void UpdateCycle(int cycle);
        //Select an instruction to schedule
        public abstract Tuple<int, int> SelectInstruction();
    }
}