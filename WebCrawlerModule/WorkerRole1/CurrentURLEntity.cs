using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole1
{
    class CurrentURLEntity : TableEntity
    {
        public string URL { get; set; }

        public CurrentURLEntity()
        {

        }
    }
}
