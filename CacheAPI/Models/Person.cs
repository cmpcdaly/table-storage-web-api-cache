using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CacheAPI.Models
{
    public class Person : TableEntity
    {
        public string FullName { get; set; }

        public Person() { }

        public Person(string id, string name)
        {
            this.PartitionKey = "people";
            this.RowKey = id;
            this.FullName = name;
        }
    }
}