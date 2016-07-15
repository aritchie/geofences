using System;
using Acr.Geofencing;
using Autofac;
using Samples.Models;


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
            this.geofences
                .WhenRegionStatusChanged()
                .Subscribe(x => this.conn.Insert(new GeofenceEvent
                {
                    Identifier = x.Region.Identifier,
                    Status = x.Status,
                    DateCreatedUtc = DateTime.UtcNow
                }));
        }
    }
}
