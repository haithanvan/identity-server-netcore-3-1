using Microsoft.AspNetCore.Identity;
using Nmb.Shared.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerDemo.Domain.AccountAggregate
{
    public class Account : IdentityUser<Guid>, IAggregateRoot
    {
        //public string MerchantId { get; set; }
        public bool IsActive { get; set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string ProfileImageUrl { get; private set; }
        public string Address { get; private set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        public virtual IList<AccountRole> UserRoles { get; } = new List<AccountRole>();
        public virtual IList<IdentityUserLogin<Guid>> Logins { get; } = new List<IdentityUserLogin<Guid>>();

        public Account()
        {

        }

        public Account(string email, string firstName, string lastName, string profileImageUrl, string address, bool isActive) : this()
        {
            FirstName = firstName;
            LastName = lastName;
            ProfileImageUrl = profileImageUrl;
            IsActive = isActive;
            Email = email;
            UserName = email;
            Address = address;
        }

        public Account(string email, string firstName, string lastName, string profileImageUrl, string phoneNumber, string address, bool isActive) : this()
        {
            FirstName = firstName;
            LastName = lastName;
            ProfileImageUrl = profileImageUrl;
            IsActive = isActive;
            Email = email;
            UserName = email;
            PhoneNumber = phoneNumber;
            Address = address;
        }
        public void UpdateInfo(string firstName, string lastName, string profileImageUrl, string phoneNumber, string address)
        {
            FirstName = firstName;
            LastName = lastName;
            ProfileImageUrl = profileImageUrl;
            PhoneNumber = phoneNumber;
            Address = address;
        }

        public void ConfirmEmail()
        {
            EmailConfirmed = true;
        }

        public void Update(string firstName, string lastName, string profileImageUrl, string phoneNumber, string address)
        {
            FirstName = firstName;
            LastName = lastName;
            ProfileImageUrl = profileImageUrl;
            PhoneNumber = phoneNumber;
            Address = address;
        }

        public void UpdateStatus(bool active)
        {
            IsActive = active;
        }

        public static Account CreateNewAccount(string email, string firstName, string lastName, bool isActive, string profileImageUrl)
        {
            return new Account(email, firstName, lastName, profileImageUrl, "", isActive);
        }

    }
}
