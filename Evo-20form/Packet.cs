using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20form
{
    class Packet
    {
        const int PARAM_COUNT = 3;

        int[] w = new int[PARAM_COUNT];
        int[] a = new int[PARAM_COUNT];
        int[] u = new int[PARAM_COUNT];
        public int id
        {
            get;
            private set;
        }

        public Packet(int[] w, int[] a, int[] u, int id)
        {
            this.w = w;
            this.a = a;
            this.u = u;
            this.id = id;
        }
        public override string ToString()
        {
            string buffer = "id" + id.ToString() + " " + w[0].ToString() + " " + w[1].ToString() + " " + w[1].ToString() + "     ";
            buffer += a[0].ToString() + " " + a[1].ToString() + " " + a[1].ToString() + "     ";
            buffer += u[0].ToString() + " " + u[1].ToString() + " " + u[1].ToString() + "     ";
            return buffer;
        }
    }
}
