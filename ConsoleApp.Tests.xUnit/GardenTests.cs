using AutoFixture;

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

        [Fact]
        //public void Plant_ChangesNameWhenDuplicated()
        //public void Plant_WhenNameDuplicated_DuplicationCounterAddesToName()
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

    }
}
