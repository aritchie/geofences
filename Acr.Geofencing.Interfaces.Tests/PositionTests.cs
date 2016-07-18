using System;
using FluentAssertions;
using NUnit.Framework;


namespace Acr.Geofencing.Interfaces.Tests
{
    [TestFixture]
    public class PositionTests
    {

        [Test]
        public void Inside_Geofence()
        {
            var center = new Position(43.6411314, -79.3808415); // 88 queen's quay
            var current = new Position(43.6429228, -79.3789959); // union station
            var distance = center.GetDistanceTo(current);
            Assert.IsTrue(distance < Distance.FromKilometers(1), "Union station is less than a 1000 meters away");

            PositionUtils
                .IsInsideGeofence(center, current, Distance.FromKilometers(3))
                .Should()
                .Be(true, "Union station is inside the 3km geofence from 88 Queen's Quay");

        }


        [Test]
        public void Outside_Geofence()
        {
            var center = new Position(43.6411314, -79.3808415); // 88 queen's quay
            var current = new Position(43.6515754, -79.3492364); // random point outside fence

            PositionUtils
                .IsInsideGeofence(center, current, Distance.FromKilometers(2))
                .Should()
                .Be(false);
        }
    }
}
