
/*
 * Core.cs
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
using System;
using System.IO;
using System.Reflection;

namespace JOBSEC
{
    class Core
    {
        //private static GRAS method;
        private static String       version = "01";
        private static Model        model;
        private static ExcelFile    excelFile; 



        static void Main(string[] args)
        {

            if (args.Length > 0)
            {
                //  There is input, processing it 
                actionSelection(args[0]);

            }
            else
            {
                // No input, presenting the tool and then asking for instruction
                actionSelection("--information");
                // Asking for an option 
                string option; 

                do
                {
                    MyConsole.display("Please, enter a commande : "); 
                    option = MyConsole.readInput().Replace(" ",string.Empty);

                    // Processing the option 
                    actionSelection(option);

                }
                while (!option.Equals("--quit"));
                MyConsole.readInput(); 
            }

        }


      

        public static void actionSelection(String option )
        {
            String mainMessage =    "JOBSEC V" + version + "--------------------------------------------------\n" +
                                    "Designed by Ismail EL MOUANI- all rights reserved-----------\n\n\n" +


                                    "List of commands"+
                                    "--information  : display information regarding the tool.....\n" +
                                    "--schedule     : applying one of the method to schedule jobs\n" +
                                    "                 selection is done in a second time.........\n" +
                                    "--changeIO     : changing the IO Excel file.................\n" +
                                    "--readIOFile   : reding Input-Output file. Mandatory before.\n" +
                                    "                 scheduling\n"+
                                    "--quit         : Quiting JOBSEC.............................\n" ;
         
            switch (option)
            {
                
                case "--schedule":
                    schedule();     
                    break;

                case "--information":
                    MyConsole.displayMain(mainMessage);
                    break;

                case "--changeIO":
                    MyConsole.displayResult("changingIO");
                    break;

                case "--readIOFile":
                    string outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                    string excelFilePath = Path.Combine(outPutDirectory, "res\\ExcelFile.xlsx");
                    excelFile = new ExcelFile(excelFilePath);
                    model = new Model(excelFile);
                    if (excelFile == null || model == null)
                    {
                        MyConsole.displayError("Couldn't red input file and/or initialize scheduling schema....");
                    }
                    break; 


                case "--quit":
                    MyConsole.displayResult("quiting, press a key...");
                    break; 

                default:
                    MyConsole.displayResult("input parameter not identified !\n");
                    break; 
            }

        }




        public static void schedule()
        {

            if (excelFile == null || model == null)
            {
                MyConsole.displayError("model and/or IO file are not initialized yet\n" +
                                       "Please type --readIOFile");
            }

            else
            {
                MyConsole.display("Select the scheduling method : --GRAS\n" +
                        "                               --SPT\n" +
                        "                               --LPT\n" +
                        "                               --EDD\n" +
                        "                               --CR\n"  +
                        "                               --SES\n"+
                        "                               --SALCK\n"+
                        "                                Abort(A)");

                string methodChoice = MyConsole.readInput().Replace(" ", string.Empty);

                applySecMethod(model, excelFile, methodChoice);

            }

        }

        static public void applySecMethod(Model model, ExcelFile excelFile, string methodChoice)
        {
            switch (methodChoice)
            {
                case "--GRAS":
                    GRAS methodGRAS = new GRAS(1f,model, excelFile);
                    methodGRAS.apply();
                    break;

                case "--SPT":
                    SPT methodSPT = new SPT( model, excelFile);
                    methodSPT.apply();
                    break;

                case "--LPT":
                    LPT methodLPT = new LPT(model, excelFile);
                    methodLPT.apply();
                    break;

                case "--EDD":
                    EDD methodEDD = new EDD(model, excelFile);
                    methodEDD.apply();
                    break;

                case "--CR":
                    CR methodCR = new CR(model, excelFile);
                    methodCR.apply();
                    break;

                case "--SES":
                    SES methodSES = new SES(model, excelFile);
                    methodSES.apply();
                    break;

                case "--SLACK":
                    SES methodSLACK = new SES(model, excelFile);
                    methodSLACK.apply();
                    break;

                case "A":
                    MyConsole.displayError("Aborting operation...\n");
                    break;
                default:
                    {
                        MyConsole.displayError("Input invalid, applying GRAS anyway... \n");
                        GRAS methodDefault = new GRAS(1f, model, excelFile);
                        methodDefault.apply();
                    }
                    break;
            }

        }

    }


}
