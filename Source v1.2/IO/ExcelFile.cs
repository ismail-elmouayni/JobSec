﻿using System;
using Microsoft.Office.Interop.Excel; 



namespace JOBSEC
{
    public class ExcelFile
    {

        private string          path;                                       // Variable containing the path to the excel file 
        private Workbook        wb;                                         // Variable containing the workbook object related to the file
        private _Application    excel;                                      // Variable containing the application excel launched 
        private bool            wbIsOpen        = false;                    // State variable giving the state of the file : opened or not 
        private int             fitnessUpdate   = 0;                        // Variable for number of fitness save 


        // Class constructor using the excelfile path to call excel 
        // application and read / write excel file
        public ExcelFile(string path)
        {
            this.path = path;                                               // Initialize workbook path 
            try                                                             // try to open an excel application 
            {
                excel = new Microsoft.Office.Interop.Excel.Application();   
            }
            catch (Exception e)
            {
                MyConsole.displayError(e.ToString());   
            }
        }


        // Returns true when Excel application 
        // is successfully loaded in memory
        public Boolean AppIsLoaded()
        {
            return (!(excel != null)); 
        }


        // Opens workbook for data read-write 
        public void open()
        {
            if (excel==null)
            {
                MyConsole.displayError("Couldn't create an excel application instance");
            }
            else
            {
                try
                {
                    wb = excel.Workbooks.Open(path);
                    wbIsOpen = true;
                }
                catch (Exception e)
                {
                    MyConsole.displayError(e.ToString());
                }
            }
      
        }

        // Read the content of a cell in the workbook 
        public double readCell(int sheetIndex, int row, int colomn)
        {
            if (wb != null)
            {
                try
                {
                    return wb.Worksheets[sheetIndex].Cells[row, colomn].Value2;
                }
                catch( Exception e)
                {
                    MyConsole.displayError($"can't read sheet {sheetIndex}, cell {row},{colomn}");
                    MyConsole.displayError(e.ToString());
                    return READ_OPERATION_FAILED;
                }
                
            }
            else
            {
                MyConsole.displayError("Can't read a cell, the workbook couldn't be initilized or excel file is not open");
                MyConsole.displayError("Use instruction excelFile.open()");
                return READ_OPERATION_FAILED; 
            }
        }


        // Write a float in a cell of the workbook (excel file) 
        public void writeCell(float value, int sheetIndex, int row, int colomn)
        {
            if (wb != null)
            {
                wb.Worksheets[sheetIndex].Cells[row, colomn].Value2 = value;
                wb.Save();
            }
            else
            {
                MyConsole.displayError("Can't read a cell, the workbook couldn't be initilized or excel file is not open");
                MyConsole.displayError("Use instruction excelFile.open()");
            }
          
        }

        // Write  a string in a cell of the workbook (excelFile) 
        public void writeCell(String value, int sheetIndex, int row, int colomn)
        {
            if (wb != null)
            {
                wb.Worksheets[sheetIndex].Cells[row, colomn].Value2 = value;
                wb.Save();
            }
            else
            {
                MyConsole.displayError("Can't read a cell, the workbook couldn't be initilized or excel file is not open");
                MyConsole.displayError("Use instruction excelFile.open()");
            }

        }


        // Returns the number of columns of a sheet 
        public int endColomns(int sheetIndex)
        {
            return wb.Worksheets[sheetIndex].Colomns.Count; 
        }

        // Returns the number of rows of a sheet 
        public int endRows(int sheetIndex)
        {
            return wb.Worksheets[sheetIndex].Rows.Count;
        }

        // Number of fitness upadates after an enhancement
        public int getNumOfFitnessUpdate()
        {
            return fitnessUpdate; 
        }


        // Set the number of fitness updates 
        public void setNumOfFitnessUpdate(int fitnessUpdate)
        {
            this.fitnessUpdate = fitnessUpdate; 
        }

        // Close the work book 
        public void close ()
        {
            wb.Close();
            wbIsOpen = false; 
        }


        // Class destructor ending to 
        // excel application closing 
        ~ExcelFile() { excel.Quit();  }


        // EXCEL FILE sheet indices  ------------------------------------- 
        // Can be enhanced by looking at sheets labels

        public const int        READ_OPERATION_FAILED                   = -1,
                                MAIN_SHEET_INDEX                        = 2,
                                QUANTITY_PER_ORDER_SHEET_INDEX          = 3,
                                REFERENCE_RELASE_DATE_SHEET_INDEX       = 4,
                                l_SHEET_INDEX                           = 5,
                                REFERENCE_DUE_DATE_SHEET_INDEX          = 6,
                                REFERENCE_PROCESSING_SHEET_INDEX        = 7,
                                REFERENCE_SETTING_TIME_SHEET_INDEX      = 8,
                                REFERENCE_PRIORITY_SHEET_INDEX          = 9,
                                REFERENCE_DIFFICULTY_SHEET_INDEX        = 10,
                                REFERENCE_STRESS_FACTORS_SHEET_INDEX    = 11,
                                REFERENCE_RECOVERY_RATE_SHEET_INDEX     = 12,
                                MACHINE_LOAD_SHEET_INDEX                = 13,
                                FITNESS_SHEET_INDEX                     = 15,
                                REFERENCE_PER_ORDER_SHEET_INDEX         = 14,
                                X_SHEET_INDEX                           = 16,
                                Y_SHEET_INDEX                           = 17,
                                U_SHEET_INDEX                           = 18,
                                F_SHEET_INDEX                           = 19,
                                R_SHEET_INDEX                           = 20,
                                E_SHEET_INDEX                           = 21,
                                W_SHEET_INDEX                           = 22,
                                I_F_SHEET_INDEX                         = 23,
                                I_R_SHEET_INDEX                         = 24,
                                I_U_SHEET_INDEX                         = 25,
                                I_X_SHEET_INDEX                         = 26,
                                I_Y_SHEET_INDEX                         = 27,
                                I_V_SHEET_INDEX                         = 28,
                                I_W_SHEET_INDEX                         = 29,
                                I_E_SHEET_INDEX                         = 30,
                                ALGO_METRICS_SHEET_INDEX                = 31;
    }

}
