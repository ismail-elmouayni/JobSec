using NSimulate;
using NSimulate.Instruction;
using System.Collections.Generic;

namespace JOBSEC.Simulation
{
    class JobProcess : Process
    {
        /// <summary>
        ///  Simulate the process. 
        /// </summary>
        /// 
        /*
        public override IEnumerator<InstructionBase> Simulate()
        { 
            //
            // this method is implemented as an iterator that uses _yield return_ statements to return instructions to the simulator
            // instructions may:
            //   - cause the process to wait for a period of time, until a condition is met, or until a notification is recieved
            //   - cause the process to wait until resource can be allocated
            //   - schedule activity
            //   - activate or inactivate another process
            //   - a number of other possibilities (see documentation for Instructions)

            // e.g. wait until a notification is raised... in this case a notification that an alarm is ringing
            yield return new WaitNotificationInstruction<AlarmRingingNotification>();

            // request a resource (if resources are not available, the process will be blocked here until they become available
            var allocateResourceXInstruction = new AllocateInstruction<ResourceX>(1);
            yield return allocateResourceXInstruction;

            // wait for a period of time
            yield return new WaitInstruction(10);

            // release a previously allocated resource
            yield return new ReleaseInstruction<ResourceX>(allocateResourceXInstruction);
        }
       */
    }
}
