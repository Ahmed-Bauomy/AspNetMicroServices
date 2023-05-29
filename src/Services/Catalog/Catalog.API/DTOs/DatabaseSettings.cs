using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.DTOs
{
    public class DatabaseSettings
    {
        public string ConnectionString {get; set;}
        public string DatabaseName     {get; set;}
        public string CollectionName { get; set; }
    }
}
