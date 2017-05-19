using System.Threading.Tasks;

namespace GeofencePlayground.Geofencing
{
    public interface IGeofencingManager
    {
        IGeofencingManager AddGeofenceData(params GeofenceRegion[] args);
        Task<bool> StartGeofencing();
        Task<bool> StopGeofencing();
    }
}