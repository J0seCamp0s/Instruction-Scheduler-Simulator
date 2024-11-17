namespace InstructionScheduler 
{
    public interface IScheduler 
    {
        public void ScheduleInstructions();
        public void ReadInstrcutions();
        protected Tuple<List<int>,char> DecodeInstruction(string instruction);
        protected void DecreaseWaits(int cycle);
        protected void FetchInstructions(int availableFetches, int cycle);
        protected void UpdateWaitTimesList();
        protected bool CheckDependencies(Tuple<List<int>, char> decodedInstruction);
        protected int SelectInstruction();
        protected void PrintCycle(int cycleIndex, string instruction, string instructionIndex, string instructionDoneIndex);
        protected int SetWaitTime(char operation);
    }
}