using System;
using System.Threading.Tasks;
using BandApp.Services;

namespace BandApp.ViewModels
{
    public class HeartRateViewModel : NotifyPropertyChangedBase
    {
        private readonly IHeartRateService _heartRateService;
        private readonly TaskFactory _uiFactory;

        private int _heartRate;
        private string _state;

        public HeartRateViewModel( IHeartRateService heartRateService )
        {
            _uiFactory = new TaskFactory( TaskScheduler.FromCurrentSynchronizationContext() );

            _heartRateService = heartRateService;
            RegisterEvents();
        }

        /// <summary>
        /// Registers Events from service and handles data
        /// </summary>
        public void RegisterEvents()
        {
            this.State = "Connecting..";

            _heartRateService.OnHeartRate += OnHeartRate;
            _heartRateService.ConnectAsync().ContinueWith( connectTaskInfo =>
            {
                _uiFactory.StartNew( () =>
                {
                    this.State = "Connected.";
                } );
            } );
        }

        /// <summary>
        /// Current Monitoring State
        /// </summary>
        public string State
        {
            get { return _state; }
            set
            {
                _state = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Receives new heart rate and sets <see cref="HeartRate"/>
        /// </summary>
        private void OnHeartRate( object sender, EventArgs e )
        {
            MsBandHeartRateEventArgs args = e as MsBandHeartRateEventArgs;
            if( args != null )
            {
                _uiFactory.StartNew( () =>
                {
                    this.HeartRate = args.HeartRate;
                } );
            }
        }

        /// <summary>
        /// Observerable HeartRate
        /// </summary>
        public int HeartRate
        {
            get { return _heartRate; }
            set
            {
                _heartRate = value;
                NotifyPropertyChanged();
            }
        }
    }
}
