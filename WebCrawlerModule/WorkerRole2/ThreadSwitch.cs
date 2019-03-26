using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole2
{
    /// <summary>
    /// class for table object: switch to tell if thread should start or start crawling
    /// </summary>
    class ThreadSwitch : TableEntity
    {
        public bool IsSwitched { get; set; }

        public ThreadSwitch()
        {

        }
    }
}
