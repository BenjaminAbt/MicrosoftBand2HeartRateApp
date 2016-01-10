using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Band;
using Microsoft.Band.Sensors;

namespace BandApp.Services
{
    /// <summary>
    /// Provides heart rate of first connected Microsoft Band
    /// </summary>
    public class MsBandHeartRateService : IDisposable, IHeartRateService
    {
        private IBandClient _bandClient;
        private IBandInfo _bandInfo;

        public event EventHandler OnHeartRate;

        public bool IsConnected { get; private set; }

        // Connects to the band
        public async Task<bool> ConnectAsync()
        {
            if( IsConnected )
            {
                return false;
            }

            // Find all Bands
            IBandInfo[ ] allBands = await BandClientManager.Instance.GetBandsAsync();
            if( !allBands.Any() )
            {
                throw new NoBandFoundException();
            }

            // Use first Band
            _bandInfo = allBands.First();

            // Connect to first Band
            _bandClient = await BandClientManager.Instance.ConnectAsync( _bandInfo );

            // Get User Consent of current band
            UserConsent uc = _bandClient.SensorManager.HeartRate.GetCurrentUserConsent();
            bool isConsented = false;
            if( uc == UserConsent.NotSpecified )
            {
                isConsented = await _bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
            }

            // Check if really consented and check if permissions are granted
            if( isConsented || uc == UserConsent.Granted )
            {
                // provide new rate via event
                _bandClient.SensorManager.HeartRate.ReadingChanged += FireHeartRate;

                // Start reading
                return ( await _bandClient.SensorManager.HeartRate.StartReadingsAsync() );
            }

            return ( IsConnected = false );
        }

        protected virtual void FireHeartRate( object sender, BandSensorReadingEventArgs<IBandHeartRateReading> args )
        {
            OnHeartRate?.Invoke( this, new MsBandHeartRateEventArgs( args.SensorReading.HeartRate ) );
        }

        public void Dispose()
        {
            if( _bandClient != null )
            {

                _bandClient.Dispose();
                _bandClient = null;
            }

            _bandInfo = null;
        }
    }
}
