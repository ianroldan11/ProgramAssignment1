using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    public class MachineCountersEntity : TableEntity
    {
        public double Value { get; set; }

        public MachineCountersEntity()
        {

        }
    }
}