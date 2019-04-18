using Xunit;
using SysSize = System.Drawing.Size;

namespace WinCad.UnitTests
{
    public class ZoomerTests
    {
        [Fact]
        public void ZoomExtents_Zero()
        {
            var zoomer = new Zoomer();

            double actual = zoomer.ZoomFactorForExtents(
                new SysSize(0,0), 
                new Canvas());

            Assert.Equal(0.0, actual);
        }

        [Fact]
        public void ZoomExtents_Size200x100_Factor1()
        {
            var zoomer = new Zoomer();
            var canvas = new Canvas();
            var pline = new Polyline();
            pline.Vertices.Add(new Point(0, 0));
            pline.Vertices.Add(new Point(200, 100));

            canvas.CurrentLayer.Polylines.Add(pline);
          
            double actual = zoomer.ZoomFactorForExtents(
                new SysSize(200, 100),
                canvas);

            Assert.Equal(1.0, actual);
        }

        [Fact]
        public void ZoomExtents_Size100_FactorHalf()
        {
            var zoomer = new Zoomer();
            var canvas = new Canvas();
            var pline = new Polyline();
            pline.Vertices.Add(new Point(0, 0));
            pline.Vertices.Add(new Point(50, 50));

            canvas.CurrentLayer.Polylines.Add(pline);

            double actual = zoomer.ZoomFactorForExtents(
                new SysSize(100, 100),
                canvas);

            Assert.Equal(0.5, actual);
        }

        [Fact]
        public void ZoomExtents_Size100_FactorDouble()
        {
            var zoomer = new Zoomer();
            var canvas = new Canvas();
            var pline = new Polyline();
            pline.Vertices.Add(new Point(0, 0));
            pline.Vertices.Add(new Point(200, 200));

            canvas.CurrentLayer.Polylines.Add(pline);

            double actual = zoomer.ZoomFactorForExtents(
                new SysSize(100, 100),
                canvas);

            Assert.Equal(2.0, actual);
        }
    }
}
