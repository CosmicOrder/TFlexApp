using Moq;

namespace TFlexApp.Tests
{
    public class PresenterTests
    {
        private readonly Mock<IView> _mockView;
        private readonly Mock<ICadFacade> _mockCad;
        private readonly Presenter _presenter;

        public PresenterTests()
        {
            _mockView = new Mock<IView>();
            _mockCad = new Mock<ICadFacade>();
            _presenter = new Presenter(_mockView.Object, _mockCad.Object);
        }

        [Fact]
        public void Run_WithEmptyFields_ShowsDimensionsRequiredError()
        {
            _mockView.Setup(v => v.PartLength).Returns("");
            _mockView.Setup(v => v.PartHeight).Returns("");
            _mockView.Setup(v => v.PartThickness).Returns("");
            _mockView.Setup(v => v.HasHole).Returns(false);

            _mockView.Raise(v => v.RunRequested += null, EventArgs.Empty);

            _mockView.Verify(
                v => v.ShowError(Messages.ErrDimensionsRequired, Messages.TitleError),
                Times.Once);
        }

        [Fact]
        public void Run_WithNegativeValues_ShowsMustBePositiveError()
        {
            _mockView.Setup(v => v.PartLength).Returns("-10");
            _mockView.Setup(v => v.PartHeight).Returns("20");
            _mockView.Setup(v => v.PartThickness).Returns("5");
            _mockView.Setup(v => v.HasHole).Returns(false);

            _mockView.Raise(v => v.RunRequested += null, EventArgs.Empty);

            _mockView.Verify(
                v => v.ShowError(Messages.ErrDimensionsMustBePositive, Messages.TitleError),
                Times.Once);
        }

        [Fact]
        public void Run_WithHoleButEmptyDiameter_ShowsHoleDiameterRequiredError()
        {
            _mockView.Setup(v => v.PartLength).Returns("100");
            _mockView.Setup(v => v.PartHeight).Returns("50");
            _mockView.Setup(v => v.PartThickness).Returns("10");
            _mockView.Setup(v => v.HasHole).Returns(true);
            _mockView.Setup(v => v.HoleDiameter).Returns("");

            _mockView.Raise(v => v.RunRequested += null, EventArgs.Empty);

            _mockView.Verify(
                v => v.ShowError(Messages.ErrHoleDiameterRequired, Messages.TitleError),
                Times.Once);
        }

        [Fact]
        public void Run_WithHoleDiameterExceedingLimit_ShowsExceedsLimitError()
        {
            _mockView.Setup(v => v.PartLength).Returns("100");
            _mockView.Setup(v => v.PartHeight).Returns("50");
            _mockView.Setup(v => v.PartThickness).Returns("10");
            _mockView.Setup(v => v.HasHole).Returns(true);
            _mockView.Setup(v => v.HoleDiameter).Returns("40"); // 0.7 * min(100,50) = 35

            _mockView.Raise(v => v.RunRequested += null, EventArgs.Empty);

            _mockView.Verify(
                v => v.ShowError(It.Is<string>(s => s.Contains("40")), Messages.TitleError),
                Times.Once);
        }

        [Fact]
        public void Run_WithValidData_CallsCreatePartDrawingAndShowsSuccess()
        {
            _mockView.Setup(v => v.PartLength).Returns("100");
            _mockView.Setup(v => v.PartHeight).Returns("50");
            _mockView.Setup(v => v.PartThickness).Returns("10");
            _mockView.Setup(v => v.HasHole).Returns(false);

            _mockView.Raise(v => v.RunRequested += null, EventArgs.Empty);

            _mockCad.Verify(c => c.CreatePartDrawing(It.IsAny<Model>()), Times.Once);
            _mockView.Verify(v => v.ShowError(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockView.Verify(
                v => v.ShowSuccess(It.IsAny<string>(), Messages.TitleSuccess),
                Times.Once);
        }

        [Fact]
        public void Run_WithValidDataAndHole_CallsCreatePartDrawingAndShowsSuccess()
        {
            _mockView.Setup(v => v.PartLength).Returns("100");
            _mockView.Setup(v => v.PartHeight).Returns("50");
            _mockView.Setup(v => v.PartThickness).Returns("10");
            _mockView.Setup(v => v.HasHole).Returns(true);
            _mockView.Setup(v => v.HoleDiameter).Returns("20");

            _mockView.Raise(v => v.RunRequested += null, EventArgs.Empty);

            _mockCad.Verify(c => c.CreatePartDrawing(It.Is<Model>(m => m.HasHole && m.HoleDiameter == 20)), Times.Once);
            _mockView.Verify(v => v.ShowError(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockView.Verify(
                v => v.ShowSuccess(It.IsAny<string>(), Messages.TitleSuccess),
                Times.Once);
        }
    }
}