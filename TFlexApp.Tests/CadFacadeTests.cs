using TFlexApp;

namespace TFlexApp.Tests
{
    public class CadFacadeTests
    {
        [Fact]
        public void SelectStandardScale_ModelFitsExactly_ReturnsOneToOne()
        {
            var scale = CadFacade.SelectStandardScale(100, 100, 100, 100);
            Assert.Equal(1, scale.Value);
            Assert.Equal("1:1", scale.Text);
        }

        [Fact]
        public void SelectStandardScale_ModelTooLarge_ReturnsSmallerScale()
        {
            var scale = CadFacade.SelectStandardScale(200, 200, 100, 100);
            Assert.Equal(0.5, scale.Value);
            Assert.Equal("1:2", scale.Text);
        }

        [Fact]
        public void SelectStandardScale_ModelSmall_ReturnsTwentyToOne()
        {
            var scale = CadFacade.SelectStandardScale(5, 5, 100, 100);
            Assert.Equal(20, scale.Value);
            Assert.Equal("20:1", scale.Text);
        }

        [Fact]
        public void SelectStandardScale_NothingFits_ReturnsLastScale()
        {
            var scale = CadFacade.SelectStandardScale(1000000, 1000000, 100, 100);
            Assert.Equal(0.01, scale.Value);
            Assert.Equal("1:100", scale.Text);
        }

        [Fact]
        public void SelectStandardScale_WidthConstrains_ReturnsCorrectScale()
        {
            //required = min(100/300, 100/50) = min(0.333, 2) = 0.333 → 1:2.5 (0.4 > 0.333, нет) → 1:4 (0.25 <= 0.333, да)
            var scale = CadFacade.SelectStandardScale(300, 50, 100, 100);
            Assert.Equal(0.25, scale.Value);
            Assert.Equal("1:4", scale.Text);
        }
    }
}
