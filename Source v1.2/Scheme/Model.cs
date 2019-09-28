// Class containing the mathematical model used for 
// KANBAN system modeling. 



using System;


namespace JOBSEC
{
    class Model
    {
        // Indices definition 
        private int         i = 0,                  // Reference index 
                            k = 0,                  // Order index 
                            j = 0,                  // Machine index 
                            t = 0;                  // period index 

        // Indices limits 
        private int         I = 0,                  // Number of references 
                            K = 0,                  // Number of Orders at reschaduling 
                            J = 0,                  // Number of machines 
                            T = 0,                  // Planing horizon 
                            numOfIterations = 1,    // Number of iteration using the solving method    
                            S = 0;                  // Number of stress factors 
        
        private double      dt = 0,                 // Period lenght 
                            C = 1;                  // Constant of the HEP model  
                           

        // Input variables 
        // One dimension 
        private int[]       q,                      // Quantity per order k : q(k)
                            a,                      // Order k arrival date : a(k)
                            l,                      // time waiting at rescheduling l(k)
                            d,                      // Absolute due date per reference i : d(i)
                            p,                      // Processing time of the reference i : p(i)
                            s;                      // Set up time of the reference i : s(i)
        private float[]     lamda,                  // Fatigue rate when preparting a machine for reference i : lamda(i) 
                            deta,                   // Difficulty of preparing a machine for reference i processing : deta(i)
                            mu;                     // Fatigue recovery rate after processing setting a machine for reference i : mu(i)  

        private float[][]   alpha;                  // Priority of the reference i : alpha(i)

        // Two dimensions 
        private int[][]     v,                      // System load at reschaduling : state of the machine j at period t : v(t,j)
                            z,                      // Equals 1 if the order k concerns the reference i : z(k,i) 
                            f;                      // Stressors level induced by a reference processing
                    


       

        // Constructor : initializing the indices limits ...
        // ...and input variables values 
        public Model(ExcelFile excelFile)
        {
            readInputs(excelFile);
            if (checkInputConstraints())
            {
                MyConsole.displayResult("Do you wish to show the input read ? Yes(Y)/ No (N)"); 
                if(MyConsole.readInput().Equals("Y") || MyConsole.readInput().ToLower().Equals("yes"))
                    showInputs();
                
            }
            else
            {
                MyConsole.displayError("ERROR : input are not consitent ");
            }
               
        }



        // Reading input variables 
        public void readInputs(ExcelFile excelFile)
        {

            // Reading input from the excel file
            MyConsole.displayMain("Reading input file ....");
     

            excelFile.open();

            I               = Convert.ToInt32(excelFile.read(ExcelFile.MAIN_SHEET_INDEX, 1, 2));
            K               = Convert.ToInt32(excelFile.read(ExcelFile.MAIN_SHEET_INDEX, 2, 2));
            J               = Convert.ToInt32(excelFile.read(ExcelFile.MAIN_SHEET_INDEX, 3, 2));
            T               = Convert.ToInt32(excelFile.read(ExcelFile.MAIN_SHEET_INDEX, 4, 2));
            dt              = excelFile.read(ExcelFile.MAIN_SHEET_INDEX, 5, 2);
            C               = excelFile.read(ExcelFile.MAIN_SHEET_INDEX, 6, 2);
            numOfIterations = Convert.ToInt32(excelFile.read(ExcelFile.MAIN_SHEET_INDEX, 7, 2));
            S               = Convert.ToInt32(excelFile.read(ExcelFile.MAIN_SHEET_INDEX, 8, 2));

            MyConsole.display("\nReading job batch sizes 'q'.... \n");
            q = new int[K];
            for (int compt = 0; compt < K; compt++)
            {
                q[compt] = Convert.ToInt32(excelFile.read(ExcelFile.QUANTITY_PER_ORDER_SHEET_INDEX, compt + 1, 1));
                
            }

            MyConsole.display("\nRelease dates 'a'.... \n");
            a = new int[K];
            for (int compt = 0; compt < K; compt++)
            {
                a[compt] = Convert.ToInt32(excelFile.read(ExcelFile.REFERENCE_RELASE_DATE_SHEET_INDEX, compt + 1, 1));
              
            }

            MyConsole.display("\nRelease dates 'l'.... \n");
            l = new int[K];
            for (int compt = 0; compt < K; compt++)
            {
                l[compt] = Convert.ToInt32(excelFile.read(ExcelFile.l_SHEET_INDEX, compt + 1, 1));

            }
            MyConsole.display("\nReading absolute due time 'd'.....\n");
            d = new int[I];
            for (int compt = 0; compt < I; compt++)
            {
                d[compt] = Convert.ToInt32(excelFile.read(ExcelFile.REFERENCE_DUE_DATE_SHEET_INDEX, compt + 1, 1));
           
            }

            Console.WriteLine("\nProcessing times 'p'.....\n");
            p = new int[I];
            for (int compt = 0; compt < I; compt++)
            {
                p[compt] = Convert.ToInt32(excelFile.read(ExcelFile.REFERENCE_PROCESSING_SHEET_INDEX, compt + 1, 1));
               
            }

            MyConsole.display("\nSetup times 's'....\n");
            s = new int[I];
            for (int compt = 0; compt < I; compt++)
            {
                s[compt] = Convert.ToInt32(excelFile.read(ExcelFile.REFERENCE_SETTING_TIME_SHEET_INDEX, compt + 1, 1));
               
            }

            MyConsole.display("\nReading references fitness weights 'alpha'\n");
            alpha = new float[I][];
            
            for (int compt = 0; compt < I; compt++)
            {
                alpha[compt] = new float[3];   
            }
         
            for (int compt = 0; compt < I; compt++)
            {
                alpha[compt][0] = (float)excelFile.read(ExcelFile.REFERENCE_PRIORITY_SHEET_INDEX, compt + 1, 1);
                alpha[compt][1] = (float)excelFile.read(ExcelFile.REFERENCE_PRIORITY_SHEET_INDEX, compt + 1, 2);
                alpha[compt][2] = (float)excelFile.read(ExcelFile.REFERENCE_PRIORITY_SHEET_INDEX, compt + 1, 3);

            }

            MyConsole.display("\nReading task difficulties 'deta'....\n");
            deta = new float[I];
            for (int compt = 0; compt < I; compt++)
            {
                deta[compt] = (float)excelFile.read(ExcelFile.REFERENCE_DIFFICULTY_SHEET_INDEX, compt + 1, 1);
              
            }


            MyConsole.display("\nReading fatigue rate 'lamda'....\n");
            lamda   = new float[I];
            mu      = new float[I];
            FatigueParametersGenerators fpg = new FatigueParametersGenerators();

            for (int compt = 0; compt < I; compt++)
            {
                float[] stressFactors = new float[S]; 
                for (int sfi = 0; sfi < S; sfi++)                 // sfi : Stress Factor Index 
                {
                    stressFactors[sfi] = Convert.ToSingle(excelFile.read(ExcelFile.REFERENCE_STRESS_FACTORS_SHEET_INDEX, sfi+2, compt+2));
                }

                lamda[compt]    = fpg.genFatigueRate(stressFactors)[FatigueParametersGenerators.IFATIGUE_RATE]; 
                mu[compt]       = fpg.genFatigueRate(stressFactors)[FatigueParametersGenerators.IRECOVERY_RATE];
            }

            /*MyConsole.display("\nReading recovery rate 'mu'....\n");
         
            for (int compt = 0; compt < I; compt++)
            {
                mu[compt] = excelFile.read(ExcelFile.REFERENCE_RECOVERY_RATE_SHEET_INDEX, compt + 1, 1);
            
            }*/

            MyConsole.display("\nReading machine states 'v'....\n");
            v = new int[T][];
            for (int compt = 0; compt < T; compt++)
            {
                v[compt] = new int[J];
             
            }

            ProgressBar progress = new ProgressBar(); 
            {
                for (int compt = 0; compt < T; compt++)
                {
                    for (int compt2 = 0; compt2 < J; compt2++)
                        v[compt][compt2] = Convert.ToInt32(excelFile.read(ExcelFile.MACHINE_LOAD_SHEET_INDEX, compt + 1, compt2 + 1));
                    progress.Report((double)compt / T); 
                }

                progress.Dispose(); 

            }


            MyConsole.display("\nReading job's references 'z'....\n");
            z = new int[K][];
            for (int compt = 0; compt < K; compt++)
            {
                z[compt] = new int[I];
            }

            for (int compt = 0; compt < K; compt++)
            {
                for (int compt2 = 0; compt2 < I; compt2++)
                    z[compt][compt2] = Convert.ToInt32(excelFile.read(ExcelFile.REFERENCE_PER_ORDER_SHEET_INDEX, compt + 1, compt2 + 1));
            }

            excelFile.close();
        }

        public int[][] getInitialMachinesLoad()
        {
            return v; 
        }

        public void showInputs()
        {
            MyConsole.display("Inputs ................");
            MyConsole.display($"Number of references I = {I}");
            MyConsole.display($"Number of orders K = {K}");
            MyConsole.display($"Number of workstations J = {J}");
            MyConsole.display($"Horizon T = {T}");
            MyConsole.display($"HEP coefficient C = {C}");

            MyConsole.display(" Quantity per order : ");
            for (int compt = 0; compt < K; compt++)
                MyConsole.display($" q({compt}) = {q[compt]} ");


            MyConsole.display(" release dates : ");
            for (int compt = 0; compt < K; compt++)
                MyConsole.display($" a({compt}) = {a[compt]} ");

            MyConsole.display(" time in queue at rescheduling : ");
            for (int compt = 0; compt < K; compt++)
                MyConsole.display($" l({compt}) = {l[compt]} ");


            MyConsole.display(" References' due date : ");
            for (int compt = 0; compt < I; compt++)
                MyConsole.display($" d({compt}) = {d[compt]} ");

            MyConsole.display(" References' processing times : ");
            for (int compt = 0; compt < I; compt++)
                MyConsole.display($" p({compt}) = {p[compt]} ");

            MyConsole.display(" References' setting times : ");
            for (int compt = 0; compt < I; compt++)
                MyConsole.display($" s({compt}) = {s[compt]} ");


            MyConsole.display(" References' priorities : ");
            for (int compt = 0; compt < I; compt++)
            {
                MyConsole.display($" alpha({compt},1) = {alpha[compt][0]} ");
                MyConsole.display($" alpha({compt},2) = {alpha[compt][1]} ");
            }

            MyConsole.display(" References' difficulties : ");
            for (int compt = 0; compt < I; compt++)
            {
                MyConsole.display($" deta({compt}) = {deta[compt]} ");
            }


            MyConsole.display(" References' fatigue rate  : ");
            for (int compt = 0; compt < I; compt++)
                MyConsole.display($" lamda({compt}) = {lamda[compt]} ");


            MyConsole.display(" References' recovery rate : ");
            for (int compt = 0; compt < I; compt++)
                MyConsole.display($" mu({compt}) = {mu[compt]} ");


            MyConsole.display(" Machine initial load : ");
            for (int compt = 0; compt < T; compt++)
            {
                MyConsole.display("\n");
                for (int compt2 = 0; compt2 < J; compt2++)
                    MyConsole.display($" v({compt}, {compt2}) = {v[compt][compt2]}\t");
            }

            MyConsole.display("\n Reference per order : ");
            for (int compt = 0; compt < K; compt++)
            {
                MyConsole.display("\n");
                for (int compt2 = 0; compt2 < I; compt2++)
                    MyConsole.display($" z({compt}, {compt2}) = {z[compt][compt2]}\t");
            }


        }


        // Check feasibility constraints 
        public Boolean checkInputConstraints()
        {
            bool checkOK = false;
            checkOK = isOrderConsistent();
            return checkOK;
        }

        // Compute decision variables when feasability is checked 
        public void computeDecision()
        {

        }

        // Returns the objective function value 
        public float objectiveFunction()
        {
            return 1;
        }

        // Checking if an order contains only one reference
        private bool isOrderConsistent()
        {
            Console.WriteLine("\n Checking orders consistency");

            for (int k = 0; k < K; k++)
            {
                int s = 0;
                for (int i = 0; i < I; i++)
                    s = z[k][i] + s;

                if (s != 1)
                {
                    Console.Write("Orders are not consitent");
                    return false;
                }
            }

            Console.WriteLine("\n Orders consistency checked with success");
            return true;
        }

        public bool orderIsAssigned(Solution solution, int order)
        {

            int s = 0;
            for (int t = 0; t < T; t++)
                s = solution.getX()[order][t] + s;

            if (s == 0)
            {
                return false;
            }
            else
                return true;
        }

        public bool orderIsAssignedToMachine(Solution solution, int order, int machine)
        {

            if (solution.getY()[order][machine] == 1)
                return true;
            else
                return false; 
              
        }

        public bool assignJob(Solution solution, int selJob, int delay)
        {
            // It assignes the job to the first available sery of periods at an available machine
            bool jobAssigned = true;
            int t;                                  // Index for time line 
            int m = 0;                              // Index for machine checking 

            // Console.WriteLine($"currentPeriod :{currentPeriod} ");

            for (t = a[selJob]+ delay; t < T - pk(selJob) - sk(selJob); t++)
            {
                //Console.WriteLine(T - pk(selJob) - sk(selJob));
                // Checking for available machine
                for (m = 0; m < J; m++)
                {
                    int _v = 0;                             // Variable containing sum(v(t,j)) during pk + sk  
                    int _w = 0;                             // Variable containing sum(w(t)) during sk
                    jobAssigned = true;                     // Set to true if the job is assigned to a machine 

                    // Compute sum(v(t,j)) for a certain j 
                    for (int tCursor = t; tCursor < ( t + pk(selJob) + sk(selJob)) && tCursor < T; tCursor++)
                    {
                        _v = solution.getV()[tCursor][m] + _v;
                    }

                    // Compute sum(w(t)) for a certain j  during  sk 
                    for (int tCursor = t; (tCursor < t + sk(selJob)) && tCursor < T ; tCursor++)
                    {
                        _w = solution.getW()[tCursor] + _w;
                    }

                    // Check if machine is used during t + pk(selJob) + sk(selJob) or ...
                    // ... worker is setting a machine during t + sk
                    if (_w + _v > 0)
                    {
                        jobAssigned = false;
                    }

                    // If jobAssigned to machine, break from the loop 
                    if (jobAssigned)
                        break;
                }
                if (jobAssigned)
                    break;
            }

            if (!jobAssigned)
            {
                //Console.WriteLine($"------Unfeasable : could'nt assign {selJob} to any machine!");
                return false;
            }
            else
            {
                solution.getY()[selJob][m] = 1;
                //Console.WriteLine($"-----job {selJob} is assigned to machine {m} ");

                for (int tCursor = t; (tCursor < t + pk(selJob) + sk(selJob)) && tCursor < T; tCursor++)
                {
                    solution.getX()[selJob][tCursor] = 1;                                           // Setting job processing period 
                    solution.getV()[tCursor][m] = 1;                                                // Setting machine as busy 
                    //Console.WriteLine($"x{selJob}{tCursor} = {solution.getX()[selJob][tCursor]}");
                }

                for (int tCursor = t; (tCursor < t + sk(selJob)) && tCursor < T ; tCursor++)
                {
                    solution.getW()[tCursor] = 1;
                    //Console.WriteLine($"w{t} = {solution.getW()[t]}"); 
                }

                return true;
            }


        }


        public bool assignJobToMachine(Solution solution, int selJob, int machine, int delay)
        {
            // It assignes the job to the first available sery of periods at an available machine
            bool jobAssigned = true;
            int t;                                  // Index for time line 
          

            // Console.WriteLine($"currentPeriod :{currentPeriod} ");

            for (t = a[selJob] + delay; t < T - pk(selJob) - sk(selJob); t++)
            {
                
                    int _v = 0;                             // Variable containing sum(v(t,j)) during pk + sk  
                    int _w = 0;                             // Variable containing sum(w(t)) during sk
                    jobAssigned = true;                     // Set to true if the job is assigned to a machine 

                    // Compute sum(v(t,j)) for a certain j 
                    for (int tCursor = t; tCursor < (t + pk(selJob) + sk(selJob)) && tCursor < T; tCursor++)
                    {
                        _v = solution.getV()[tCursor][machine] + _v;
                    }

                    // Compute sum(w(t)) for a certain j  during  sk 
                    for (int tCursor = t; (tCursor < t + sk(selJob)) && tCursor < T; tCursor++)
                    {
                        _w = solution.getW()[tCursor] + _w;
                    }

                    // Check if machine is used during t + pk(selJob) + sk(selJob) or ...
                    // ... worker is setting a machine during t + sk
                    if (_w + _v > 0)
                    {
                        jobAssigned = false;
                    }

                    // If jobAssigned to machine, break from the loop 
                    if (jobAssigned)
                        break;
            }

            if (!jobAssigned)
            {
                //Console.WriteLine($"------Unfeasable : could'nt assign {selJob} to any machine!");
                return false;
            }
            else
            {
                solution.getY()[selJob][machine] = 1;
                //Console.WriteLine($"-----job {selJob} is assigned to machine {m} ");

                for (int tCursor = t; (tCursor < t + pk(selJob) + sk(selJob)) && tCursor < T; tCursor++)
                {
                    solution.getX()[selJob][tCursor] = 1;                                           // Setting job processing period 
                    solution.getV()[tCursor][machine] = 1;                                          // Setting machine as busy 
                    //Console.WriteLine($"x{selJob}{tCursor} = {solution.getX()[selJob][tCursor]}");
                }

                for (int tCursor = t; (tCursor < t + sk(selJob)) && tCursor < T; tCursor++)
                {
                    solution.getW()[tCursor] = 1;
                    //Console.WriteLine($"w{t} = {solution.getW()[t]}"); 
                }

                return true;
            }


        }

        public int maxDelay(int job)
        {
            return Math.Max(0,T-a[job]- (int) (pk(job) + sk(job))); 
        }


        public int getNumOfReferences()
        {
            return I;
        }

        // Retruns the number of orders 
        public int getNumOfOrders()
        {
            return K;
        }

        // Returns the number of machines 
        public int getNumOfMachines()
        {
            return J;
        }

        // Returns the planning horizon : T
        public int getHorizon()
        {
            return T;
        }

        // Returns the order cost used for orders selection
        // ... the optimization method
        public float getCriticalRatio(int orderIndex)
        {
            //Console.WriteLine($"CR for {orderIndex} with pk = {pk(orderIndex)}, sk = {sk(orderIndex)} dk = {dk(orderIndex)}");
            return dk(orderIndex) / (pk(orderIndex) + sk(orderIndex));
        }


        // Returns the relative due date relative to ...
        // ... a given order k 
        public int dk(int k)
        {
            int dk = 0;
            for (int i = 0; i < I; i++)
                dk = z[k][i] * (d[i]*q[k] - l[k]) + dk;

            return dk;
        }


        // Return the processing time relative to ...
        // ... a given order k 
        public float pk(int k)
        {
            float pk = 0;

            for (int i = 0; i < I; i++)
                pk = z[k][i]* p[i]*q[k] + pk;

            return pk;

        }

        // Returns the setting time relative to an...
        // ...order k
        public float sk(int k)
        {
            float sk = 0;

            for (int i = 0; i < I; i++)
                sk = z[k][i] * s[i] + sk;

            return sk;
        }

        

        // Finalizing solution, caluclating fatigue ...
        // ... and rejected parts
        public void finalize(Solution solution)
        {
            //Console.WriteLine("-----Finalizing solution");
            double _lamda = 0,
                   _mu = 0.03;

            // Fatigue calculation 
            for (int t = 0; t < T - 1; t++)
            {

                if (solution.getW()[t] == 1)
                {
                    // Computing fatigue rate 
                    for (int k = 0; k < K; k++)
                        for (int i = 0; i < I; i++)
                            _lamda = solution.getX()[k][t] * z[k][i] * lamda[i] + _lamda;
                    // Computing fatigue index 
                    solution.getF()[t + 1] = (float)(1 - (1 - solution.getF()[t]) * Math.Exp(-_lamda));

                }
                else
                {
                    int prOrder = -1;
                    //  Find last order executed 
                    for (int tCursor = t; tCursor >= 0; tCursor--)
                        for (int k = 0; k < K; k++)
                        {
                            if (solution.getX()[k][tCursor] == 1 && solution.getW()[tCursor] == 1)
                                prOrder = k;
                        }

                    // If no work is made before, use default fatigue rate 
                    if (prOrder < 0)
                        _mu = DEFAULT_FATIGUE_RATE;
                    else
                    {
                        // Compute recovery rate 
                        for (int i = 0; i < I; i++)
                            _mu = solution.getX()[k][t] * z[k][i] * mu[i] + _mu;
                    }


                    // Update fatigue index 
                    solution.getF()[t + 1] = (float)(solution.getF()[t] * Math.Exp(-_mu));
                }
                // Ploting results 
                //Console.WriteLine($"F[{t + 1}] : {solution.getF()[t + 1]}");


            }

            // Rejected part proportion calculation 
            for (int k = 0; k < K; k++)
            {
                // Compute task difficulty per order k
                double _detak = 0;
                for (int i = 0; i < I; i++)
                    _detak = deta[i] * z[k][i] + _detak;

                //Console.WriteLine($"deta{k} = {_detak}");

                //Compute average fatigue 
                double maxF = 0;
                int endSet = T-1;
                for (int t = 0; t < T-1; t++)
                {
                    if (solution.getX()[k][t] == 1 && solution.getX()[k][t + 1] == 0)
                    {
                        endSet = t;
                        break;
                    }
                }
                
                //avgF =  solution.getX()[k][t] * solution.getW()[t] * solution.getF()[t];  // Compute the sum of fatigue index during setting 
                maxF = solution.getF()[endSet];
                // Devid by setting lenght 
                //avgF = avgF / sk(k);
                //Console.WriteLine($"maxF[{k}] : {maxF}");                                                              

                solution.getR()[k] = (float)(Math.Truncate(q[k] * C * Math.Pow(maxF, _detak)));
                //Console.WriteLine($" r[{k}] : {solution.getR()[k]}");

                // Number of delays 
                solution.getU()[k] = 0;
                for (int t = dk(k); t < T; t++)
                {
                    solution.getU()[k] = solution.getX()[k][t] + solution.getU()[k];
                }

                // Number of earliness 
                solution.getE()[k] = 0;
                for (int t = 0; (t < dk(k)) && t < T; t++)
                {
                    solution.getE()[k] = solution.getX()[k][t] + solution.getE()[k];
                }
                //Console.WriteLine($"tardness for {k} : {solution.getU()[k]}");
            }

        }

       

        public float fitness(Solution solution)
        {
            float f = 0;                        // Fitness
            for (int k = 0; k < K; k++)
            {
                // Compute order priority
                float  _alpha1 = 0,
                       _alpha2 = 0,
                       _alpha3 = 0;

                for (int i = 0; i < I; i++)
                {
                    _alpha1 = _alpha1 + z[k][i] * alpha[i][0];
                    _alpha2 = _alpha2 + z[k][i] * alpha[i][1];
                    _alpha3 = _alpha3 + z[k][i] * alpha[i][2];
                }

                f = f + _alpha1 * solution.getR()[k] + _alpha2 * solution.getU()[k] + _alpha3*solution.getE()[k];
            }
            return f; 
        }

        public Solution removeJob(int job, Solution s)
        {
            Solution _s = new Solution(this);

            
            int startTime   = -1;
            int m           = -1;

            // Find job starting time
            for (int t = 0; t < T; t++)
            {
                if (s.getX()[job][t] == 1)
                {
                    startTime = t;
                    break;
                }
            }

            // Find machine used for the job to remove 
            for (int j = 0; j < J; j++)
            {
                if (s.getY()[job][j] == 1)
                {
                    m = j;
                    break;
                }
            }

            // Copy worker load and skiping removed job setting time
            for (int t = 0; t < T; t++)
                if (t < startTime || t > startTime + sk(job))
                {
                    _s.getW()[t] = s.getW()[t];

                }

            if(startTime <0 || m <0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR : in local search, could'nt remove job. Unconstructed solution ");
                Console.ForegroundColor = ConsoleColor.Gray;
                return null; 
            }
            else
            {
                // Capture used machine laod , skip processing period for the removed job 
                for (int t = 0; t < T; t++)
                    if (t < startTime || t > startTime + sk(job) + pk(k))
                    {
                        _s.getV()[t][m] = s.getV()[t][m];
                    }

                // Copy other machines load 
                for (j = 0; j < J; j++)
                    for (int t = 0; t < T; t++)
                        if (j != m)
                        {
                            _s.getV()[t][j] = s.getV()[t][j];
                        }


                // Reset copy X and Y for other jobs 
                for (int k = 0; k < K; k++)
                    if (k != job)
                    {
                        for (int t = 0; t < T; t++)
                        {
                            _s.getX()[k][t] = s.getX()[k][t];
                        }
                        for (int j = 0; j < J; j++)
                        {
                            _s.getY()[k][j] = s.getY()[k][j];
                        }
                    }
                return _s;
            }

           
        }

        // Copy procedure for aleady finalized solution oldS
        public void copyTo(Solution newS, Solution oldS)
        {

          
            for (int k = 0; k < K; k++)
            {
                // Copying x
                for (int t = 0; t < T; t++)
                    newS.getX()[k][t] = oldS.getX()[k][t];

                // Copying y
                for (int j = 0; j < J; j++)
                    newS.getY()[k][j] = oldS.getY()[k][j];

                // U and R 
                newS.getU()[k] = oldS.getU()[k];
                newS.getR()[k] = oldS.getR()[k];
            }

            // Copying w and F
            for (int t = 0; t < T; t++)
            {
                newS.getW()[t] = oldS.getW()[t];
                newS.getF()[t] = oldS.getF()[t];
            }

            // Copying v
            for (int t = 0; t < T; t++)
                for(int j=0; j<J; j++)
                    newS.getV()[t][j] = oldS.getV()[t][j];

        }


        public int getNumOfIterations()
        {
            return numOfIterations; 
        }



   

    public const float  DEFAULT_FATIGUE_RATE = 0.0385f;
}
       
}
    