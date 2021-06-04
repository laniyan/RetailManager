using System.Threading.Tasks;
using RetailManagerWPFGUI.Models;

namespace RetailManagerWPFGUI.Helpers
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string userName, string password);
    }
}