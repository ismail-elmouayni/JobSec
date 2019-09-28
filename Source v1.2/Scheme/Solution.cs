
/*
 * Solution.cs
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


namespace JOBSEC
{
    class Solution
    {
        // Decision variable 
        // One dimension 
        private int[]   u,      // Number of period of tardness regarding the order k : u(k) 
                        w,      // Equals to 1 if the worker is busy at period t, O otherwise : w(t)  
                        e;      // Earliness

        private float[] F,      // Fatigue index at the begining of the period t : F(t) 
                        r;      // The number of rejected parts regarding the order k : r(k)        
       // Two dimensions 
        private int[][] x,      // Equals to one if the order k is processed at period t : x(k,t)
                        y,      // If the machine j is used for the order k : y(k,j)  
                        v;      // Machine state at t : v(t,j)=1 means that the machine j is busy at t


        public Solution (Model model)
        {
            // Initialize decision variable regarding.....
            // ...the order k processing periods .........
            x = new int[model.getNumOfOrders()][];
            for (int k = 0; k < model.getNumOfOrders(); k++)
                x[k] = new int[model.getHorizon()];

            // Initialize decision variable regarding......
            // ...which machine to use for order processing 
            y = new int[model.getNumOfOrders()][];
            for (int k = 0; k < model.getNumOfOrders(); k++)
                y[k] = new int[model.getNumOfMachines()];

            // Initialize variable regarding for fatigue...
            // ...estimation
            F = new float[model.getHorizon()];

            // Initialize variable for rejected parts.....
            r = new float[model.getNumOfOrders()];

            // Initialize variable for worker state........
            w = new int[model.getHorizon()];

            // Initialize variable for number of periods...
            // ...of tardiness 
            u = new int[model.getNumOfOrders()];

            // Initialize variable for number of periods...
            // ...of earliness 
            e = new int[model.getNumOfOrders()];

            // Initialize machine load 
            v = new int[model.getHorizon()][];
            for (int t = 0; t < model.getHorizon(); t++)
                v[t] = new int[model.getNumOfMachines()];

            for (int compt = 0; compt <model.getHorizon(); compt++)
                for (int compt2 = 0; compt2 < model.getNumOfMachines(); compt2++)
                    v[compt][compt2] = model.getInitialMachinesLoad()[compt][compt2]; 
        }


        public int[][] getX()
        {
            return x;
        }

        public int[][] getY()
        {
            return y; 
        }

        public int[] getW()
        {
            return w; 
        }

        public float[] getF()
        {
            return F; 
        }

        public float[] getR()
        {
            return r; 
        }

        public int[] getU()
        {
            return u; 
        }

        public int[][] getV()
        {
            return v;
        }

        public int[] getE()
        {
            return e; 
        }
    }
}
