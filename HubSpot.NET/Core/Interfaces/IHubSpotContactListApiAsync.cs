using System.Collections.Generic;
using System.Threading.Tasks;
using HubSpot.NET.Api.ContactList;
using HubSpot.NET.Api.ContactList.Dto;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotContactListApiAsync
    {
        Task<ListContactListModel> GetContactListsAsync(ListOptions opts = null);

        Task<ListContactListModel> GetStaticContactListsAsync(ListOptions opts = null);

        Task<ContactListModel> GetContactListByIdAsync(long contactListId);

        Task<ContactListUpdateResponseModel> AddContactsToListAsync(long listId, IEnumerable<long> contactIds);

        Task<ContactListUpdateResponseModel> RemoveContactsFromListAsync(long listId, IEnumerable<long> contactIds);

        Task DeleteContactListAsync(long listId);

        Task<ContactListModel> CreateStaticContactListAsync(string contactListName);
    }
}