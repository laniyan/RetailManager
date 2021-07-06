using System.Collections.Generic;
using RMDataManager.Library.Models;

namespace RMDataManager.Library.DataAccess
{
    public interface IUserData
    {
        List<UserModel> GetUserById(string Id);
    }
}