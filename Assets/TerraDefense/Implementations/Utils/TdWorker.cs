using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Assets.TerraDefense.Implementations.Utils
{
    public class TdWorker
    {
        public bool IsDone { get; set; }
        public ThreadStart Action;
    }
}
