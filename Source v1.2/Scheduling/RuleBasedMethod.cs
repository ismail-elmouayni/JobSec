
/*
 * RuleBasedMethod.cs
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
    abstract class RuleBasedMethod : SecMethod
    {

        public RuleBasedMethod(Model model, ExcelFile excelFile) : base(model, excelFile) { }

       // Put the selection rule in the child classes 
        abstract protected int getJobToAssign(Solution s);


        override public void apply()
        {
            MyConsole.displayMain($"\n\n Applying {name} ----------------------------------: ");
            solution = buildSolution();
            if (solution != null)
                MyConsole.displayResult($"-----solution fitness : {model.fitness(solution)}");
            else
                MyConsole.displayError("ERROR : Couldn't construct solution ");
        }


        override
        protected Solution buildSolution()
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
                selJob = getJobToAssign(s);
                if (selJob >= 0)
                {
                    MyConsole.display($"Assigning job {selJob}"); 
                    constructOK = model.assignJob(s, selJob, 0);
                    if(!constructOK)
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

    }
}
