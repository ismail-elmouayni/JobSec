
/*
 * GRAS.cs
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


using System;
using System.Collections;
using JOBSEC.Scheduling;

namespace JOBSEC
{
    class GRAS : IterativeMethod 
    {
        private float pRCL; 
    
        // class constructor. Uses a mathematical model to construct solution 
        // ...and a excel file containing the input data
        public GRAS(float pRCL, Model model, ExcelFile excelFile) : base (model, excelFile) {
            name = "GRAS";
            this.pRCL = pRCL; 
        }

      

        // An implementation of the greedy construction
        // The build solution is processed iteratively 
        // Iteration scheme is defined in the base class
        // Iterative methode. buildSolution build a new 
        // Solution each time, randomly with greedy rule
        override
        protected Solution buildSolution()
        {
            // Variables 
            Solution    solution        = new Solution(model);          // Initializing a new solution structure solution(vector x, vector y)    
            ArrayList   jobsToAssign    = new ArrayList();              // Array containing non assigned jobs ordered using Critical Ratio index
            int[]       RCL;                                            // Reduced Candidate List 
            int         selOrder;
            bool        constructOK     = true;

            

            // Iterative construction of solution 
           do
           {
                // Using the model and the precedure CR (Critical Ratio), the procedure orderJobs reorders the jobs ....
                // ...using CR. CR is defined under the Model class. Only not assigned job are considered ..............
                jobsToAssign = getJobsToAssign(solution);
               // Console.WriteLine($"-----job to assign : {jobsToAssign.Count}");
                if(jobsToAssign.Count >0)
                {
                    // Creating a Candidate list 
                    RCL = new int[(int)Math.Truncate(jobsToAssign.Count * pRCL)];
                    for (int i = 0; i < RCL.Length; i++)
                        RCL[i] = (int)jobsToAssign[i];

                    // Selecting at random a job to assign 
                    selOrder = RCL[rn.Next(RCL.Length)];
                    constructOK = model.assignJob(solution, selOrder,0);
                }
               

            }
            while (jobsToAssign.Count>0 && constructOK);

            if (constructOK)
            {
                model.finalize(solution);
                //Console.WriteLine($"-----solution fitness : {model.fitness(solution)}");
                return solution;
            }
            else
            {
                MyConsole.displayError("ERROR : Couldn't construct solution ");
                return null;
            }
           
           
        }
        
        override
        protected Solution localSearch(Solution solution)
        {
            Solution    _s,                 // Uncomplete solution  
                        bls = solution ;    // Best local solution 
            int         job = -1;



            //Console.WriteLine("\n---local search "); 
            // Chossing at random, a job to reschadual 
            job = rn.Next(model.getNumOfOrders());
           
            for (int d = 0; d < model.maxDelay(job); d++)
            {
                _s = model.removeJob(job, solution); 
               if(model.assignJob(_s, job, d))
                {
                    model.finalize(_s);
                    if (model.fitness(bls) > model.fitness(_s))
                        bls = _s;
                }
            }
            //Console.WriteLine($"-----solution fitness : {model.fitness(bls)}");
            return bls; 
            
        }
      
        // Return the rest of jobs to be assigned based on 
        // In construction solution provieded as an imput
        private ArrayList getJobsToAssign(Solution solution)
        {
            ArrayList jobsToAssign = new ArrayList(); 
            // Select jobs to assign 
            for (int i = 0; i < model.getNumOfOrders(); i++)
            {
                if (!model.orderIsAssigned(solution, i))
                    jobsToAssign.Add(i);
            }

            // Order job to assign 
            for (int i = 0; i < jobsToAssign.Count - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < jobsToAssign.Count; j++)
                    if ((model.getCriticalRatio((int)jobsToAssign[minIndex]) > model.getCriticalRatio((int)jobsToAssign[j])))
                    {
                        minIndex = j;
                    }
                // Do swap if i is not the index of the ordrer with the smalest CR
                if (minIndex != i)
                {
                    int c = (int)jobsToAssign[i];
                    jobsToAssign[i] = jobsToAssign[minIndex];
                    jobsToAssign[minIndex] = c;
                }

            }

            return jobsToAssign; 

        }

    }
}
