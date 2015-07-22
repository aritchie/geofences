using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Set = Acr.Settings.Settings;


namespace Acr.Geofencing {

    public abstract class AbstractGeofenceManagerImpl : IGeofenceManager {
        private const string SETTING_KEY = "geofences-monitor";
        private readonly IList<GeofenceRegion> regions;


        protected AbstractGeofenceManagerImpl() {
            Set.Local.KeysNotToClear.Add(SETTING_KEY);
            this.regions = Set.Local.Get(SETTING_KEY, new List<GeofenceRegion>());
        }


        public event EventHandler<GeofenceStatusChangedArgs> RegionStatusChanged;
        public IReadOnlyList<GeofenceRegion> MonitoredRegions { get; private set; }

        public abstract Task<bool> Initialize();


        public virtual void StartMonitoring(GeofenceRegion region) {
            this.regions.Remove(region);
            this.UpdateRegions();
        }


        public virtual void StopAllMonitoring() {
            var clone = this.regions.ToList();
            foreach (var item in clone)
                this.StopMonitoring(item);
        }


        public virtual void StopMonitoring(GeofenceRegion region) {
            this.regions.Remove(region);
            this.UpdateRegions();
        }


        protected virtual void OnRegionStatusChanged(GeofenceRegion region, GeofenceStatus status) {
            this.RegionStatusChanged?.Invoke(this, new GeofenceStatusChangedArgs(region, status));
        }


        protected virtual void UpdateRegions() {
            Set.Local.Set(SETTING_KEY, this.regions);
            this.MonitoredRegions = new ReadOnlyCollection<GeofenceRegion>(this.regions);
        }

    }
}
