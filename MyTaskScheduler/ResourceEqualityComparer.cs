using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTaskScheduler
{
    class ResourceEqualityComparer : IEqualityComparer<MyResource>
    {
        public bool Equals(MyResource x, MyResource y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
                return false;
            else if (x.getPath() == y.getPath())
                return true;
            else
                return false;
        }

        public int GetHashCode(MyResource obj)
        {
            return obj.GetHashCode();
        }
    }
}
