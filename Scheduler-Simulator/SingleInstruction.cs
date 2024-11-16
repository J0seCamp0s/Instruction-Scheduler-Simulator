namespace InstructionScheduler 
{
    public class SingleInstruction: GeneralScheduler
    {
        public override void ScheduleInstructions()
        {
            int wait = -1;
            int i = 0;
            int iDone = 0;
            int cycle = 1;
            Tuple<List<int>, char> decodedInstruction;
            while(i < instructions.Length) 
            {
                if(wait == -1)
                {
                    Console.WriteLine("| Cycle # | Instruction Index | Instruction Scheduled | Instruction Done |");
                    wait++;
                }
                else if(wait == 0)
                {
                    decodedInstruction = DecodeInstruction(instructions[i]);
                    PrintCycle(cycle,instructions[i],i+1,0);
                    if(decodedInstruction.Item2 == '+'|| decodedInstruction.Item2 == '-')
                    {
                        wait = 1;
                    }
                    else if(decodedInstruction.Item2 == '*')
                    {
                        wait = 2;
                    }
                    else if(decodedInstruction.Item2 == 'S'|| decodedInstruction.Item2 =='L')
                    {
                        wait = 3;
                    }
                    else if(decodedInstruction.Item2 == '\0')
                    {
                        //Instruction weren't in appropiate format
                        return;
                    }
                }
                else
                {
                    wait--;
                }
            }
            Console.WriteLine("Execution Done!");
            Console.WriteLine($"Total Number of Cycles: {cycle}");
        }
    }
}