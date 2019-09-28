
/*
 * SecMethod.cs
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
    /*
     * SecMethod class is super class for sechduling methods 
     */
 
    abstract class SecMethod
    {
        protected   Model       model;                                  // Object for mathematical model of optimization 
        protected   AlgoTimer   timer;                                  // Capturing computation time
        protected   ExcelFile   excelFile;                              // IO file, excel format 
        protected   Solution    solution;                               // To store solution or temporary solution in case of iterative method 
        protected   string      name;                                   // Containing the name of the method 

        protected SecMethod(Model model, ExcelFile excelFile)
        {
            timer           = new AlgoTimer();
            this.model      = model;
            this.excelFile  = excelFile; 
        }


        abstract public void apply();
        abstract protected Solution buildSolution(); 

    }
}
