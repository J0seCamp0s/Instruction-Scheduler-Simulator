namespace InstructionScheduler
{
    public class CPU: ICPU
    {
        private IRegister registers;
        private IScheduler scheduler;
        private IFunctionalUnit functionalUnit = new ALU();

        public CPU(int registerType, int schedulerType) {
            registers = registerType switch
            {
                0 => new NormalRegisters(),
                1 => new ExtendedRegisters(),
                _ => new NormalRegisters(),
            };
            scheduler = schedulerType switch
            {
                0 => new SingleInstruction(),
                1 => new SuperScaler(),
                _ => new SingleInstruction(),
            };
        }
    }
}