namespace InstructionScheduler 
{ 
    public class NormalRegisters : IRegister 
    {
        private int[] regs = new int[8];

        public void setRegister(int index, int value){
            if(index > 7 || index < 0) {
                Console.WriteLine("Index out of range!");
                return;
            }
            else {
                regs[index] = value;
            }
        }
    }
}