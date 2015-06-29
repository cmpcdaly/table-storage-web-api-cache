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
        public void ReturnIfNotModified(IEnumerable<TableEntity> entities, out EntitiesETag cacheKey)
        {
            // Get the If-None-Match header from the request
            var ifNoneMatch = this.Request.Headers.IfNoneMatch.FirstOrDefault();

            // Create the new EntitiesCacheKey
            cacheKey = new EntitiesETag(entities);

            // If the user has sent this header back
            if (ifNoneMatch != null)
            {
                if (cacheKey.Tag.Equals(ifNoneMatch.Tag))
                {
                    // If the lastModified date is not greater than the if-modified-since, return 304
                    throw new HttpResponseException(HttpStatusCode.NotModified);
                }
            }
        }

        public IHttpActionResult CachedOk<T>(T content, EntitiesETag cacheKey)
        {
            var response = this.Request.CreateResponse(HttpStatusCode.OK, content);
            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                NoCache = true,
                Private = true
            };

            response.Headers.ETag = new EntityTagHeaderValue(cacheKey.Tag, true);

            return this.ResponseMessage(response);
        }
    }
}