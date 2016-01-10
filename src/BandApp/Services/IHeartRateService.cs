using System;
using System.Threading.Tasks;

namespace BandApp.Services
{
    public interface IHeartRateService
    {
        Task<bool> ConnectAsync();
        event EventHandler OnHeartRate;
    }
}