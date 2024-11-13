using System;
using System.IO;
namespace InstructionScheduler 
{
    public abstract class GeneralScheduler: IScheduler
    {
        protected string[] instructions = new string[]{};
        public void ReadInstrcution(string path) 
        {
            string rawInstructions = File.ReadAllText(path);
            instructions = rawInstructions.Split("\n");
        }
        public abstract void ScheduleInstructions(string instructionSet);
    }
}