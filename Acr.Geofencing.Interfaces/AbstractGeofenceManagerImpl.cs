using System;
using System.Collections.Generic;
using System.Linq;


namespace Acr.Geofencing {

    public abstract class AbstractGeofenceManagerImpl : IGeofenceManager {

        protected IList<GeofenceRegion> Regions { get; } = new List<GeofenceRegion>();
        public IReadOnlyList<GeofenceRegion> MonitoredRegions { get; private set; }


        protected abstract void StartMonitoringNative(GeofenceRegion region);
        protected abstract void StopMonitoringNative(GeofenceRegion region);
        public abstract IObservable<GeofenceStatusEvent> WhenRegionStatusChanged();


        public virtual void StartMonitoring(GeofenceRegion region) 
        {
            this.Regions.Add(region);
            this.StartMonitoringNative(region);
        }


        public virtual void StopAllMonitoring() 
        {
            var clone = this.Regions.ToList();
            foreach (var item in clone)
                this.StopMonitoring(item);
        }


        public virtual void StopMonitoring(GeofenceRegion region) 
        {
            if (this.Regions.Remove(region))
                this.StopMonitoringNative(region);
        }
    }
}
