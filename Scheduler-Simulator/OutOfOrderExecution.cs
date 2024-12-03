
namespace InstructionScheduler 
{
    public class OutOfOrderExecution: GeneralScheduler 
    {
        private List<int> benchedInstructions = new List<int>(){};

        public OutOfOrderExecution(int functionalUnitsNumber, int registerNumber)
        : base(functionalUnitsNumber,registerNumber){}

        public override Tuple<int,int> SelectInstruction()
        {
            int nextUnscheduledIndex = 0;
            //Schedule benched instruction if possible
            Tuple<int,int> benchedInstructionScheduled = ScheduleBenchedInstructions();
            if(benchedInstructionScheduled.Item1 > -1)
            {
                return benchedInstructionScheduled;
            }
            else if(benchedInstructionScheduled.Item1 == -2)
            {
                return benchedInstructionScheduled;
            }

            //Find earliest unscheduled instruction
            if(waits.Count > 0)
            {
                for(int i = 0; i < instructions.Count; i++)
                {                    
                    //Set "instruction can't be scheduled" as default
                    nextUnscheduledIndex = -1;
                    
                    //if instruction hasn't been scheduled yet and is not a benched instruction
                    if(!waits.ContainsKey(i)&&!benchedInstructions.Contains(i))
                    {
                        //If latest instruction hasn't been scheduled yet
                        if(nextUnscheduledIndex != instructions.Count)
                        {
                            nextUnscheduledIndex = i;
                            if(registerRenamingEnabled)
                            {
                                UpdateInstructions(nextUnscheduledIndex);
                            }
                            Tuple<List<int>, char> decodedInstruction;
                            decodedInstruction = DecodeInstruction(instructions[nextUnscheduledIndex]);
                            
                            if(decodedInstruction.Item2 == '\0')
                            {
                                //Error in instruction format
                                Console.WriteLine("Instruction Format Error!");
                                return new Tuple<int, int> (-2,1);
                            }
                            //Check instruction dependencies
                            if(CheckDependencies(decodedInstruction))
                            {
                                break;
                            }
                            else
                            {
                                benchedInstructions.Add(i);
                                return new Tuple<int, int> (-1,1);
                            }
                        }
                        else
                        {
                            //No instruction to schedule available
                            return new Tuple<int, int> (nextUnscheduledIndex,1);
                        } 
                    }
                }
            }
            return new Tuple<int, int> (nextUnscheduledIndex,1);
        }
        private Tuple<int,int> ScheduleBenchedInstructions()
        {
            if(benchedInstructions.Count > 0)
            {
                foreach(int instructionIndex in benchedInstructions)
                {
                    Tuple<List<int>, char> decodedInstruction;
                    decodedInstruction = DecodeInstruction(instructions[instructionIndex]);
                    
                    if(decodedInstruction.Item2 == '\0')
                    {
                        //Error in instruction format
                        Console.WriteLine("Instruction Format Error!");
                        return new Tuple<int, int> (-2,1);
                    }
                    //If instruction at instructionIndex has no dependencies
                    if(CheckDependencies(decodedInstruction))
                    {
                        //Remove it from benched instructions and schedule it
                        //Does not consume fetches
                        benchedInstructions.Remove(instructionIndex);
                        return new Tuple<int, int> (instructionIndex, 0);
                    }
                }
                //No benched instruction can be scheduled
                return new Tuple<int, int> (-1,1);
            }
            else
            {
                //No benched instructions to schedule
                return new Tuple<int, int> (-1,1);
            }
        }
        public override void UpdateCycle(int cycle)
        {
            PrintCycle(cycle,"", "","");
            DecreaseWaits(cycle);
        }
    }
}