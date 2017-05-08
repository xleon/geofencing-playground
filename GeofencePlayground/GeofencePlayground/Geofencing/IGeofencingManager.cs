using System.Threading.Tasks;

namespace GeofencePlayground.Geofencing
{
    public interface IGeofencingManager
    {
        void AddGeofenceData(params GeofenceData[] args);
        void Connect();
        void Disconnect();
        Task<bool> StartGeofencing();
        Task<bool> StopGeofencing();
    }
}