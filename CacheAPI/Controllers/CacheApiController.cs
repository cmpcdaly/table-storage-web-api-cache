using CacheAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage.Table;

namespace CacheAPI.Controllers
{
    public class CacheApiController : ApiController 
    {
        public void ReturnIfNotModified(IEnumerable<TableEntity> entities, out DateTimeOffset lastModified)
        {
            // Get the If-None-Match header from the request
            var ifNoneMatch = this.Request.Headers.IfNoneMatch.FirstOrDefault();

            // Get the last modified date for the entities
            lastModified = entities.Select(entity => entity.Timestamp).Max();

            // If the user has sent this header back
            if (ifNoneMatch != null)
            {
                var newETag = GenerateTag(lastModified);

                if (newETag.Equals(ifNoneMatch.Tag))
                {
                    // If the lastModified date is not greater than the if-modified-since, return 304
                    throw new HttpResponseException(HttpStatusCode.NotModified);
                }
            }
        }

        public IHttpActionResult CachedOk<T>(T content, DateTimeOffset lastModified)
        {
            var response = this.Request.CreateResponse(HttpStatusCode.OK, content);
            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                NoCache = true,
                Private = true
            };

            string tag = GenerateTag(lastModified);
            response.Headers.ETag = new EntityTagHeaderValue(tag, true);

            return this.ResponseMessage(response);
        }

        private string GenerateTag(DateTimeOffset dateTime)
        {
            // ToDo: What if an entity is deleted?
            return string.Concat("\"", dateTime.UtcTicks.ToString(), "\"");
        }
    }
}