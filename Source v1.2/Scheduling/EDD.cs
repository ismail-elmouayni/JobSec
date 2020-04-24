


namespace JOBSEC.Scheduling
{
    class EDD : RuleBasedMethod
    {
        public EDD(Model model, ExcelFile excelFile) : base(model, excelFile) { name = "Rule of Early Due Date"; }

        // ruturns the job to assign first depending on the rule of scheduling
        protected override int getJobToAssign(Solution s)
        {
            int selJob = -1;
            // Looking for a job wich is not assigned yet
            for (int j = 0; j < model.getNumOfOrders(); j++)
                if (!model.orderIsAssigned(s, j))
                {
                    selJob = j;
                    break;
                }

            // if selJob still have the value -1, that means 
            // that are jobs are assigned, then break 
            if (selJob == -1)
                return selJob;
            // Else, from those who are not assigned yet, get
            // the one with the closest due date,
            // including setup time 
            else
            {
                for (int i = selJob; i < model.getNumOfOrders(); i++)
                    if ((model.dk(selJob) > model.dk(i)) && !model.orderIsAssigned(s, i))
                        selJob = i;

                return selJob;
            }
        }
    }
}
