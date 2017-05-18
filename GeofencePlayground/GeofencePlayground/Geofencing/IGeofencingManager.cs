using System.Threading.Tasks;

namespace GeofencePlayground.Geofencing
{
    public interface IGeofencingManager
    {
        void AddGeofenceData(params GeofenceData[] args);
        Task<bool> StartGeofencing();
        Task<bool> StopGeofencing();
    }
}