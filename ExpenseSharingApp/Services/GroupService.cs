using ExpenseSharingApp.Interfaces;
using ExpenseSharingApp.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ExpenseSharingApp.Services
{
    public class GroupService : IGroupService
    {
        private readonly ExpenseSharingContext _context;

        public GroupService(ExpenseSharingContext context)
        {
            _context = context;
        }

        public Group CreateGroup(GroupCreationModel groupDetails)
        {
            try
            {
                string uniqueGroupId = Guid.NewGuid().ToString();

                List<UserGroup> members = new List<UserGroup>();
                if (groupDetails.MemberEmails.Count > 10)
                {
                    throw new ArgumentException($"A group cannot have more than 10 members.");
                }

                foreach (var email in groupDetails.MemberEmails)
                {
                    var user = _context.Users.SingleOrDefault(u => u.Email == email);

                    if (user != null)
                    {
                        members.Add(new UserGroup
                        {
                            UserId = user.Id,
                            GroupId = uniqueGroupId,
                            AmountToPay = 0,
                        });
                    }
                    else
                    {
                        throw new ArgumentException($"User with email '{email}' is not a valid user.");
                    }
                }

                Group group = new Group
                {
                    GroupId = uniqueGroupId,
                    Name = groupDetails.Name,
                    Description = groupDetails.Description,
                    CreatedDate = groupDetails.CreatedDate,
                    Members = members
                };

                _context.Groups.Add(group);
                _context.SaveChanges();

                return group;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while creating the group.", ex);
            }
        }

        public IEnumerable<Group> GetAllGroups()
        {
            try
            {
                return _context.Groups.Include(g => g.Members).Include(g => g.Expenses).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while retrieving all groups.", ex);
            }
        }

        public Group GetGroupById(string id)
        {
            try
            {
                return _context.Groups.Include(g => g.Members).Include(g => g.Expenses).SingleOrDefault(g => g.GroupId == id);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"An error occurred while retrieving the group with ID {id}.", ex);
            }
        }

        public IEnumerable<Group> GetGroupsByUserId(string userId)
        {
            try
            {
                var userGroups = _context.UserGroups
                    .Where(ug => ug.UserId == userId)
                    .Include(ug => ug.Group)
                        .ThenInclude(g => g.Members) // Include Members of the Group
                    .Include(ug => ug.Group)
                        .ThenInclude(g => g.Expenses) // Include Expenses of the Group
                    .Select(ug => ug.Group)
                    .ToList();

                return userGroups;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"An error occurred while retrieving groups for user with ID {userId}.", ex);
            }
        }

        public void DeleteGroup(string id)
        {
            try
            {
                var group = _context.Groups.Include(g => g.Members).ThenInclude(ug => ug.User).FirstOrDefault(g => g.GroupId == id);
                if (group == null)
                {
                    throw new Exception("Group not found");
                }

                // Recalculate balances for each user
                foreach (var userGroup in group.Members)
                {
                    var user = userGroup.User;

                    // Calculate user's balance based on unsettled expenses where the user paid
                    var unsettledExpenses = _context.Expenses.Where(e => e.PaidById == user.Id && !e.IsSettled).ToList();
                    foreach (var expense in unsettledExpenses)
                    {
                        user.Balance += expense.Amount;
                    }

                    // Calculate user's balance based on settled expenses where the user paid
                    var settledExpenses = _context.Expenses.Where(e => e.PaidById == user.Id && e.IsSettled).ToList();
                    foreach (var expense in settledExpenses)
                    {
                        var expenseGroup = _context.Groups
                            .Include(g => g.Members)
                            .ThenInclude(m => m.User)
                            .FirstOrDefault(g => g.GroupId == expense.GroupId);

                        // Calculate the portion of settled expense to return to the user
                        var numberOfGroupMembers = expenseGroup.Members.Count;
                        user.Balance += expense.Amount / numberOfGroupMembers;
                    }

                    // Update the user's balance in the context
                    _context.Users.Update(user);
                }

                // Save changes to the context after recalculating balances
                _context.SaveChanges();

                // Remove all expenses related to the group
                var expensesToDelete = _context.Expenses.Where(e => e.GroupId == id).ToList();
                _context.Expenses.RemoveRange(expensesToDelete);

                // Remove all members from the group in UserGroup table
                _context.UserGroups.RemoveRange(group.Members);

                // Remove the group
                _context.Groups.Remove(group);

                // Save changes to the context
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"An error occurred while deleting the group with ID {id}.", ex);
            }
        }
    }
}
