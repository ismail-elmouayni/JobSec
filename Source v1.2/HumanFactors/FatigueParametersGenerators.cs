using System;

namespace JOBSEC
{
    class FatigueParametersGenerators
    {
        public const int    IFATIGUE_RATE = 0;           // Index of fatigue rate in the output file 
        public const int    IRECOVERY_RATE = 1;          // Index of recovery rate in the output file

        public const float DELTA_F = 0.01f;              // 1-DELTA_F is of fatigue index representing 
                                                         // ...fatigued operator. Close to 1........
        public const float DELTA_R = 0.01f;              // The fatigue index value representing total
                                                         // ...recovery, different thant 0 (and very small)
        public const float ENDURANCE_LIMIT = 360;        // Time that the worker can perform without...
                                                          // ...leading to total exhaution

        String version = "1";
        String methodShortName = "ILO";
        String methodFullName = "(International Office of Labour)";

        // Conversion matrix from point into allowance
        // 
        protected static float[,] conMatrix ={
                                                {0.10f,0.10f,0.10f,0.10f,0.10f,0.10f,0.10f,0.11f,0.11f,0.11f},
                                                {0.11f,0.11f,0.11f,0.11f,0.11f,0.12f,0.12f,0.12f,0.12f,0.12f},
                                                {0.13f,0.13f,0.13f,0.13f,0.14f,0.14f,0.14f,0.14f,0.15f,0.15f},
                                                {0.15f,0.16f,0.16f,0.16f,0.17f,0.17f,0.17f,0.18f,0.18f,0.18f},
                                                {0.19f,0.19f,0.20f,0.20f,0.21f,0.21f,0.22f,0.22f,0.23f,0.23f},
                                                {0.24f,0.24f,0.25f,0.26f,0.26f,0.27f,0.27f,0.28f,0.28f,0.29f},
                                                {0.30f,0.30f,0.31f,0.32f,0.32f,0.33f,0.34f,0.34f,0.35f,0.36f},
                                                {0.37f,0.37f,0.38f,0.39f,0.40f,0.40f,0.41f,0.42f,0.43f,0.44f},
                                                {0.45f,0.46f,0.47f,0.48f,0.48f,0.49f,0.50f,0.51f,0.52f,0.52f},
                                                {0.54f,0.55f,0.56f,0.57f,0.58f,0.59f,0.60f,0.61f,0.62f,0.63f},
                                                {0.64f,0.65f,0.66f,0.68f,0.69f,0.70f,0.71f,0.72f,0.73f,0.74f},
                                                {0.75f,0.77f,0.78f,0.79f,0.80f,0.82f,0.83f,0.84f,0.85f,0.87f},
                                                {0.88f,0.89f,0.91f,0.92f,0.93f,0.95f,0.96f,0.97f,0.99f,0.100f},
                                                {1.01f,1.03f,1.05f,1.06f,1.07f,1.09f,1.10f,1.12f,1.13f,1.15f},
                                                };



        public float[] genFatigueRate(float[] stressFactorsLevels)
        {

            float   numOfStressPoints   = 0;
            float[] output = new float[2]; 


            // Process the sum of stress levels 
            for (int i = 0; i < stressFactorsLevels.Length; i++)
            {
                numOfStressPoints = numOfStressPoints + stressFactorsLevels[i];
            }
        
            // Convert into allowance time 
            float allowancePercent = conMatrix[(int)(numOfStressPoints / (10.0f)),(int)(numOfStressPoints - ((int)(numOfStressPoints / (10.0f))) * 10)];
           
            // Process fatigue rate
            output[IFATIGUE_RATE]   = (float)(Math.Log(1 / DELTA_F) * (1 + allowancePercent) / ENDURANCE_LIMIT);
            output[IRECOVERY_RATE]  = (float)(Math.Log((1 + DELTA_F) / DELTA_R) * (1 + 1 / allowancePercent) / ENDURANCE_LIMIT);

            return output; 
        }




    }
}
