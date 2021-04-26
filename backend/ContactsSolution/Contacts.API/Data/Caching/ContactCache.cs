using Contacts.API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Contacts.API.Data.Caching
{
    public class ContactCache : IContactCache
    {
        #region Properties
        private MemoryCache _cache { get; set; }
        #endregion

        #region Ctor
        public ContactCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions { SizeLimit = 100 });
        }
        #endregion

        #region Caching

        private string GetCacheKey(int contactID) => $"Contact-{contactID}";

        public ContactModel Get(int contactID)
        {
            ContactModel contactModel;
            _cache.TryGetValue(GetCacheKey(contactID), out contactModel);
            return contactModel;
        }

        public void Set(ContactModel contactModel)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(100);
            _cache.Set(GetCacheKey(contactModel.ContactID), contactModel, cacheEntryOptions);
        }

        public void Remove(int contactID)
        {
            _cache.Remove(GetCacheKey(contactID));
        }

        #endregion
    }
}
