namespace ConsoleApp.Tests.xUnit
{
    public class OrderCalculatorTests
    {

        [Fact]
        public void CalculateOrderTotal_NullParameter_ArgumentNullException()
        {
            // Arrange
            var calculator = new OrderCalculator();
            const IEnumerable<decimal> NULL_PARAMETER = null;

            // Act
            Action action = () => calculator.CalculateOrderTotal(NULL_PARAMETER);

            // Assert
            Assert.Throws<ArgumentNullException>(action);
        }
    }
}
