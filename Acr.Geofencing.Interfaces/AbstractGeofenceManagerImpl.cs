using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Acr.Settings;


namespace Acr.Geofencing {

    public abstract class AbstractGeofenceManagerImpl : IGeofenceManager {
        private const string SETTING_KEY = "geofences-monitor";
        private readonly IList<GeofenceRegion> regions;


        protected AbstractGeofenceManagerImpl() {
            Settings.Local.KeysNotToClear.Add(SETTING_KEY);
            this.regions = Settings.Local.Get(SETTING_KEY, new List<GeofenceRegion>());
        }

        public IReadOnlyList<GeofenceRegion> MonitoredRegions { get; private set; }

        public abstract Task<bool> Initialize();


        public virtual void StartMonitoring(GeofenceRegion region) {
            this.regions.Remove(region);
        }


        public virtual void StopAllMonitoring() {
        }


        public virtual void StopMonitoring(GeofenceRegion region) {
        }


        protected virtual void UpdateRegions() {
            Settings.Local.Set(SETTING_KEY, this.regions);
            this.MonitoredRegions = new ReadOnlyCollection<GeofenceRegion>(this.regions);
        }
    }
}
