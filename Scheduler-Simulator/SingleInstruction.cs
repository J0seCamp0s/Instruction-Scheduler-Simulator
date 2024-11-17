namespace InstructionScheduler 
{
    public class InOrderExecution: GeneralScheduler
    {
        public InOrderExecution(int functionalUnitsNumber, int registerNumber)
        : base(functionalUnitsNumber,registerNumber){}
        public override void ScheduleInstructions()
        {
            int cycle = 0;
            Tuple<List<int>, char> decodedInstruction;

            Console.WriteLine("| Cycle # | Instruction Index | Instruction Scheduled | Instruction Done |");

            while(true) 
            {
                cycle ++;
                PrintCycle(cycle,"", "","");
                DecreaseWaits(cycle);
                foreach(int key in waits.Keys)
                {
                    foreach(int compareKey in waits.Keys)
                    {
                        //If a previous instruction is not done yet
                        if((compareKey < key)&&(waits[compareKey] > waits[key]))
                        {
                            //Make current keys wait time equal to that of the previous instruction
                            waits[key] = waits[compareKey];
                        }
                    }
                    //Remove wait times that are not needed anymore
                    if(waits[key] == -2)
                    {
                        waits.Remove(key);
                    }  
                }
                //If all instructions done
                if(instructionsDone == instructions.Count)
                {
                    //finish execution
                    break;
                }
                int availableFetches = fetches;
                //If instruction can be fetched
                while(availableFetches > 0)
                {
                    int latestInstructionIndex = -1;
                    //Find latest instruction index
                    if(waits.Count > 0)
                    {
                        foreach(int key in waits.Keys)
                        {
                            if(key > latestInstructionIndex)
                            {
                                latestInstructionIndex = key;
                            }
                        }
                    }
                    //If last instruction wasn't scheduled yet
                    if(latestInstructionIndex != instructions.Count-1)
                    {
                        decodedInstruction = DecodeInstruction(instructions[latestInstructionIndex + 1]);
                        if(decodedInstruction.Item2 == '\0')
                        {
                            //Error in instruction format
                            Console.WriteLine("Instruction Format Error!");
                            return;
                        }
                        //Check instruction dependencies
                        if(CheckDependencies(decodedInstruction))
                        {
                            //No dependencies
                            //Schedule next instruction 
                            int waitTime = SetWaitTime(decodedInstruction.Item2);
                            waits.Add(latestInstructionIndex + 1, waitTime);
                            SetRegistersUsed(true,decodedInstruction.Item1);

                            //Print instruction issue cycle
                            PrintCycle(cycle,instructions[latestInstructionIndex + 1], (latestInstructionIndex + 2).ToString(),"");
                        }
                    }
                    
                    availableFetches -= 1;
                }
            }
            Console.WriteLine("Execution Done!");
            Console.WriteLine($"Total Number of Cycles: {cycle}");
        }

        public bool CheckDependencies(Tuple<List<int>, char> decodedInstruction)
        {
            int register;

            //Check for Write after Read and Write after Write dependencies
            register = decodedInstruction.Item1[0];
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
    }
}