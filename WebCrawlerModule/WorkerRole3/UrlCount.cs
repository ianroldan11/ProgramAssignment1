﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole3
{
    class UrlCount : TableEntity
    {
        public int Urls { get; set; }

        public UrlCount()
        {

        }
    }
}
