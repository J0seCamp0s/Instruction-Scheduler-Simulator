namespace InstructionScheduler 
{
    public class InOrderExecution: GeneralScheduler
    {
        public InOrderExecution(int functionalUnitsNumber, int registerNumber)
        : base(functionalUnitsNumber,registerNumber){}

        public override Tuple<int,int> SelectInstruction()
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
            
            //If latest instruction hasn't been scheduled
            if(latestInstructionIndex + 1 != instructions.Count)
            {
                if(registerRenamingEnabled)
                {
                    UpdateInstructions(latestInstructionIndex + 1);
                }
                Tuple<List<int>, char> decodedInstruction;
                decodedInstruction = DecodeInstruction(instructions[latestInstructionIndex+1]);
                
                if(decodedInstruction.Item2 == '\0')
                {
                    //Error in instruction format
                    Console.WriteLine("Instruction Format Error!");
                    return new Tuple<int,int> (-2,1);
                }
                //Check instruction dependencies
                if(!CheckDependencies(decodedInstruction))
                {
                    //Instruction can't be scheduled
                    return new Tuple<int,int> (-1,1);
                }
                //Return the next instruction index
                return new Tuple<int,int> (latestInstructionIndex + 1,1);
            }
            else
            {
                return new Tuple<int,int> (-1,1);
            }
            
        }

        public override void UpdateCycle(int cycle)
        {
            PrintCycle(cycle,"", "","");
            DecreaseWaits(cycle);

            //Print finished instructions and remove unused wait times
            UpdateWaitTimesList();
        }
        public void UpdateWaitTimesList()
        {
            //Traverse all current keys from the dictionary
            foreach(int key in waits.Keys)
            {
                //Compare with all the keys
                foreach(int compareKey in waits.Keys)
                {
                    //If a previous instruction is not done yet
                    if((compareKey < key)&&(waits[compareKey] > waits[key]))
                    {
                        //Make current key's wait time equal to that of the previous instruction
                        //Enforce in order retirement
                        waits[key] = waits[compareKey];
                    }
                }
                //Remove wait times that are not needed anymore
                if(waits[key] == -2)
                {
                    waits.Remove(key);
                }  
            }
        }
    }
}