using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.OS;
using GeofencePlayground.Geofencing;
using Splat;

namespace GeofencePlayground.Droid.Geofencing
{
    public class GeofencingManager : 
        IGeofencingManager, 
        IEnableLogger, 
        GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener
    {
        public event Action<bool> StatusChanged;
        public event Action<ConnectionResult> ConnectionFailed;

        public bool Started
        {
            get { return _started; }
            private set
            {
                _started = value;
                StatusChanged?.Invoke(_started);
            }
        }

        public IntPtr Handle => _context.Handle;

        private readonly Context _context;
        private readonly IList<IGeofence> _geofences;
        private readonly GoogleApiClient _googleApiClient;

        private PendingIntent _geofencePendingIntent;
        private bool _started;

        private PendingIntent GeofencePendingIntent 
        {
            get
            {
                if (_geofencePendingIntent != null)
                    return _geofencePendingIntent;

                var intent = new Intent(_context, typeof(GeofencingManager));
                _geofencePendingIntent = PendingIntent.GetService(_context, 0, intent, PendingIntentFlags.UpdateCurrent);
                return _geofencePendingIntent;
            }
        }

        public GeofencingManager(Context context)
        {
            _context = context;
            _geofences = new List<IGeofence>();
            _googleApiClient = new GoogleApiClient.Builder(_context)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .AddApi(LocationServices.API)
                .Build();
        }

        public void AddGeofenceData(params GeofenceData[] args)
        {
            foreach (var data in args)
            {
                if (_geofences.Any(x => x.RequestId == data.Id))
                    continue;

                var geofence = new GeofenceBuilder()
                    .SetRequestId(data.Id)
                    .SetCircularRegion(data.Latitude, data.Longitude, data.Radius)
                    .SetExpirationDuration(data.Expiration)
                    .SetTransitionTypes(Geofence.GeofenceTransitionEnter | Geofence.GeofenceTransitionExit)
                    .Build();

                _geofences.Add(geofence);
            }
        }

        public void Connect() 
            => _googleApiClient?.Connect();

        public void Disconnect() => 
            _googleApiClient?.Disconnect();

        public async Task<bool> StartGeofencing()
        {
            if(_geofences.Count == 0)
                throw new Exception("You need to add geofences before starting");

            if (!_googleApiClient.IsConnected)
            {
                this.Log().Error("Cannot start geofencing because google api client is not connected");
                return false;
            }

            var request = new GeofencingRequest.Builder()
                .SetInitialTrigger(GeofencingRequest.InitialTriggerEnter)
                .AddGeofences(_geofences)
                .Build();

            try
            {
                var status = await LocationServices.GeofencingApi
                    .AddGeofencesAsync(_googleApiClient, request, GeofencePendingIntent);

                Started = status.IsSuccess;

                if (status.IsSuccess)
                    return true;

                this.Log().Error(GeofenceErrorMessages.GetErrorString(_context, status.StatusCode));
                return false;
            }
            catch (Java.Lang.SecurityException securityException)
            {
                LogSecurityException(securityException);
            }
            catch (Java.Lang.Exception ex)
            {
                this.Log().Error(ex.Message);
            }

            return false;
        }

        public async Task<bool> StopGeofencing()
        {
            if (!_googleApiClient.IsConnected)
            {
                this.Log().Error("Cannot stop geofencing because google api client is not connected");
                return false;
            }

            try
            {
                var status = await LocationServices.GeofencingApi
                    .RemoveGeofencesAsync(_googleApiClient, GeofencePendingIntent);

                Started = !status.IsSuccess;

                if (status.IsSuccess)
                    return true;

                this.Log().Error(GeofenceErrorMessages.GetErrorString(_context, status.StatusCode));
                return false;
            }
            catch (Java.Lang.SecurityException securityException)
            {
                LogSecurityException(securityException);
            }
            catch (Java.Lang.Exception ex)
            {
                this.Log().Error(ex.Message);
            }

            return false;
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            this.Log().Warn($"Connection failed with code {result.ErrorCode}: {result.ErrorMessage}");
            ConnectionFailed?.Invoke(result);

            //if (result.HasResolution)
            //{
            //    try
            //    {
            //        result.StartResolutionForResult(this, 9000);
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}
        }

        public void OnConnected(Bundle connectionHint) 
            => this.Log().Info("Connected to GoogleApiClient");

        public void OnConnectionSuspended(int cause) 
            => this.Log().Info($"Connection suspended. Cause: {cause}");

        public void Dispose()
        {
            Disconnect();
            Started = false;
            _googleApiClient.Dispose();
            _geofences.Clear();
            _geofencePendingIntent = null;
        }

        private void LogSecurityException(Java.Lang.SecurityException securityException)
        {
            var message = "Invalid location permission. " +
                          "Please, add 'ACCESS_FINE_LOCATION' to your manifest. \n" +
                          securityException.Message;

            this.Log().Error(message);
        }
    }
}