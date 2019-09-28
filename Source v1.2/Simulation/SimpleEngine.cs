
/*
 * SimpleEngine.cs
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

using JOBSEC.Scheduling;
using JOBSEC.Utilities;
using System.Collections;

namespace JOBSEC
{

    /// <summary>
    /// The class simulation engine simulates rescheduling of the jobs by
    /// considering random arrivals
    /// </summary>
    class SimpleEngine
    {
        SecMethod   method;
        Model       model;
        Solution    solution;
        Curve       fintnessHistory; 

        // Structure to capture arrival event
        private struct ArrivalEvent
        {
            int arrivalTime;
            int quantity;
            int reference;
        }

         private ArrayList LFA = new ArrayList();


        SimpleEngine(SecMethod method, Model model, Solution solution)
        {
            this.model      = model;
            this.method     = method;
            this.solution   = solution; 
        }
    }

}






    










