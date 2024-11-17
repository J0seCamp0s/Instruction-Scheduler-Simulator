
namespace InstructionScheduler 
{
    public class OutOfOrderExecution: GeneralScheduler 
    {

        public OutOfOrderExecution(int functionalUnitsNumber, int registerNumber)
        : base(functionalUnitsNumber,registerNumber){}

        public override int SelectInstruction()
        {
            int nextUnscheduledIndex = -1;
            //Find earliest unscheduled instruction
            if(waits.Count > 0)
            {
                for(int i = 0; i < instructions.Count; i++)
                {
                    //if instruction hasn't been scheduled yet
                    if(!waits.Keys.Contains(i))
                    {
                        nextUnscheduledIndex = i;
                        break;
                    }
                }
            }
            return nextUnscheduledIndex;
        }
        public override void UpdateWaitTimesList()
        {
            //Traverse all current keys from the dictionary
            foreach(int key in waits.Keys)
            {
                //Remove wait times that are not needed anymore
                if(waits[key] == -2)
                {
                    waits.Remove(key);
                }  
            }
        }
    }
}