using AutoFixture;
using ConsoleApp.Properties;

namespace ConsoleApp.Tests.MSTest
{
    [TestClass]
    public class GardenTests
    {
        [TestMethod]
        public void Plant_ValidName_True()
        {
            //Arrange
            const int MINIMAL_VALID_SIZE = 1;
            const string VALID_NAME = "none_empty_string";
            Garden garden = new Garden(MINIMAL_VALID_SIZE);

            //Act
            var result = garden.Plant(VALID_NAME);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Plant_GardenFull_False()
        {
            //Arrange
            const int MINIMAL_VALID_SIZE = 0;
            string validName = new Fixture().Create<string>();
            Garden garden = new Garden(MINIMAL_VALID_SIZE);

            //Act
            var result = garden.Plant(validName);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(2, 3)]
        [DataRow(4, 5)]
        [DataRow(1, 2)]
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
            Assert.Contains(expectedName, garden.GetItems()); //MSTest V4
            //CollectionAssert.Contains(garden.GetItems().ToList(), expectedName); //MSTest V3
        }

        [TestMethod]
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
            Assert.Contains(expectedName, garden.GetItems());
            // CollectionAssert.Contains(garden.GetItems().ToList(), expectedName);
        }

        //metoda testująca wyjątek - sposób MSTest V3
        /*
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        //[ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void Plant_NullName_ArgumentNullException()
        {
            //Arrange
            const int INSIGNIFICANT_SIZE = 0;
            const string? NULL_NAME = null;
            Garden garden = new Garden(INSIGNIFICANT_SIZE);
            const string EXPECTED_PARAM_NAME = "item";

            //Act
            garden.Plant(NULL_NAME);
        }
        */

        [TestMethod]
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
            //var exception = Assert.ThrowsException<ArgumentNullException>(action); // MSTest V3
            var exception = Assert.ThrowsExactly<ArgumentNullException>(action); //sprawdzamy konkretny typ wyjątku
            //var exception = Assert.Throws<ArgumentException>(action); //uzwdlędnienie dziedziczenia
            Assert.AreEqual(EXPECTED_PARAM_NAME, exception.ParamName);
        }

        [TestMethod]
        [Ignore("Replaced by Plant_EmptyName_ArgumentException")]
        public void Plant_EmptyName_ArgumentException()
        {
            //Arrange
            const int INSIGNIFICANT_SIZE = 0;
            const string EMPTY_NAME = "";
            Garden garden = new Garden(INSIGNIFICANT_SIZE);
            const string EXPECTED_PARAM_NAME = "item";
            string expectedMessageSubstring = Resources.emptyStringException;

            //Act
            Action action = () => garden.Plant(EMPTY_NAME);

            //Assert
            var argumentException = Assert.ThrowsExactly<ArgumentException>(action);
            AssertItemParameter(EXPECTED_PARAM_NAME, expectedMessageSubstring, argumentException);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")] //space
        [DataRow("\t")]
        [DataRow("\n")]
        [DataRow("\r")]
        [DataRow("\r\n\t ")]
        [DataRow(" ")] //#255
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
            var argumentException = Assert.ThrowsExactly<ArgumentException>(action);
            AssertItemParameter(EXPECTED_PARAM_NAME, expectedMessageSubstring, argumentException);
        }

        //metoda pomocnicza do asercji powtarzających się w testach
        private static void AssertItemParameter(string expectedParamName, string expectedMessageSubstring, ArgumentException argumentException)
        {
            Assert.AreEqual(expectedParamName, argumentException.ParamName);
            Assert.Contains(expectedMessageSubstring, argumentException.Message); //MSTest V4
            //StringAssert.Contains(argumentException.Message, expectedMessageSubstring); //MSTest V3
        }

        [TestMethod]
        public void GetItems_ReturnsCopyOfPlantsCollection()
        {
            //Arrange
            const int INSIGNIFICANT_SIZE = 0;
            Garden garden = new Garden(INSIGNIFICANT_SIZE);
            var items1 = garden.GetItems();

            //Act
            var items2 = garden.GetItems();

            //Assert
            Assert.AreNotSame(items1, items2);
        }
    }
}
