using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRole1
{
    public class CurrentURLEntity : TableEntity
    {
        public string URL { get; set; }

        public CurrentURLEntity()
        {

        }
    }
}
