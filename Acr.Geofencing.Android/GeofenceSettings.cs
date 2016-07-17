using System;
using System.Collections.Generic;
using Acr.Settings;


namespace Acr.Geofencing
{
    public class GeofenceSettings : AbstractSettingObject
    {
        public IList<GeofenceRegion> MonitoredRegions { get; set; }


        static readonly Lazy<GeofenceSettings> instanceLazy = new Lazy<GeofenceSettings>(() => Acr.Settings.Settings.Local.Bind<GeofenceSettings>());
        public static GeofenceSettings GetInstance()
        {
            return instanceLazy.Value;
        }
    }
}