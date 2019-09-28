
/*
 * IterativeMethod.cs
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




using JOBSEC.Utilities;
using System;

namespace JOBSEC.Scheduling
{
    abstract class IterativeMethod : SecMethod 
    {

        // Constants definition 
        public const int MAX_ITERATION = 10000;

        protected Random    rn;                             // Random variable for random selection 
        protected Curve     fitnessCurve;                   // Capturing fitness evolution when applying the heuristic 
        protected Solution  bestSolution,                   // To store best solutions 
                            initialSolution;     
        
        protected IterativeMethod(Model model, ExcelFile excelFile) : base(model, excelFile)
        {
            rn = new Random();
            fitnessCurve = new Curve(); 
        }

        // Local Search to improve localy the solution build
        protected abstract Solution localSearch(Solution solution);

        override
        public void apply()
        {

            MyConsole.displayMain($"\n\n Applying {name} ----------------------------------: ");
            MyConsole.displayMain($"\n\n-----iteration : 0");

            
            // Starting timer to mesure completion time 
            timer.start();
            // generate initial solution using greedy randomized construction 
            initialSolution = buildSolution(); 
            // At the beginning, the best is the initial solution 
            bestSolution = initialSolution;

            if(initialSolution != null)
            {

                CursorPosition cp = new CursorPosition();
                fitnessCurve.Add(model.fitness(initialSolution), 0);

                for (int i = 1; i < model.getNumOfIterations(); i++)
                {
                    // for display ------------------------------------------------------------------------------------------
                    MyConsole.restCursorPosition(cp);
                    MyConsole.displayMain($"\n\n-----------------------------iteration : {i + 1}/{model.getNumOfIterations()}");
                    MyConsole.display($"-----------------------------constructing solution");
                    // -------------------------------------------------------------------------------------------------------

                    solution = buildSolution();
                    solution = localSearch(solution);
                    // If fitness is improved 
                    updateBestSolution(i);
                }

                timer.stop(excelFile);


                MyConsole.displayMain($"\n best solution reached : {model.fitness(bestSolution)}");
                exportFitnessToExcel();
                //saveInitialSolution(); 
                //saveBestSolution();

            }
            else
                MyConsole.displayError("ERROR : Couldn't find Initial solution. Program breaks ");

        }

        protected void updateBestSolution(int iteration)
        {
            if (model.fitness(solution) < model.fitness(bestSolution))
            {
                bestSolution = solution;
                fitnessCurve.Add(model.fitness(bestSolution), iteration);
            }
        }



        // Exproting fitness data into excelfile
        protected void exportFitnessToExcel()
        {
            // Show fitness evolution summary 
            for (int i = 0; i < fitnessCurve.lenght(); i++)
                MyConsole.displayResult($"fitnessCurve ({fitnessCurve.getY(i)}, {fitnessCurve.getX(i)})");

            // Ask if the user wish to export data into excel file 
            MyConsole.displayMain("Wish you export the result yes (Y)/ no (N)");
            if (MyConsole.readInput().Equals("Y"))
            {
                // Opening IO file for data s
                excelFile.open();
                // Progess bare show the progression of the export 
                ProgressBar progress = new ProgressBar();
                MyConsole.displayResult($"Exporting fitness...");

                for (int i = 0; i < fitnessCurve.lenght(); i++)
                {
                    excelFile.write(fitnessCurve.getX(i), ExcelFile.FITNESS_SHEET_INDEX, i + 1, 1);
                    excelFile.write(fitnessCurve.getY(i), ExcelFile.FITNESS_SHEET_INDEX, i + 1, 2);
                    progress.Report(i / fitnessCurve.lenght());
                }
                // Get rid of the progress bar variable 
                progress.Dispose();
                // closing the excel file / not the application (EXCEL) 
                excelFile.close(); 
                // Notify that data is exported 
                MyConsole.displayResult($"Fitness data exported");
            }
            else
                // Notify that the user do not wish to export data 
                MyConsole.displayMain("Fitness data are not exported");

        }


    


        protected void saveBestSolution()
        {
            ProgressBar progress = new ProgressBar();


            // Saving X
            MyConsole.displayResult("\n\n----Saving bestSolution...\n");
            MyConsole.displayResult("----Saving X, Y, U, R ...\n");
            int K = model.getNumOfOrders();
            int T = model.getHorizon();
            int J = model.getNumOfMachines();

            for (int k = 0; k < K; k++)
            {
                progress.Report((double)(k) / K);
                // Saving X
                for (int t = 0; t < T; t++)
                    excelFile.write(bestSolution.getX()[k][t], ExcelFile.X_SHEET_INDEX, k + 1, t + 1);



                // Saving Y
                for (int j = 0; j < J; j++)
                    excelFile.write(bestSolution.getY()[k][j], ExcelFile.Y_SHEET_INDEX, k + 1, j + 1);

                // Saving U
                excelFile.write(bestSolution.getU()[k], ExcelFile.U_SHEET_INDEX, k + 1, 1);

                // Saving E
                excelFile.write(bestSolution.getE()[k], ExcelFile.E_SHEET_INDEX, k + 1, 1);

                // Saving R
                excelFile.write(bestSolution.getR()[k], ExcelFile.R_SHEET_INDEX, k + 1, 1);


            }

            MyConsole.displayResult("----Saving fatigue and W...");
            for (int t = 0; t < T; t++)
            {
                // Fatigue index
                excelFile.write(t, ExcelFile.F_SHEET_INDEX, t + 1, 1);
                excelFile.write(bestSolution.getF()[t], ExcelFile.F_SHEET_INDEX, t + 1, 2);

                // Worker load 
                excelFile.write(bestSolution.getW()[t], ExcelFile.W_SHEET_INDEX, t + 1, 2);
                excelFile.write(t, ExcelFile.W_SHEET_INDEX, t + 1, 1);
                progress.Report((double)t / T);
            }

            progress.Dispose();

        }

        protected void saveInitialSolution()
        {
            ProgressBar progress = new ProgressBar();


            // Saving X
            MyConsole.displayResult("\n\n----Saving initialSolution...\n");
            MyConsole.displayResult("----Saving X, Y, U, R ...\n");
            int K = model.getNumOfOrders();
            int T = model.getHorizon();
            int J = model.getNumOfMachines();

            for (int k = 0; k < K; k++)
            {
                progress.Report((double)(k) / K);
                // Saving X
                for (int t = 0; t < T; t++)
                    excelFile.write(initialSolution.getX()[k][t], ExcelFile.I_X_SHEET_INDEX, k + 1, t + 1);



                // Saving Y
                for (int j = 0; j < J; j++)
                    excelFile.write(initialSolution.getY()[k][j], ExcelFile.I_Y_SHEET_INDEX, k + 1, j + 1);

                // Saving U
                excelFile.write(initialSolution.getU()[k], ExcelFile.I_U_SHEET_INDEX, k + 1, 1);


                // Saving E
                excelFile.write(initialSolution.getE()[k], ExcelFile.I_E_SHEET_INDEX, k + 1, 1);

                // Saving R
                excelFile.write(initialSolution.getR()[k], ExcelFile.I_R_SHEET_INDEX, k + 1, 1);


            }

            MyConsole.displayResult("\n\n----Saving fatigue and W...");
            for (int t = 0; t < T; t++)
            {
                // Fatigue index
                excelFile.write(t, ExcelFile.I_F_SHEET_INDEX, t + 1, 1);
                excelFile.write(initialSolution.getF()[t], ExcelFile.I_F_SHEET_INDEX, t + 1, 2);

                // Worker load 
                excelFile.write(initialSolution.getW()[t], ExcelFile.I_W_SHEET_INDEX, t + 1, 2);
                excelFile.write(t, ExcelFile.I_W_SHEET_INDEX, t + 1, 1);
                progress.Report((double)t / T);
            }

            progress.Dispose();
        }

    }
}
