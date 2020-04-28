using Xunit;

namespace WinCad.UnitTests
{
    public class ZoomerTests
    {
        [Fact]
        public void ZoomExtents_Zero()
        {
            var zoomer = new Zoomer(padding: 0);

            var actual = zoomer.ZoomExtents(
                new Size(0,0), 
                new Canvas());

            Assert.Equal(0.0, actual.ZoomFactor);
        }

        [Fact]
        public void ZoomExtents_Size200x100_Factor1()
        {
            AssertFactor(
                expectedFactor: 1.0,
                width: 200,
                height: 100,
                new Point(0, 0),
                new Point(200, 100));
        }

        [Fact]
        public void ZoomExtents_Size200x100_SmallestFactorWidth()
        {
            AssertFactor(
                expectedFactor: 0.5,
                width: 100,
                height: 100,
                new Point(0, 0),
                new Point(200, 100));
        }

        [Fact]
        public void ZoomExtents_Size200x100_SmallestFactorHeight()
        {
            AssertFactor(
                expectedFactor: 0.5,
                width: 100,
                height: 100,
                new Point(0, 0),
                new Point(100, 200));
        }

        [Fact]
        public void ZoomExtents_Size100_FactorHalf()
        {
            AssertFactor(
                expectedFactor: 0.5,
                width: 100,
                height: 100,
                new Point(0, 0),
                new Point(200, 200));
        }

        [Fact]
        public void ZoomExtents_Size100_FactorDouble()
        {
            AssertFactor(
                expectedFactor: 2.0, 
                width: 100, 
                height: 100, 
                new Point(0, 0), 
                new Point(50, 50));
        }

        [Fact]
        public void ZoomExtents_Size100_OffCenter_FactorHalf()
        {
            AssertOffsetFactor(
                expectedOffset: new Offset(-50, -50),
                expectedFactor: 0.5,
                width: 100,
                height: 100,
                new Point(-100, -100),
                new Point(100, 100));
        }

        void AssertOffsetFactor(
          Offset expectedOffset,
          double expectedFactor,
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

            var actual = zoomer.ZoomExtents(
              new Size(width, height),
              canvas);

            Assert.Equal(expectedOffset, actual.PanningOffset);
            Assert.Equal(expectedFactor, actual.ZoomFactor);
        }

        void AssertFactor(
            double expectedFactor, 
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

            var actual = zoomer.ZoomExtents(
              new Size(width, height),
              canvas);

            Assert.Equal(expectedFactor, actual.ZoomFactor);
        }

        void AssertOffset(
            Offset expectedOffset,
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

            var actual = zoomer.ZoomExtents(
              new Size(width, height),
              canvas);

            Assert.Equal(expectedOffset, actual.PanningOffset);
        }
    }
}
