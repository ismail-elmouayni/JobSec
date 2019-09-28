
/*
 * EDD.cs
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
