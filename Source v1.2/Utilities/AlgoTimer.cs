using System;
// Providing time mertrics for an algorithm

namespace JOBSEC
{
    public class AlgoTimer
    {
        private DateTime startingTime = new DateTime();
    


        public AlgoTimer()
        {
          
        }

        public void start()
        {
            startingTime = DateTime.Now;
        }


        public void stop(ExcelFile file)
        {
            TimeSpan timeSpan= DateTime.Now - startingTime;
            Console.WriteLine($"algo computation time {(timeSpan).TotalSeconds.ToString("0.000")} s");
            file.open(); 
            file.write("computation time (s) ", ExcelFile.ALGO_METRICS_SHEET_INDEX, 1, 1);
            file.write((timeSpan).TotalSeconds.ToString("0.000"), ExcelFile.ALGO_METRICS_SHEET_INDEX, 1, 2);
            file.close(); 
        }


    }
}
