using TFlexApp;

namespace TFlexApp.Tests
{
    public class ModelTests
    {
        [Fact]
        public void Designation_WithoutHole_NoDiameterSuffix()
        {
            Model model = new()
            {
                Length = 100,
                Height = 50,
                Thickness = 10,
                HasHole = false
            };
            Assert.Equal("АБВГ-100-50-10", model.Designation);
        }

        [Fact]
        public void Designation_WithHole_IncludesDiameterSuffix()
        {
            Model model = new()
            {
                Length = 100,
                Height = 50,
                Thickness = 10,
                HasHole = true,
                HoleDiameter = 20
            };
            Assert.Equal("АБВГ-100-50-10-D20", model.Designation);
        }

        [Fact]
        public void PartName_WithoutHole_NoHoleText()
        {
            Model model = new()
            {
                Length = 100,
                Height = 50,
                Thickness = 10,
                HasHole = false
            };
            Assert.Equal("деталь 100*50*10", model.PartName);
        }

        [Fact]
        public void PartName_WithHole_IncludesHoleText()
        {
            Model model = new()
            {
                Length = 100,
                Height = 50,
                Thickness = 10,
                HasHole = true,
                HoleDiameter = 20
            };
            Assert.Equal("деталь 100*50*10 с отверстием 20", model.PartName);
        }
    }
}
