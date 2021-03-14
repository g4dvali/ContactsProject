using Contacts.API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contacts.API.Data.Caching
{
    public interface IContactCache
    {
        ContactModel Get(int contactID);
        void Remove(int contactID);
        void Set(ContactModel contactModel);
    }
}
