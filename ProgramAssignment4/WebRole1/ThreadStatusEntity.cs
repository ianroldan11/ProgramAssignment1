using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    public class ThreadStatusEntity : TableEntity
    {
        public string Status { get; set; }

        public ThreadStatusEntity()
        {

        }
    }
}