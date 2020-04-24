

namespace JOBSEC.Scheduling
{
    class CR : RuleBasedMethod
    {
        public CR(Model model, ExcelFile excelFile) : base(model, excelFile) { name = "Rule of Critical Ratio"; }


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
            // the one with the shortest processing time,
            // including setup time 
            else
            {
                for (int i = selJob; i < model.getNumOfOrders(); i++)
                    if ((model.getCriticalRatio(selJob) > model.getCriticalRatio(i)) && !model.orderIsAssigned(s, i))
                        selJob = i;

                return selJob;
            }
        }
    }
}
