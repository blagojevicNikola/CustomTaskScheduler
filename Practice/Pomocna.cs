using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Practice
{
    class Pomocna
    {

        private String tekst;
        private bool p;
        private Mutex m = new Mutex();

        public Pomocna(String tekst, bool p)
        {
            this.tekst = tekst;
            this.p = p;
        }

        public String getTekst()
        {
            return tekst;
        }

        public void setTekst(String t)
        {
            tekst = t;
        }


        public void zakljucaj()
        {
            m.WaitOne();
        }

        public void otkljucaj()
        {
            m.ReleaseMutex();
        }
    }
}
