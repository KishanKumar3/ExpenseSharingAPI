using ExpenseSharingApp.Models;

namespace ExpenseSharingApp.Interfaces
{
    public interface IGroupService
    {
        Group CreateGroup(GroupCreationModel groupDetails);
        IEnumerable<Group> GetAllGroups();
        Group GetGroupById(string id);

        void DeleteGroup(string id);
        IEnumerable<Group> GetGroupsByUserId(string userId);

    }

}
