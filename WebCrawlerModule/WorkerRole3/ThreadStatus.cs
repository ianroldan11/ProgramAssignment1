﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole3
{
    class ThreadStatus : TableEntity
    {
        public string Status { get; set; }

        public ThreadStatus()
        {

        }
    }
}
