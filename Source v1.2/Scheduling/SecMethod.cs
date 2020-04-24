


namespace JOBSEC.Scheduling
{
    /*
     * SecMethod class is base class for sechduling methods 
     */
 
    abstract class SecMethod
    {
        protected   Model       model;                                  // Mathematical model describing the addresed optimization problem  
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

        // virtual method consisting on applying the method 
        abstract public void apply();

        // virtual method consisting of building the solution 
        // using this method
        abstract protected Solution buildSolution(); 

    }
}
