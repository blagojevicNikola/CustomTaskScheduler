﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MySched
{
    public class MyResource
    {
        private static int staticID = 0;

        private int id;
        private String putanja;
        private readonly object monitorObj = new object();
        private readonly object maxPrioritetObj = new object();
        private volatile bool zauzeto = false;
        private int maxPrioritet = 0;
        private List<MyTask> vlasnici = new List<MyTask>();

        public MyResource(String putanja)
        {
            id = staticID++;
            this.putanja = putanja;
        }

        public void setPutanja(String putanja)
        {
            this.putanja = putanja;
        }

        public String getPutanja()
        {
            return putanja;
        }

        public void lockResource()
        {
            //Console.WriteLine("Tred[{0}] zeli da zakljuca resurs!", Thread.CurrentThread.ManagedThreadId);
            zauzeto = true;
            Monitor.Enter(monitorObj);
            //Console.WriteLine("Tred[{0}] je zakljucao resurs!", Thread.CurrentThread.ManagedThreadId);
        }

        public void unlcokResource()
        {
            //Console.WriteLine("Tred[{0}] otkljucava resurs!", Thread.CurrentThread.ManagedThreadId);
            zauzeto = false;
            Monitor.Exit(monitorObj);
        }

        public String getContent()
        {
            String text = File.ReadAllText(putanja);
            return text;
        }

        public int getMaxPrioritet()
        {
            return maxPrioritet;
        }

        public void setMaxPrioritet(int maxPrioritet, MyTask task)
        {
            
            if (this.maxPrioritet < maxPrioritet)
            {
                this.maxPrioritet = maxPrioritet;
                if(zauzeto)
                {
                    lock(maxPrioritetObj)
                    {
                        task.setPrioritet(maxPrioritet);
                    }
                }
            }
            
        }

        public void addVlasnik(MyTask t)
        {
            vlasnici.Add(t);
        }

        public void removeVlasnik(MyTask t)
        {
            vlasnici.Remove(t);
        }

        public int getId()
        {
            return id;
        }

    }
}
