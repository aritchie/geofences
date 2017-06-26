using System;
using FluentAssertions;
using NUnit.Framework;


namespace Plugin.Geofencing.Interfaces.Tests
{
    [TestFixture]
    public class DistanceTests
    {
        [Test]
        public void MetersToKm()
        {
            Distance
                .FromMeters(2000)
                .TotalKilometers
                .Should()
                .Be(2);
        }


        [Test]
        public void MilesToKm()
        {
            Distance
                .FromMiles(2)
                .TotalKilometers
                .Should()
                .Be(3.21868);
        }


        [Test]
        public void KmToMiles()
        {
            Distance
                .FromKilometers(2)
                .TotalMiles
                .Should()
                .Be(1.242742);
        }
    }
}
