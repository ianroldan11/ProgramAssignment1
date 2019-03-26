using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole2
{
    /// <summary>
    /// table entity for the number of urls stored in a storage
    /// </summary>
    class UrlCount : TableEntity
    {
        public int Urls { get; set; }

        public UrlCount()
        {

        }
    }
}
