
/*
 * SES.cs
 *
 *
 * ***** BEGIN GPL LICENSE BLOCK *****
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, If not, see <http://www.gnu.org/licenses/>.
 *
 * The Original Code is Copyright (C) 2018-2019 by Ismail EL MOUAYNI.
 * All rights reserved.
 *
 * The Original Code is: all of this file.
 *
 * Contributor(s): none yet.
 *
 * ***** END GPL LICENSE BLOCK *****
 */

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

        private int rule = (int)METHOD.SPT; 
        public SES(Model model, ExcelFile excelFile) : base (model, excelFile)
        {
            name = "SES"; 
        }


        public override void apply()
        {
          foreach (int r in System.Enum.GetValues(typeof(METHOD)))
          {
                rule = r;
                solution = buildSolution();
                if (solution != null)
                {
                    solution = localSearch(solution);
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
        
        override protected Solution buildSolution()
        {
            // Variables 
            Solution s = new Solution(model);          // Initializing a new solution structure solution(vector x, vector y)    
            int selJob;
            bool constructOK = true;

            // Iterative scheduling of jobs based on priority rule  
            do
            {
                // Select the job that has the priority 
                // Implemented in the child class
                selJob = getJobToAssign(s, rule);
                if (selJob >= 0)
                {
                   // MyConsole.display($"Assigning job {selJob}");
                    constructOK = model.assignJob(s, selJob, 0);
                    if (!constructOK)
                        MyConsole.displayError($"Could not assign job {selJob}");
                }

            }
            while (selJob >= 0 && constructOK);

            if (constructOK)
            {
                model.finalize(s);
                return s;
            }
            else
            {
                return null;
            }
      
        }


        protected override Solution localSearch(Solution solution)
        {
            Solution _s             = solution;
            Solution betterSolution = solution; 

            for(int m = 0; m <model.getNumOfMachines(); m++)
            {
                //Removing job from machine. 
                for (int job = 0; job < model.getNumOfOrders(); job++)
                    if (model.orderIsAssignedToMachine(_s, job, m))
                    {
                        model.removeJob(job, _s); 
                    }

                // Reassigning job to machines 
                foreach (int i in System.Enum.GetValues(typeof(METHOD)))
                {
                    int     selJob      = -1;
                    bool    constructOK = true; 

                    do
                    {
                        selJob = getJobToAssign(_s, i);
                        if (selJob >= 0)
                        {
                            MyConsole.display($"Assigning job {selJob}");
                            constructOK = model.assignJobToMachine(_s, selJob,m, 0);
                            if (!constructOK)
                                MyConsole.displayError($"In local search, could not assign job {selJob} to machine {m}");
                        }
                    }
                    while (selJob >= 0 && constructOK);

                    if(constructOK)
                    {
                        if (model.fitness(_s) < model.fitness(betterSolution))
                            betterSolution = _s; 
                    }
                    
                }
            }
            return betterSolution; 
        }


        protected int getJobToAssign(Solution s, int localRule )
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
                switch(localRule)
                {
                    case (int)METHOD.SPT:
                        
                        for (int i = selJob; i < model.getNumOfOrders(); i++)
                            if ((model.pk(selJob) + model.sk(selJob) > model.pk(i) + model.sk(i)) && !model.orderIsAssigned(s, i))
                                selJob = i;
                        break;

                    case (int)METHOD.LPT:
                       
                        for (int i = selJob; i < model.getNumOfOrders(); i++)
                            if ((model.pk(selJob) + model.sk(selJob) < model.pk(i) + model.sk(i)) && !model.orderIsAssigned(s, i))
                                selJob = i;
                        break;


                    case (int)METHOD.EDD:
                        
                        for (int i = selJob; i < model.getNumOfOrders(); i++)
                            if ((model.dk(selJob) > model.dk(i)) && !model.orderIsAssigned(s, i))
                                selJob = i;
                        break;


                    case (int)METHOD.CR:
                       
                        for (int i = selJob; i < model.getNumOfOrders(); i++)
                            if ((model.getCriticalRatio(selJob) > model.getCriticalRatio(i)) && !model.orderIsAssigned(s, i))
                                selJob = i;
                        break;


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
