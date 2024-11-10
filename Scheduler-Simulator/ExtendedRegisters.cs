namespace InstructionScheduler 
{
    public class ExtendedRegisters: IRegister 
    {
        //Register support 8 additional temporary registers
        //for register renaming
        private int[] regs = new int[16];
    }
}
