﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CacheAPI.Models
{
    public class PersonModel
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public PersonModel(Person person)
        {
            this.Id = person.RowKey;
            this.FullName = person.FullName;
        }
    }
}