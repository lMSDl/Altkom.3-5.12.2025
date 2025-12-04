using AutoFixture;
using System.Globalization;

namespace ConsoleApp.Tests.xUnit
{
    public class LoggerTests
    {
        [Fact]
        public void Log_AnyInput_EventInvokedOnce()
        {
            //Arrage
            var logger = new Logger();
            var log = new Fixture().Create<string>();
            int result = 0;
            const int EXPECTED_INVOCATIONS = 1;
            logger.MessageLogged += (_, _) => result++;

            //Act
            logger.Log(log);

            //Assert
            Assert.Equal(EXPECTED_INVOCATIONS, result);
        }

        [Fact]
        public void Log_AnyInput_ValidLoggerEventArgs()
        {            
            //Arrage
            var logger = new Logger();
            var log = new Fixture().Create<string>();
            object? sender = null;
            Logger.LoggerEventArgs? eventArgs = null;

            logger.MessageLogged += (s, e) => { sender = s; eventArgs = e; };

            //Act
            var dateTimeStart = DateTime.Now;
            logger.Log(log);
            var dateTimeEnd = DateTime.Now;

            //Assert
            Assert.NotNull(sender);
            Assert.Same(logger, sender);
            Assert.NotNull(eventArgs);
            Assert.Equal(log, eventArgs!.Message);
            Assert.InRange(eventArgs!.Timestamp, dateTimeStart, dateTimeEnd);
        }

        [Fact]
        public async Task GetLogAsync_ValidDateRange_LoggerMessage()
        {
            //Arrange
            const int LOG_SPLIT_COUNT = 2;
            const string EXPECTED_DATETIME_FORMAT = "dd.MM.yyyy HH:mm";
            var logger = new Logger();
            var log = new Fixture().Create<string>();

            logger.Log(log);
            var dateTimeStart = DateTime.Now;
            logger.Log(log);
            var dateTimeEnd = DateTime.Now;
            logger.Log(log);

            //Act
            var result = await logger.GetLogsAsync(dateTimeStart, dateTimeEnd);

            //Assert
            var splitted = result.Split(": ");
            Assert.Equal(LOG_SPLIT_COUNT, splitted.Length);
            Assert.Equal(log, splitted[1]);
            Assert.True(DateTime.TryParseExact(splitted[0], EXPECTED_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out _));
        }
    }
}

