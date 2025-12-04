using AutoFixture;
using ConsoleApp.Properties;
using FluentAssertions;
using FluentAssertions.Execution;
using System;

namespace ConsoleApp.Tests.xUnit.FluentAssertions
{
    public class GardenTests
    {

        [Fact]
        public void Plant_ValidName_True()
        {
            //Arrange
            const int MINIMAL_VALID_SIZE = 1; 
            const string VALID_NAME = "none_empty_string";
            Garden garden = new Garden(MINIMAL_VALID_SIZE);

            //Act
            var result = garden.Plant(VALID_NAME);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Plant_GardenFull_False()
        {
            //Arrange
            const int MINIMAL_VALID_SIZE = 0;
            string validName = new Fixture().Create<string>();
            Garden garden = new Garden(MINIMAL_VALID_SIZE);

            //Act
            var result = garden.Plant(validName);

            //Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(2, 3)]
        [InlineData(4, 5)]
        [InlineData(1, 2)]
        public void Plant_WhenNameDuplicated_DuplicationCounterAddesToName(int numberOdCopies, int expectedCounter)
        {
            //Arrange
            string duplicatedName = new Fixture().Create<string>();
            string expectedName = duplicatedName + expectedCounter;
            Garden garden = new Garden(expectedCounter);
            Enumerable.Repeat(duplicatedName, numberOdCopies).ToList().ForEach(_ => garden.Plant(duplicatedName));

            //Act
            _ = garden.Plant(duplicatedName);

            //Assert
            garden.GetItems().Should().Contain(expectedName);
        }

        [Fact]
        public void Plant_NullName_ArgumentNullException()
        {
            //Arrange
            const int INSIGNIFICANT_SIZE = 0;
            const string? NULL_NAME = null;
            Garden garden = new Garden(INSIGNIFICANT_SIZE);
            const string EXPECTED_PARAM_NAME = "item";

            //Act
            Action action = () => garden.Plant(NULL_NAME);

            //Assert

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be(EXPECTED_PARAM_NAME);
        }


        [Theory]
        [InlineData("")]
        [InlineData(" ")] //space
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\r\n\t ")]
        [InlineData(" ")] //#255
        public void Plant_EmptyOrWhitespaceName_ArgumentException(string invalidName)
        {
            //Arrange
            const int INSIGNIFICANT_SIZE = 0;
            Garden garden = new Garden(INSIGNIFICANT_SIZE);
            const string EXPECTED_PARAM_NAME = "item";
            string expectedMessageSubstring = Resources.emptyStringException;

            //Act
            Action action = () => garden.Plant(invalidName);

            //Assert
            using (new AssertionScope())
            {
                action.Should().Throw<ArgumentException>()
                    .WithMessage($"*{expectedMessageSubstring}")
                    .And.ParamName.Should().Be(EXPECTED_PARAM_NAME);
            }
        }

        [Fact]
        public void GetItems_ReturnsCopyOfPlantsCollection()
        {
            //Arrange
            const int INSIGNIFICANT_SIZE = 0;
            Garden garden = new Garden(INSIGNIFICANT_SIZE);
            var items1 = garden.GetItems();

            //Act
            var items2 = garden.GetItems();

            //Assert
            items1.Should().NotBeSameAs(items2);
        }
    }
}
