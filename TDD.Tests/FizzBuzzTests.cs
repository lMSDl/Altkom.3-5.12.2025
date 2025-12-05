namespace TDD.Tests
{
    public class FizzBuzzTests
    {
        [Theory]
        [InlineData(1, "1")]
        [InlineData(3, "Fizz")]
        [InlineData(5, "Buzz")]
        [InlineData(6, "Fizz")]
        [InlineData(15, "FizzBuzz")]
        public void GetFizzBuzz_AnyNumber_CorrectString(int input, string output)
        {
            // Act
            var result = FizzBuzz.GetFizzBuzz(input);
            // Assert
            Assert.Equal(output, result);
        }

        [Theory]
        [InlineData(1, "1")]
        [InlineData(3, "1 2 Fizz")]
        [InlineData(15, "1 2 Fizz 4 Buzz Fizz 7 8 Fizz Buzz 11 Fizz 13 14 FizzBuzz")]
        public void GetSequence(int count, string exprectedOutput)
        {
            //Act
            var result = FizzBuzz.GetSequence(count);

            //Assert
            Assert.Equal(exprectedOutput, result);
        }
    }
}
