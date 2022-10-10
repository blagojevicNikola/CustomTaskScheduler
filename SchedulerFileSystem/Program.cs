using DokanNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerFileSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            new MyFileSystem().Mount("L:\\", DokanOptions.DebugMode | DokanOptions.StderrOutput);
        }
    }
}
