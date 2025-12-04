using AutoFixture;

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
    }
}

