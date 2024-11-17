namespace InstructionScheduler 
{
    public class InOrderExecution: GeneralScheduler
    {
        public InOrderExecution(int functionalUnitsNumber, int registerNumber)
        : base(functionalUnitsNumber,registerNumber){}

        public override int SelectInstruction()
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
            return latestInstructionIndex;
        }
        public override void UpdateWaitTimesList()
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