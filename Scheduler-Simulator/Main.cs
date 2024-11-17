using System;

namespace InstructionScheduler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IScheduler singleInstructionScheduler = new InOrderExecution(1,8);
            IScheduler superScalarScheduler = new InOrderExecution(2,8);

            Console.WriteLine("SuperScalar execution with 2 functional units:");
            superScalarScheduler.ReadInstrcutions();
            superScalarScheduler.ScheduleInstructions();
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("Single instruction exection:");
            singleInstructionScheduler.ReadInstrcutions();
            singleInstructionScheduler.ScheduleInstructions();
            Console.WriteLine("-------------------------------------------------");
        }
    }
}