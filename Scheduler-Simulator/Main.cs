using System;

namespace InstructionScheduler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IScheduler singleInOrder16Regs = new InOrderExecution(1,16);
            IScheduler singleInOrder8Regs = new InOrderExecution(1,8);
            IScheduler singleOutOfOrder16Regs = new OutOfOrderExecution(1,16);
            IScheduler singleOutOfOrder8Regs = new OutOfOrderExecution(1,8);
            IScheduler super2InOrder16Regs = new InOrderExecution(2,16);
            IScheduler super2InOrder8Regs = new InOrderExecution(2,8);
            IScheduler super2OutOfOrder16Regs = new OutOfOrderExecution(2,16);
            IScheduler super2OutOfOrder8Regs = new OutOfOrderExecution(2,8);
            IScheduler super3InOrder16Regs = new InOrderExecution(3,16);
            IScheduler super3InOrder8Regs = new InOrderExecution(3,8);
            IScheduler super3OutOfOrder16Regs = new OutOfOrderExecution(3,16);
            IScheduler super3OutOfOrder8Regs = new OutOfOrderExecution(3,8);

            Console.WriteLine("Single instruction execution, in order (renaming enabled):");
            singleInOrder16Regs.ReadInstrcutions();
            singleInOrder16Regs.ScheduleInstructions();
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("Single instruction execution, in order (renaming disabled):");
            singleInOrder8Regs.ReadInstrcutions();
            singleInOrder8Regs.ScheduleInstructions();
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("Single instruction execution, out of order (renaming enabled):");
            singleOutOfOrder16Regs.ReadInstrcutions();
            singleOutOfOrder16Regs.ScheduleInstructions();
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("Single instruction execution, out of order (renaming disabled):");
            singleOutOfOrder8Regs.ReadInstrcutions();
            singleOutOfOrder8Regs.ScheduleInstructions();
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("SuperScalar execution with 2 functional units, in order (renaming enabled):");
            super2InOrder16Regs.ReadInstrcutions();
            super2InOrder16Regs.ScheduleInstructions();
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("SuperScalar execution with 2 functional units, in order (renaming disabled):");
            super2InOrder8Regs.ReadInstrcutions();
            super2InOrder8Regs.ScheduleInstructions();
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("SuperScalar execution with 2 functional units, out of order (renaming enabled):");
            super2OutOfOrder16Regs.ReadInstrcutions();
            super2OutOfOrder16Regs.ScheduleInstructions();
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("SuperScalar execution with 2 functional units, out of order (renaming disabled):");
            super2OutOfOrder8Regs.ReadInstrcutions();
            super2OutOfOrder8Regs.ScheduleInstructions();
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("SuperScalar execution with 3 functional units, in order (renaming enabled):");
            super3InOrder16Regs.ReadInstrcutions();
            super3InOrder16Regs.ScheduleInstructions();
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("SuperScalar execution with 3 functional units, in order (renaming disabled):");
            super3InOrder8Regs.ReadInstrcutions();
            super3InOrder8Regs.ScheduleInstructions();
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("SuperScalar execution with 3 functional units, out of order (renaming enabled):");
            super3OutOfOrder16Regs.ReadInstrcutions();
            super3OutOfOrder16Regs.ScheduleInstructions();
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("SuperScalar execution with 3 functional units, out of order (renaming disabled):");
            super3OutOfOrder8Regs.ReadInstrcutions();
            super3OutOfOrder8Regs.ScheduleInstructions();
            Console.WriteLine("-------------------------------------------------");
        }
    }
}