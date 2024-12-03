using System;

namespace InstructionScheduler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IScheduler singleInstructionSchedulerInOrder = new InOrderExecution(1,16);
            IScheduler superScalarSchedulerInOrder = new InOrderExecution(2,16);
            IScheduler superScalarSchedulerOutOfOrder = new OutOfOrderExecution(2,16);

            // Console.WriteLine("SuperScalar execution with 2 functional units, in order:");
            // superScalarSchedulerInOrder.ReadInstrcutions();
            // superScalarSchedulerInOrder.ScheduleInstructions();
            // Console.WriteLine("-------------------------------------------------");

            // Console.WriteLine("Single instruction exection, in order:");
            // singleInstructionSchedulerInOrder.ReadInstrcutions();
            // singleInstructionSchedulerInOrder.ScheduleInstructions();
            // Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("Single instruction exection, out of order:");
            superScalarSchedulerOutOfOrder.ReadInstrcutions();
            superScalarSchedulerOutOfOrder.ScheduleInstructions();
            Console.WriteLine("-------------------------------------------------");
        }
    }
}