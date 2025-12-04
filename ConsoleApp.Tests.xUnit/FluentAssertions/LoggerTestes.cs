using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;

namespace ConsoleApp.Tests.xUnit.FluentAssertions
{
    public class LoggerTestes
    {
        [Fact]
        public void Log_AnyInput_ValidLoggerEventArgs()
        {
            //Arrage
            var logger = new Logger();
            var log = new Fixture().Create<string>();
            object? sender = null;
            Logger.LoggerEventArgs? args = null;
            logger.MessageLogged += (s, e) => { sender = s; args = e; };

            //Act
            logger.Log(log);

            //Assert
            using (new AssertionScope())
            {
                sender.Should().NotBeNull().And.Be(logger);
                args.Should().NotBeNull();
                args.Message.Should().NotBeNull().And.Be(log);
                args.Timestamp.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10));
            }
        }

        [Fact]
        public void Log_AnyInput_ValidLoggerEventArgs_FA()
        {
            //Arrage
            var logger = new Logger();
            var log = new Fixture().Create<string>();
            using var monitor = logger.Monitor();

            //Act
            logger.Log(log);

            //Assert
            using (new AssertionScope())
            {
                monitor.Should().Raise(nameof(logger.MessageLogged))
                    .WithSender(logger)
                    .WithArgs<Logger.LoggerEventArgs>(args => args.Message == log);
            }
        }
    }
}
