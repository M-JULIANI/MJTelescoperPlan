using Elements;
using Xunit;
using System;
using System.IO;
using System.Collections.Generic;
using Elements.Serialization.glTF;


namespace MJTelescoperPlan
{
    public class RangeTests
    {
        [Theory]
        [MemberData(nameof(VerticalTestData))]
        public void VerticalOffsetTests(double minHeight, double maxHeight, int index, int maxIndex, double expected)
        {
            double calculated = TelescopePlan.ComputeVerticalDistance(minHeight, maxHeight, index, maxIndex);
            Console.WriteLine("calc: " + calculated + " expected: " + expected);
            Assert.Equal(calculated, expected);
        }


        public static IEnumerable<object[]> VerticalTestData()
        {
            yield return new object[] { 10, 100, 0, 3, 100 };
            yield return new object[] { 10, 100, 1, 3, 55 };
            yield return new object[] { 10, 100, 2, 3, 10 };
        }
    }

    public class HorizontalRangeTests
    {
        [Theory]
        [MemberData(nameof(HorizontalTestData))]
        public void HorizontalOffsetTests(double maxTelescope, int index, int maxIndex, double minModule, double expected)
        {
            double calculated = TelescopePlan.ComputeHorizontalDistance(index, maxIndex, maxTelescope, minModule);
            Console.WriteLine("calc: " + calculated + " expected: " + expected);
            Assert.Equal(calculated, expected);
        }


        public static IEnumerable<object[]> HorizontalTestData()
        {
            yield return new object[] { 60, 0, 4, 3, 3 };
            yield return new object[] { 60, 1, 4, 3, 20 };
            yield return new object[] { 60, 2, 4, 3, 40 };
            yield return new object[] { 60, 3, 4, 3, 60 };
        }
    }
}