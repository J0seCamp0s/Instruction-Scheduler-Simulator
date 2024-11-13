namespace InstructionScheduler 
{
    public interface IScheduler 
    {
        void ScheduleInstructions(string instructionSet);
        void ReadInstrcution(string path);
    }
}