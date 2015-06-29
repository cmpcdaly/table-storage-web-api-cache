using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CacheAPI.Models
{
    public class EntitiesETag
    {
        private string _tag;

        public string Tag
        {
            get
            {
                return _tag;
            }
        }

        public EntitiesETag(IEnumerable<TableEntity> entities)
        {
            var sb = new StringBuilder();

            foreach (var entity in entities)
            {
                sb.Append(entity.ETag);
            }

            var allETags = sb.ToString();

            using (var sha1 = new SHA1Managed())
            {
                var eTagBytes = Encoding.UTF8.GetBytes(allETags);
                var hash = sha1.ComputeHash(eTagBytes);
                sb.Clear();
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }
                _tag = string.Concat("\"", sb.ToString(), "\"");
            }
        }
    }
}