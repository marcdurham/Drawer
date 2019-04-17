using Xunit;

namespace WinCad.UnitTests
{
    public class BoxRenderTests
    {
        [Fact]
        public void BoxNull()
        {
            var builder = new BoxBuilder();

            var actual = builder.Draw(null, null);

            Assert.Empty(actual.Vertices);
        }

        [Fact]
        public void BoxZeroToZero()
        {
            var builder = new BoxBuilder();

            var actual = builder.Draw(new Point(0, 0), new Point(0, 0));

            AssertPointAt(actual.Vertices[0], 0, 0);
            AssertPointAt(actual.Vertices[1], 0, 0);
            AssertPointAt(actual.Vertices[2], 0, 0);
            AssertPointAt(actual.Vertices[3], 0, 0);
            AssertPointAt(actual.Vertices[4], 0, 0);
        }

        [Fact]
        public void BoxZeroToTenFive()
        {
            var builder = new BoxBuilder();

            var actual = builder.Draw(new Point(0, 0), new Point(10, 5));

            AssertPointAt(actual.Vertices[0], 0, 0);
            AssertPointAt(actual.Vertices[1], 10, 0);
            AssertPointAt(actual.Vertices[2], 10, 5);
            AssertPointAt(actual.Vertices[3], 0, 5);
            AssertPointAt(actual.Vertices[4], 0, 0);
        }

        [Fact]
        public void BoxTenFiveToZero()
        {
            var builder = new BoxBuilder();

            var actual = builder.Draw(new Point(10, 5), new Point(0, 0));

            AssertPointAt(actual.Vertices[0], 0, 0);
            AssertPointAt(actual.Vertices[1], 10, 0);
            AssertPointAt(actual.Vertices[2], 10, 5);
            AssertPointAt(actual.Vertices[3], 0, 5);
            AssertPointAt(actual.Vertices[4], 0, 0);
        }

        [Fact]
        public void BoxNegativeTenFiveToZero()
        {
            var builder = new BoxBuilder();

            var actual = builder.Draw(new Point(-10, 5), new Point(0, 0));

            AssertPointAt(actual.Vertices[0], -10, 0);
            AssertPointAt(actual.Vertices[1], 0, 0);
            AssertPointAt(actual.Vertices[2], 0, 5);
            AssertPointAt(actual.Vertices[3], -10, 5);
            AssertPointAt(actual.Vertices[4], -10, 0);
        }

        void AssertPointAt(Point point, double x, double y)
        {
            Assert.Equal(x, point.X);
            Assert.Equal(y, point.Y);
        }
    }
}
