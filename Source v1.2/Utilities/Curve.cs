using System.Collections;


namespace JOBSEC.Utilities
{
    class Curve
    {
        private ArrayList y = new ArrayList();
        private ArrayList x = new ArrayList();

        public void Add(float y, float x)
        {
            (this.y).Add(y);
            (this.x).Add(x);
        }


        public int lenght()
        {
            return y.Count;
        }


        public float getY(int index)
        {
            return (float)y[index]; 
        }


        public float getX(int index)
        {
            return (float)x[index];
        }
    }
}
