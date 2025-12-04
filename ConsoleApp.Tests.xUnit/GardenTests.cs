using AutoFixture;
using ConsoleApp.Properties;
using Moq;

namespace ConsoleApp.Tests.xUnit
{
    public class GardenTests /*: IDisposable*/
    {
        //używanie metod setup i teardown w testach jednostkowych jest niezalecane,
        //ponieważ testy powinny być niezależne i nie powinny mieć efektów ubocznych.
        //metody setup i teardown są wywoływane przed i po KAŻDYM teście
        /*
                Garden Garden { get; set; }
                //xUnit nie posiada metod setup, ale możemy użyć konstruktora klasy testowej
                public GardenTests()
                {
                    Garden = new Garden(3);
                }

                // xUnit nie posiada metod teardown, ale możemy użyć metody Dispose
                public void Dispose()
                {
                    Garden = null;
                }
        */

        //Zamiast setup/teardown możemy używać metod pomocniczych jak poniższa, któe są konfigurowalne w zależności od potrzeb testu
        private static Garden CreateGarden(int minimalValidSize)
        {
            return new Garden(minimalValidSize);
        }



        [Fact]
        //public void Plant_GivesTrueWhenNameIsValid()
        //public void Plant_PassValidName_ReturnsTrue()
        //<nazwa_metody>_<scenariusz>_<oczekiwany_wynik>
        public void Plant_ValidName_True()
        {
            //Arrange
            //opisujemy swoje intencje
            const int MINIMAL_VALID_SIZE = 1; //używamy parametrów, któe dają minimalny przekaz (o jak najmniejszym polu do interpretacji)
            const string VALID_NAME = "none_empty_string"; //minimalistyczny parametr, który jest wystarczający do testu (unikamy nadinterpretacji)
            Garden garden = CreateGarden(MINIMAL_VALID_SIZE);

            //Act
            var result = garden.Plant(VALID_NAME);

            //Assert
            Assert.True(result);
        }

        [Fact]
        //public void Plant_GivesFalseWhenNotEnoughtSpace()
        //public void Plant_NotEnoughtSpace_ReturnsFalse()
        public void Plant_GardenFull_False()
        {
            //Arrange
            //opisujemy swoje intencje
            const int MINIMAL_VALID_SIZE = 0;
            //można wykorzysać AutoFixture do generowania danych testowych
            string validName = new Fixture().Create<string>();
            Garden garden = new Garden(MINIMAL_VALID_SIZE);

            //Act
            var result = garden.Plant(validName);

            //Assert
            Assert.False(result);
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
            //staramy się unikać pętli, jeśli już to używamy LINQ (unikamy wtedy logiki)
            Enumerable.Repeat(duplicatedName, numberOdCopies).ToList().ForEach(_ => garden.Plant(duplicatedName));

            //Act
            _ = garden.Plant(duplicatedName);

            //Assert
            Assert.Contains(expectedName, garden.GetItems());
        }

        [Fact(Skip = "Replaced by Plant_DuplicatedName_DuplicationCounterAddedToName")]
        //public void Plant_ChangesNameWhenDuplicated()
        public void Plant_DuplicatedName_ChangedName()
        {
            //Arrange
            const int MINIMAL_VALID_SIZE = 3;
            string duplicatedName = new Fixture().Create<string>();
            string expectedName = duplicatedName + "3"; //oczekujemy, że metoda Plant doda "3" do nazwy, ponieważ będą już dwie rośliny o tej nazwie
            Garden garden = new Garden(MINIMAL_VALID_SIZE);
            _ = garden.Plant(duplicatedName);
            _ = garden.Plant(duplicatedName);

            //Act
            _ = garden.Plant(duplicatedName);

            //Assert
            Assert.Contains(expectedName, garden.GetItems());
            //powinniśmy unikać asercji wielu rzeczy w jednym teście, więc nie łączymy tego testu z testem sprawdzającym czy metoda Plant zwraca true
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
            //var argumentNullException = Assert.ThrowsAny<ArgumentException>(action); //uwzględnia wszystkie wyjątki dziedziczące po ArgumentException
            var argumentNullException = Assert.Throws<ArgumentNullException>(action); //sprawdzamy konkretny typ wyjątku
            Assert.Equal(EXPECTED_PARAM_NAME, argumentNullException.ParamName);
        }

        [Fact(Skip = "Replaced by Plant_EmptyName_ArgumentException")]
        public void Plant_EmptyName_ArgumentException()
        {
            //Arrange
            const int INSIGNIFICANT_SIZE = 0;
            const string EMPTY_NAME = "";
            Garden garden = new Garden(INSIGNIFICANT_SIZE);
            const string EXPECTED_PARAM_NAME = "item";
            string expectedMessageSubstring = Resources.emptyStringException;

            //Act
            var recordException = Record.Exception(() => garden.Plant(EMPTY_NAME));

            //Assert
            var argumentException = Assert.IsType<ArgumentException>(recordException); //sprawdzamy konkretny typ wyjątku
            //Assert.Equal(EXPECTED_PARAM_NAME, argumentException.ParamName);
            //Assert.Contains(expectedMessageSubstring, argumentException.Message);
            AssertItemParameter(EXPECTED_PARAM_NAME, expectedMessageSubstring, argumentException);
        }

        /*public static IEnumerable<object[]> InvalidNames()
        {
            yield return new object[] { "" };
            yield return new object[] { " " };
            yield return new object[] { "\n" };
            yield return new object[] { "\t" };
            yield return new object[] { "\r" };
            yield return new object[] { "\r\n\t " };
            yield return new object[] { " " }; //#255
        }*/


        //Theory - testy z parametrami, które pozwalają na uruchomienie tej samej metody z różnymi danymi sprawdzającymi różne warunki (w tym brzegowe)
        [Theory]
        [InlineData("")]
        [InlineData(" ")] //space
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\r\n\t ")]
        [InlineData(" ")] //#255
        //[MemberData(nameof(InvalidNames))]
        //[ClassData(nameof(<nazwa_klasy>))] //klasa musi implementować IEnumerable<object[]>
        public void Plant_EmptyOrWhitespaceName_ArgumentException(string invalidName)
        {
            //Arrange
            const int INSIGNIFICANT_SIZE = 0;
            Garden garden = new Garden(INSIGNIFICANT_SIZE);
            const string EXPECTED_PARAM_NAME = "item";
            string expectedMessageSubstring = Resources.emptyStringException;

            //Act
            var recordException = Record.Exception(() => garden.Plant(invalidName));

            //Assert
            var argumentException = Assert.IsType<ArgumentException>(recordException); //sprawdzamy konkretny typ wyjątku

            //Assert.Equal(EXPECTED_PARAM_NAME, argumentException.ParamName);
            //Assert.Contains(expectedMessageSubstring, argumentException.Message);
            AssertItemParameter(EXPECTED_PARAM_NAME, expectedMessageSubstring, argumentException);
        }

        //metoda pomocnicza do asercji powtarzających się w testach
        private static void AssertItemParameter(string expectedParamName, string expectedMessageSubstring, ArgumentException argumentException)
        {
            Assert.Equal(expectedParamName, argumentException.ParamName);
            Assert.Contains(expectedMessageSubstring, argumentException.Message);
        }

        [Fact]
        public void GetItems_ReturnsCopyOfPlantsCollection()
        {
            //Arrange
            const int INSIGNIFICANT_SIZE = 0;

            Mock<ILogger> loggerStub = new Mock<ILogger>();
            //stub - obiekt, który nie ma żadnej logiki, ale jest wymagany przez konstruktor albo metodę
            //od niego nie zależy wynik testu
            Garden garden = new Garden(INSIGNIFICANT_SIZE, loggerStub.Object); //zakładamy, że Garden ma tylko konsturktor z 2 parametrami
            var items1 = garden.GetItems();

            //Act
            var items2 = garden.GetItems();

            //Assert
            Assert.NotSame(items1, items2); //sprawdzamy, czy to nie jest ta sama referencja
        }

        [Fact]
        public void Plant_ValidName_MessageLogged()
        {
            //Arrange
            var fixture = new Fixture();
            const int MINIMAL_VALID_SIZE = 1;
            string validName = fixture.Create<string>();
            Mock<ILogger> loggerMock = new Mock<ILogger>();

            //loggerMock.Setup(x => x.Log(It.IsAny<string>())).Verifiable(Times.Exactly(2)); //It.IsAny<string>() - dowolny string
            //loggerMock.Setup(x => x.Log(string.Format(Resources.PlantedInGarden, validName))).Verifiable();
            loggerMock.Setup(x => x.Log(string.Format(Resources.PlantedInGarden, validName))).Verifiable(Times.Once);

            Garden garden = new Garden(MINIMAL_VALID_SIZE, loggerMock.Object);

            //Act
            garden.Plant(validName);

            //Assert
            loggerMock.Verify();
        }

        [Fact]
        public void Plant_DuplicatedName_MessageLogged()
        {
            //Arrange
            var fixture = new Fixture();
            const int MINIMAL_VALID_SIZE = 2;
            string validName = fixture.Create<string>();
            string duplicatedName = validName + "2";

            Mock<ILogger> loggerMock = new Mock<ILogger>();
            Garden garden = new Garden(MINIMAL_VALID_SIZE, loggerMock.Object);
            garden.Plant(validName);

            //Act
            garden.Plant(validName);

            //Assert

            loggerMock.Verify(x => x.Log(string.Format(Resources.PlantNameChanged, validName, duplicatedName)), Times.Once);
            //loggerMock.Verify(x => x.Log(It.Is<string>(s => s.Contains(validName))), Times.Exactly(3)); //It.Is<> - pozwala na bardziej zaawansowane dopasowanie parametrów
            //loggerMock.Verify(x => x.Log(It.Is<string>(s => s.Contains(duplicatedName))), Times.Exactly(2));
        }

        [Fact]
        public void GetLastLog_SingleLog()
        {
            //Arrange
            var fixture = new Fixture();
            const int INSIGNIFICANT_SIZE = 0;
            var logs = fixture.CreateMany<string>(3).ToList();

            Mock<ILogger> loggerMock = new Mock<ILogger>();
            loggerMock.Setup(x => x.GetLogsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(string.Join("\n",logs));

            Garden garden = new Garden(INSIGNIFICANT_SIZE, loggerMock.Object);

            //Act
            var result = garden.GetLastLog();

            //Assert
            Assert.Equal(logs.Last(), result);
        }
    }
}
