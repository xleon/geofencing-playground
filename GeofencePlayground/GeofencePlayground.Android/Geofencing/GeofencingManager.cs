using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.OS;
using GeofencePlayground.Geofencing;
using Splat;

[assembly: UsesPermission(Name = "android.permission.ACCESS_FINE_LOCATION")]
[assembly: UsesPermission(Name = "android.permission.RECEIVE_BOOT_COMPLETED")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]

namespace GeofencePlayground.Droid.Geofencing
{
    public class GeofencingManager : 
        Java.Lang.Object,
        IGeofencingManager, 
        IEnableLogger, 
        GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener
    {
        public static IGeofencingManager Current => Instance.Value;

        private static readonly Lazy<IGeofencingManager> Instance 
            = new Lazy<IGeofencingManager>(() => new GeofencingManager(), 
                LazyThreadSafetyMode.PublicationOnly);

        private readonly Context _context;
        private readonly IList<IGeofence> _geofences;
        private GoogleApiClient _client;
        private PendingIntent _geofencePendingIntent;
        private TaskCompletionSource<bool> _connectionTaskCompletionSource;
        private TaskCompletionSource<bool> _disconnectionTaskCompletionSource;

        private GeofencingManager(Context context = null)
        {
            _context = context ?? Application.Context;
            _geofences = new List<IGeofence>();

            this.Log().Info($"{nameof(GeofencingManager)} created with id {Guid.NewGuid()}");
        }

        private PendingIntent GeofencePendingIntent 
        {
            get
            {
                if (_geofencePendingIntent != null)
                    return _geofencePendingIntent;

                var intent = new Intent(_context, typeof(GeofenceTransitionsIntentService));
                _geofencePendingIntent = PendingIntent.GetService(_context, 0, intent, PendingIntentFlags.UpdateCurrent);
                return _geofencePendingIntent;
            }
        }

        private GoogleApiClient Client
            => _client ?? (_client = new GoogleApiClient.Builder(_context)
                   .AddApi(LocationServices.API)
                   .AddConnectionCallbacks(this)
                   .AddOnConnectionFailedListener(this)
                   .Build());

        public IGeofencingManager AddGeofenceData(params GeofenceRegion[] args)
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

            return this;
        }

        public async Task<bool> StartGeofencing()
        {
            this.Log().Info($"{nameof(StartGeofencing)} called");

            if (_geofences.Count == 0)
            {
                this.Log().Error("No geofences found. Please add geofences before starting");
                return false;
            }

            var availability = GoogleApiAvailability.Instance
                .IsGooglePlayServicesAvailable(Application.Context);

            if (availability != ConnectionResult.Success)
            {
                this.Log().Error("Google Api is not available");
                return false;
            }

            this.Log().Info("Google Api is available");

            var connected = await Connect();

            if (!connected)
            {
                this.Log().Error("Could not connect to Google Api");
                return false;
            }

            try
            {
                var request = new GeofencingRequest.Builder()
                    .SetInitialTrigger(GeofencingRequest.InitialTriggerEnter)
                    .AddGeofences(_geofences)
                    .Build();

                var statuses = await LocationServices.GeofencingApi
                    .AddGeofencesAsync(Client, request, GeofencePendingIntent);

                if (statuses.IsSuccess)
                {
                    var ids = string.Join(", ", _geofences.Select(x => x.RequestId));
                    this.Log().Info($"Geofences added correctly!: {ids}");
                    return true;
                }

                this.Log().Error(GeofenceErrorMessages.GetErrorString(_context, statuses.StatusCode));
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

        private Task<bool> Connect()
        {
            this.Log().Info("Trying to connect");

            if (Client.IsConnected)
            {
                this.Log().Warn("Client is already connected");
                return Task.FromResult(true);
            }

            _connectionTaskCompletionSource = new TaskCompletionSource<bool>();
            this.Log().Info("Client is connecting");

            if (Client.IsConnecting)
                return _connectionTaskCompletionSource.Task;

            Client.Connect();
            return _connectionTaskCompletionSource.Task;
        }

        public Task<bool> Disconnect()
        {
            _disconnectionTaskCompletionSource = new TaskCompletionSource<bool>();
            Client.Disconnect();
            return _disconnectionTaskCompletionSource.Task;
        }

        public async Task<bool> StopGeofencing()
        {
            if (!Client.IsConnected)
            {
                this.Log().Error("Cannot stop geofencing because google api client is not yet connected");
                return false;
            }

            try
            {
                var status = await LocationServices.GeofencingApi
                    .RemoveGeofencesAsync(Client, GeofencePendingIntent);

                // Started = !status.IsSuccess;

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
            _connectionTaskCompletionSource?.TrySetResult(false);

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
        {
            this.Log().Info("Connected to GoogleApiClient");
            _connectionTaskCompletionSource?.TrySetResult(true);
        }

        public void OnConnectionSuspended(int cause)
        {
            this.Log().Info($"Connection suspended. Cause: {cause}");
            _disconnectionTaskCompletionSource?.TrySetResult(false);
        }

        private void LogSecurityException(Java.Lang.SecurityException securityException)
        {
            var message = "Invalid location permission. " +
                          "Please, add 'ACCESS_FINE_LOCATION' to your manifest. \n" +
                          securityException.Message;

            this.Log().Error(message);
        }

        // TODO implement IDisposable
    }
}