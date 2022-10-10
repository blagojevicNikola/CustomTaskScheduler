using DokanNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerFileSystem
{
    public class MyFile
    {
        public string FileName { get; set; }
        public FileInformation FileInfo { get; set;}

        private byte[] data;

        public MyFile(string name)
        {
            FileName = name;
        }

        public MyFile(string name, FileInformation info)
        {
            FileName = name;
            FileInfo = info;
        }

        public void setData(byte[] arr)
        {
            data = arr;
        }

        public byte[] getData()
        {
            return data;
        }
    }
}
