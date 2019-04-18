using Xunit;
using SysSize = System.Drawing.Size;

namespace WinCad.UnitTests
{
    public class ZoomerTests
    {
        [Fact]
        public void ZoomExtents_Zero()
        {
            var zoomer = new Zoomer(padding: 0);

            double actual = zoomer.ZoomFactorForExtents(
                new SysSize(0,0), 
                new Canvas());

            Assert.Equal(0.0, actual);
        }

        [Fact]
        public void ZoomExtents_Size200x100_Factor1()
        {
            AssertFactor(
                factor: 1.0,
                width: 200,
                height: 100,
                new Point(0, 0),
                new Point(200, 100));
        }

        [Fact]
        public void ZoomExtents_Size200x100_SmallestFactorWidth()
        {
            AssertFactor(
                factor: 0.5,
                width: 100,
                height: 100,
                new Point(0, 0),
                new Point(200, 100));
        }

        [Fact]
        public void ZoomExtents_Size200x100_SmallestFactorHeight()
        {
            AssertFactor(
                factor: 0.5,
                width: 100,
                height: 100,
                new Point(0, 0),
                new Point(100, 200));
        }

        [Fact]
        public void ZoomExtents_Size100_FactorHalf()
        {
            AssertFactor(
                factor: 0.5,
                width: 100,
                height: 100,
                new Point(0, 0),
                new Point(200, 200));
        }

        [Fact]
        public void ZoomExtents_Size100_FactorDouble()
        {
            AssertFactor(
                factor: 2.0, 
                width: 100, 
                height: 100, 
                new Point(0, 0), 
                new Point(50, 50));
        }

        [Fact]
        public void ZoomExtents_Size100_OffCenter_FactorHalf()
        {
            AssertFactor(
                factor: 0.5,
                width: 100,
                height: 100,
                new Point(-100, -100),
                new Point(100, 100));
        }

        void AssertFactor(
            double factor, 
            int width, 
            int height, 
            params Point[] points)
        {
            var zoomer = new Zoomer(padding: 0);
            var canvas = new Canvas();
            var pline = new Polyline();
            foreach (var point in points)
                pline.Vertices.Add(point);

            canvas.CurrentLayer.Polylines.Add(pline);

            double actual = zoomer.ZoomFactorForExtents(
              new SysSize(width, height),
              canvas);

            Assert.Equal(factor, actual);
        }
    }
}
