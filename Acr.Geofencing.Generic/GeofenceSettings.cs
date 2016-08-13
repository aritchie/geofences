using System;
using System.Collections.Generic;
using Acr.Settings;


namespace Acr.Geofencing
{
    public class GeofenceSettings : AbstractSettingObject
    {
        public IList<GeofenceRegion> MonitoredRegions { get; set; }


        public void Add(GeofenceRegion region)
        {
            this.MonitoredRegions.Add(region);
            this.OnPropertyChanged("MonitoredRegion");
        }


        public void Remove(GeofenceRegion region)
        {
            this.MonitoredRegions.Remove(region);
            this.OnPropertyChanged("MonitoredRegion");
        }


        static readonly Lazy<GeofenceSettings> instanceLazy = new Lazy<GeofenceSettings>(() => Acr.Settings.Settings.Local.Bind<GeofenceSettings>());
        public static GeofenceSettings GetInstance()
        {
            return instanceLazy.Value;
        }
    }
}