using ePasServices.Models;
using ePasServices.ViewModels;

namespace ePasServices.Services.Interfaces
{
    public interface INotificationService
    {
        Task<(List<NotificationListItemViewModel> Data, int Total)> GetNotificationListAsync(int page, int limit, string userId);

        Task<bool> UpdateNotificationStatusAsync(string id, string status, string userId);
    }
}
