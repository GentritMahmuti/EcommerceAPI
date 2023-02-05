using Core.IServices;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.UnitOfWork.IUnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> UpdateSentInfoAsync(int notificationId, CancellationToken cancellationToken)
        {
            var notification = await _unitOfWork.Repository<NotificationRequest>().GetByCondition(x => x.Id == notificationId).FirstOrDefaultAsync(cancellationToken);
            if (notification == null)
            {
                return false;
            }

            notification.Seen = true;

            _unitOfWork.Repository<NotificationRequest>().Update(notification);
            _unitOfWork.Complete();
            return true;
        }
    }
    
    
}
