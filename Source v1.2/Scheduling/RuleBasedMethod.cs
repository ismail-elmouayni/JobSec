
namespace JOBSEC.Scheduling
{
    abstract class RuleBasedMethod : SecMethod
    {

        public RuleBasedMethod(Model model, ExcelFile excelFile) : base(model, excelFile) { }

        // The job selection rul is specefied in the derived classes 
        // Hence, set here as virtual method 
        abstract protected int getJobToAssign(Solution s);

        override public void apply()
        {
            MyConsole.displayMain($"\n\n Applying {name} ----------------------------------: ");
            solution = buildSolution();
            if (solution != null)
                MyConsole.displayResult($"-----solution fitness : {model.fitness(solution)}");
            else
                MyConsole.displayError("Couldn't construct solution");
        }


        override
        protected Solution buildSolution()
        {
            Solution    s = new Solution(model);               // Initializing a solution structure (still emplty)    
            int         selJob;
            bool        assignmentIsSuccessfull = true;

            // Iterative scheduling of jobs based on priority rule  
            // while iterative job assignements are successfull 
            do
            {
                // Select the job that has the priority 
                // Implemented in the child class
                selJob = getJobToAssign(s);
                if (selJob >= 0)
                {
                    MyConsole.display($"Assigning job {selJob}");
                    assignmentIsSuccessfull = model.assignJob(s, selJob, 0);
                    if(!assignmentIsSuccessfull)
                        MyConsole.displayError($"Could not assign job {selJob}");
                }

            } while (selJob >= 0 && assignmentIsSuccessfull);

            if (assignmentIsSuccessfull)
            {
                model.finalize(s);
                return s;
            }
            else
            {
                return null;
            }
        }
    }
}
