using System;

namespace RMDesktopUI.Library.Models
{
    public interface ILoggedInUserModel
    {
        string Id { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string EmailAddress { get; set; }
        DateTime CreatedDate { get; set; }
        string Token { get; set; }
    }
}