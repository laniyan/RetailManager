using System.Net.Http;
using System.Threading.Tasks;
using RetailManagerWPFGUI.Models;

namespace RMDesktopUI.Library.Api
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string userName, string password);

        Task GetLoggedInUserInfo(string token);

         HttpClient ApiClient { get; }

        void LogOffUser();
    }
}