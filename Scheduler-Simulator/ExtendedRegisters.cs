namespace InstructionScheduler 
{
    public class ExtendedRegisters : IRegister 
    {
        //Register support 8 additional temporary registers
        //for register renaming
        private int[] regs = new int[16];

        public void setRegister(int index, int value) 
        {
            if(index > 15 || index < 0) {
                Console.WriteLine("Index out of range!");
                return;
            }
            else {
                regs[index] = value;
            }
        }
    }
}
