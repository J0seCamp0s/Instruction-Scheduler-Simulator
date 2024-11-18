
namespace InstructionScheduler 
{
    public class OutOfOrderExecution: GeneralScheduler 
    {

        public OutOfOrderExecution(int functionalUnitsNumber, int registerNumber)
        : base(functionalUnitsNumber,registerNumber){}

        public override int SelectInstruction()
        {
            int nextUnscheduledIndex = 0;
            //Find earliest unscheduled instruction
            if(waits.Count > 0)
            {
                for(int i = 0; i < instructions.Count; i++)
                {
                    //Set "instruction can't be scheduled" as default
                    nextUnscheduledIndex = -1;

                    //if instruction hasn't been scheduled yet
                    if(!waits.ContainsKey(i))
                    {
                        //If latest instruction hasn't been scheduled yet
                        if(nextUnscheduledIndex != instructions.Count)
                        {
                            nextUnscheduledIndex = i;
                            Tuple<List<int>, char> decodedInstruction;
                            decodedInstruction = DecodeInstruction(instructions[nextUnscheduledIndex]);
                            
                            if(decodedInstruction.Item2 == '\0')
                            {
                                //Error in instruction format
                                Console.WriteLine("Instruction Format Error!");
                                return -2;
                            }
                            //Check instruction dependencies
                            if(CheckDependencies(decodedInstruction))
                            {
                                break;
                            }
                        }
                        else
                        {
                            //No instruction to schedule available
                            return nextUnscheduledIndex;
                        } 
                    }
                }
            }
            return nextUnscheduledIndex;
        }
        public override void UpdateCycle(int cycle)
        {
            PrintCycle(cycle,"", "","");
            DecreaseWaits(cycle);
        }
    }
}