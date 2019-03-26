using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole2
{
    /// <summary>
    /// class for table object: Machine Counters i.e. CPU usage, Available Mem, reset flags
    /// </summary>
    class MachineCounterEntity : TableEntity
    {
        public double Value { get; set; }

        public MachineCounterEntity()
        {

        }


    }
}
