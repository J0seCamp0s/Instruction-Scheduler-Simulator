namespace InstructionScheduler 
{
    public interface IScheduler 
    {
        public void ScheduleInstructions();
        public void ReadInstrcutions();
        protected Tuple<List<int>,char> DecodeInstruction(string instruction);
        protected void DecreaseWaits(int cycle);
        protected void PrintCycle(int cycleIndex, string instruction, string instructionIndex, string instructionDoneIndex);
        protected int SetWaitTime(char operation);
    }
}