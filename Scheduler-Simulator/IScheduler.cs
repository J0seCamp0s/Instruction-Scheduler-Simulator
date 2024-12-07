namespace InstructionScheduler 
{
    public interface IScheduler 
    {
        //Descriptions of methods in GeneralScheduler.cs in order to avoid redundancy
        public void ScheduleInstructions();
        public void ReadInstrcutions();
        public Tuple<List<int>,char> DecodeInstruction(string instruction);
        public void DecreaseWaits(int cycle);
        public void FetchInstructions(int availableFetches, int cycle);
        public bool CheckDependencies(Tuple<List<int>, char> decodedInstruction);
        public Tuple<int,int> SelectInstruction();
        public void UpdateCycle(int cycle);
        public void PrintCycle(int cycleIndex, string instruction, string instructionIndex, string instructionDoneIndex);
        public int SetWaitTime(char operation);
        public void UpdateInstructions(int instructionIndex);
        public void UpdateRenamingRules();
    }
}