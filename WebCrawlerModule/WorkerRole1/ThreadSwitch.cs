﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole1
{
    class ThreadSwitch : TableEntity
    {
        public bool IsSwitched { get; set; }

        public ThreadSwitch()
        {

        }
    }
}
