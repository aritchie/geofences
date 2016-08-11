using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Plugin.Geolocator.Abstractions;


namespace Acr.Geofencing.Generic.Tests
{
    [TestFixture]
    public class Tests
    {

        [Test]
        [Timeout(5000)]
        public async Task Test()
        {
            var tcs = new TaskCompletionSource<GeofenceStatusChangedEventArgs>();

            var geofence = new GeofenceRegion
            {
                Identifier = "Test",
                Center = new Position(1.111, 1.111),
                Radius = Distance.FromKilometers(1)
            };
            var locator = new Mock<IGeolocator>();
            locator
                .Setup(x => x.StartListeningAsync(It.IsAny<int>(), It.IsAny<double>(), false))
                .Returns(() => Task.FromResult(true));

            var mgr = new GeofenceManagerImpl(locator.Object, new GeofenceSettings
            {
                MonitoredRegions = new List<GeofenceRegion>(new [] { geofence })
            });
            mgr.RegionStatusChanged += (sender, args) => tcs.SetResult(args);

            // first raise establishes current status, must do twice
            locator.Raise(
                x => x.PositionChanged += null,
                new PositionEventArgs(new Plugin.Geolocator.Abstractions.Position
                {
                    Latitude = geofence.Center.Latitude,
                    Longitude = geofence.Center.Longitude
                })
            );

            var result = await tcs.Task;

            result.Status.Should().Be(GeofenceStatus.Exited);
            result.Region.Identifier.Should().Be(geofence.Identifier);
        }
    }
}
