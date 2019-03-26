using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole2
{
    /// <summary>
    /// class for table object: status of thread of a web crawler
    /// </summary>
    class ThreadStatus : TableEntity
    {
        public string Status { get; set; }

        public ThreadStatus()
        {

        }
    }
}
