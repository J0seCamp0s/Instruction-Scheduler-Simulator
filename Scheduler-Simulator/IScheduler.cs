namespace InstructionScheduler 
{
    public interface IScheduler 
    {
        void ScheduleInstructions();
        void ReadInstrcutions(string path);
        Tuple<List<int>,char> DecodeInstruction(string instruction);
        void PrintCycle(int cycleIndex, string instruction, int instructionIndex, int instructionDoneIndex);
    }
}