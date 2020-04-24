

namespace JOBSEC.Scheduling
{
    class SES : IterativeMethod
    {
        public enum METHOD
        {
            LPT     = 0,
            CR      = 1,
            SPT     = 2,
            EDD     = 3,
            SLACK   = 4
        }

        private int rule = (int)METHOD.SPT;                                      //  Rule to apply 

        public SES(Model model, ExcelFile excelFile) : base (model, excelFile)
        {
            name = "SES"; 
        }

        public override void apply()
        {

          // We apply the differents rules to look for solution enhancement  
          foreach (int r in System.Enum.GetValues(typeof(METHOD)))
          {
                rule = r;
                solution = buildSolution();
                if (solution != null)
                {
                    // We also apply a local search 
                    solution = localSearch(solution);

                    // If we have a existing bestsolution 
                    if (bestSolution != null)
                    {
                        updateBestSolution(r);
                    }
                    else
                    {
                        bestSolution = solution;
                        fitnessCurve.Add(model.fitness(solution), r);
                    }
                }
                else
                    MyConsole.displayError($"Couldn't find solution using rule {r}"); 
          }

          exportFitnessToExcel();
        }
        

        // Building a solution using the selected rul 
        override protected Solution buildSolution()
        {
            // Variables 
            Solution    s = new Solution(model);                    // Initializing a new solution structure solution(vector x, vector y)    
            int         selJob;
            bool        jobIsAssigned = true;                       // Indicating if the job was assigned successfully 

            // Iterative scheduling of jobs based on priority rule  
            do
            {
                // Select the job that has the priority 
                // Implemented in the child class
                selJob = getJobToAssign(s, rule);
                if (selJob >= 0)
                {
                    jobIsAssigned = model.assignJob(s, selJob, 0);
                    if (!jobIsAssigned)
                        MyConsole.displayError($"Could not assign job {selJob}");
                }

            }
            while (selJob >= 0 && jobIsAssigned);

            if (jobIsAssigned)
            {
                model.finalize(s);                  // Do some final operation on solution (calculate fatigue index...) 
                return s;
            }
            else
            {
                return null;
            }
      
        }


        // We also apply a local search in SES to try to improve 
        // the solution found 
        protected override Solution localSearch(Solution solution)
        {
            Solution _s             = solution;                 // Contains a copy of the solution to be improved using local search  
            Solution betterSolution = solution;                 // Solution retained after improvement  

            // A local serach consist on looking for jobs assigned 
            // to machines, removing the assignement and trying 
            // by a local search, to find a better solution 
            for(int m = 0; m <model.getNumOfMachines(); m++)
            {
                // We remove the jobs for that machine (m)
                for (int job = 0; job < model.getNumOfOrders(); job++)
                    if (model.orderIsAssignedToMachine(_s, job, m))
                    {
                        model.removeJob(job, _s); 
                    }

                // Then we reassigning jobs. We try all rules using do loop 
                // we keep the best solution. 
                foreach (int i in System.Enum.GetValues(typeof(METHOD)))
                {
                    int     selJob        = -1;
                    bool    jobIsAssigned = true; 

                    do
                    {
                        selJob = getJobToAssign(_s, i);                     // The job is selected using the rule i 
                        if (selJob >= 0)
                        {
                            MyConsole.display($"Assigning job {selJob}");
                            jobIsAssigned = model.assignJobToMachine(_s, selJob,m, 0);
                            if (!jobIsAssigned)
                                MyConsole.displayError($"In local search, could not assign job {selJob} to machine {m}");
                        }
                    }
                    // We quite the loop we couldn't select a job : 
                    // all jobs were reassigned. Or if jobIsAssigned 
                    // is false, which mean that we couldn't construct 
                    // a solution as a nob couldn't be assigned 
                    while (selJob >= 0 && jobIsAssigned);

                    if(jobIsAssigned)
                    {
                        if (model.fitness(_s) < model.fitness(betterSolution))
                            betterSolution = _s; 
                    }
                    
                }
            }
            return betterSolution; 
        }


        protected int getJobToAssign(Solution s, int localRule)
        {
            int selJob = -1;

            // Looking for a job wich is not assigned yet
            for (int j = 0; j < model.getNumOfOrders(); j++)
                if (!model.orderIsAssigned(s, j))
                {
                    selJob = j;
                    break;
                }

            // If selJob still have the value -1, that means 
            // that all jobs are assigned, then break 
            if (selJob == -1)
                return selJob;
            // Else, from those who are not assigned yet, get
            // one using a selection rule : SPT, EDD, LPT....
            else
            {
                switch (localRule)
                {
                    // In case of SPT rule 
                    case (int)METHOD.SPT:
                        for (int i = selJob; i < model.getNumOfOrders(); i++)
                            if ((model.pk(selJob) + model.sk(selJob) > model.pk(i) + model.sk(i)) && !model.orderIsAssigned(s, i))
                                selJob = i;
                    break;

                    // In case of LPT rule 
                    case (int)METHOD.LPT:
                        for (int i = selJob; i < model.getNumOfOrders(); i++)
                            if ((model.pk(selJob) + model.sk(selJob) < model.pk(i) + model.sk(i)) && !model.orderIsAssigned(s, i))
                                selJob = i;
                        break;

                    // In case of EDD rule 
                    case (int)METHOD.EDD:
                        for (int i = selJob; i < model.getNumOfOrders(); i++)
                            if ((model.dk(selJob) > model.dk(i)) && !model.orderIsAssigned(s, i))
                                selJob = i;
                        break;

                    // In case of CR rule 
                    case (int)METHOD.CR:
                        for (int i = selJob; i < model.getNumOfOrders(); i++)
                            if ((model.getCriticalRatio(selJob) > model.getCriticalRatio(i)) && !model.orderIsAssigned(s, i))
                                selJob = i;
                        break;

                    // In case of SLACK rule 
                    case (int)METHOD.SLACK:
                        for (int i = selJob; i < model.getNumOfOrders(); i++)
                            if ((model.dk(selJob) - model.pk(selJob) - model.sk(selJob) > model.dk(i) - model.pk(i) - model.sk(i)) && !model.orderIsAssigned(s, i))
                                selJob = i;
                        break;
                }

                return selJob;
            }

        }
    }
}
