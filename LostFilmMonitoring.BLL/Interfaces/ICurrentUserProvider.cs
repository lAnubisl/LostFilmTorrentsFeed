using System;

namespace LostFilmMonitoring.BLL.Interfaces
{
    public interface ICurrentUserProvider
    {
        Guid GetCurrentUserId();

        void SetCurrentUserId(Guid userId);
    }
}