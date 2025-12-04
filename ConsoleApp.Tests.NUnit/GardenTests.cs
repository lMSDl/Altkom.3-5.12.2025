using AutoFixture;
using ConsoleApp.Properties;

namespace ConsoleApp.Tests.NUnit
{
    public class GardenTests
    {
        [Test]
        public void Plant_ValidName_True()
        {
            //Arrange
            const int MINIMAL_VALID_SIZE = 1; 
            const string VALID_NAME = "none_empty_string"; 
            Garden garden = new Garden(MINIMAL_VALID_SIZE);

            //Act
            var result = garden.Plant(VALID_NAME);

            //Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Plant_GardenFull_False()
        {
            //Arrange
            const int MINIMAL_VALID_SIZE = 0;
            string validName = new Fixture().Create<string>();
            Garden garden = new Garden(MINIMAL_VALID_SIZE);

            //Act
            var result = garden.Plant(validName);

            //Assert
            Assert.That(result, Is.False);
        }

        //[Theory]
        [TestCase(2, 3)]
        [TestCase(4, 5)]
        [TestCase(1, 2)]
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
            Assert.That(garden.GetItems(), Does.Contain(expectedName));
        }

        [Test]
        [Ignore("Replaced by Plant_DuplicatedName_DuplicationCounterAddedToName")]
        public void Plant_DuplicatedName_ChangedName()
        {
            //Arrange
            const int MINIMAL_VALID_SIZE = 3;
            string duplicatedName = new Fixture().Create<string>();
            string expectedName = duplicatedName + "3";
            Garden garden = new Garden(MINIMAL_VALID_SIZE);
            _ = garden.Plant(duplicatedName);
            _ = garden.Plant(duplicatedName);

            //Act
            _ = garden.Plant(duplicatedName);

            //Assert
            Assert.That(garden.GetItems(), Does.Contain(expectedName));
        }

        [Test]
        public void Plant_NullName_ArgumentNullException()
        {
            //Arrange
            const int INSIGNIFICANT_SIZE = 0;
            const string? NULL_NAME = null;
            Garden garden = new Garden(INSIGNIFICANT_SIZE);
            const string EXPECTED_PARAM_NAME = "item";

            //Act
            TestDelegate action = () => garden.Plant(NULL_NAME);

            //Assert
            var argumentNullException = Assert.Throws<ArgumentNullException>(action);
            Assert.That(argumentNullException.ParamName, Is.EqualTo(EXPECTED_PARAM_NAME));
        }

        [Theory]
        [TestCase("")]
        [TestCase(" ")] //space
        [TestCase("\t")]
        [TestCase("\n")]
        [TestCase("\r")]
        [TestCase("\r\n\t ")]
        [TestCase(" ")] //#255
        public void Plant_EmptyOrWhitespaceName_ArgumentException(string invalidName)
        {
            //Arrange
            const int INSIGNIFICANT_SIZE = 0;
            Garden garden = new Garden(INSIGNIFICANT_SIZE);
            const string EXPECTED_PARAM_NAME = "item";
            string expectedMessageSubstring = Resources.emptyStringException;

            //Act
            TestDelegate action = () => garden.Plant(invalidName);

            //Assert
            Assert.Throws(Is.InstanceOf<ArgumentException>()
                .And.Property(nameof(ArgumentException.ParamName)).EqualTo(EXPECTED_PARAM_NAME)
                .And.Message.Contains(expectedMessageSubstring),
                action);
        }

        [Test]
        public void GetItems_ReturnsCopyOfPlantsCollection()
        {
            //Arrange
            const int INSIGNIFICANT_SIZE = 0;
            Garden garden = new Garden(INSIGNIFICANT_SIZE);
            var items1 = garden.GetItems();

            //Act
            var items2 = garden.GetItems();

            //Assert
            Assert.That(items1, Is.Not.SameAs(items2));
        }
    }
}
