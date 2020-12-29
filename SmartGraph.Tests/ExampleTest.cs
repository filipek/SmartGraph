using Xunit;

namespace SmartGraph.Tests
{
    public class ExampleTest
    {
        [Fact]
        public void PassingTest()
        {
            Assert.Equal(4, 2+2);
        }

        [Fact]
        public void FailingTest()
        {
            Assert.Equal(5, 2+2);
        }
    }
}
