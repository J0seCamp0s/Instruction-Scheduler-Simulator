namespace InstructionScheduler 
{
    public class SuperScalar: GeneralScheduler 
    {

        public SuperScalar(int functionalUnitsNumber, int registerNumber)
        : base(functionalUnitsNumber,registerNumber){}
        public override void ScheduleInstructions()
        {
            
        }
    }
}