namespace HotelBooking.Services
{
    using HotelBooking.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class RoomStatusUpdater : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public RoomStatusUpdater(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
                    dbContext.Database.ExecuteSqlRaw("EXEC UpdateRoomStatus");
                }

                // Chờ 24 giờ trước khi thực hiện lần tiếp theo (có thể chỉnh lại thời gian)
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }

}
