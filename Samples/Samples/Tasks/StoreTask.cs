using System;
using Acr.Geofencing;
using Autofac;


namespace Samples.Tasks
{
    public class StoreTask : IStartable
    {
        readonly IGeofenceManager geofences;
        readonly SampleDbConnection conn;


        public StoreTask(IGeofenceManager geofences, SampleDbConnection conn)
        {
            this.geofences = geofences;
            this.conn = conn;
        }


        public void Start()
        {
        }
    }
}
