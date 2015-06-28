using CacheAPI.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace CacheAPI.Controllers
{
    public class PersonController : CacheApiController
    {
        CloudStorageAccount storageAccount;
        CloudTable table;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference("people");
        }

        static PersonController()
        {
            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("people");
            table.CreateIfNotExists();

            Person p1 = new Person("1", "Margaret");
            Person p2 = new Person("2", "David");

            table.Execute(TableOperation.InsertOrReplace(p1));
            table.Execute(TableOperation.InsertOrReplace(p2));
        }

        [HttpGet]
        public IHttpActionResult GetPeople()
        {
            // Retrieve entities
            var people = table.ExecuteQuery(new TableQuery<Person>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "people")));

            // Handle caching
            DateTimeOffset lastModified;
            this.ReturnIfNotModified(people, out lastModified);

            // Convert entities to models and return response with cache headers
            return CachedOk(people.Select(p => new PersonModel(p)), lastModified);
        }
        
        [HttpPut]
        public IHttpActionResult UpdatePerson(PersonModel model)
        {
            // Retrieve entity
            Person person = (Person)table.Execute(TableOperation.Retrieve<Person>("people", model.Id)).Result;

            // Upate name
            person.FullName = model.FullName;

            // Save Changes
            table.Execute(TableOperation.Replace(person));

            return Ok();
        }
    }
}
