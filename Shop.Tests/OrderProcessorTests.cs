using AutoFixture;
using Moq;

namespace Shop.Tests
{
    public class OrderProcessorTests
    {
        [Fact]
        public void ProcessOrder_StockIsInsufficient_False()
        {
            //Arrange
            var inventoryService = new Mock<IInventoryService>();
            var orderRepository = new Mock<IOrderRepository>();
            var paymentService = new Mock<IPaymentService>();

            var orderProcessor = new OrderProcessor(orderRepository.Object, inventoryService.Object, paymentService.Object);

            var fixture = new Fixture();
            var order = fixture.Create<Order>();
            var cardNumber = fixture.Create<string>();

            var lastItem = order.Items.Last();
            var otherItems = order.Items.Take(order.Items.Count - 1).ToList();

            inventoryService.Setup(x => x.CheckStock(It.IsAny<int>(), It.IsAny<int>())).Returns(true).Verifiable(Times.Exactly(otherItems.Count));
            //inventoryService.Setup(x => x.CheckStock(It.IsIn(otherItems.Select(x => x.ProductId)), It.IsIn(otherItems.Select(x => x.Quantity)))).Returns(true).Verifiable(Times.Exactly(otherItems.Count));
            //otherItems.ForEach(item => inventoryService.Setup(x => x.CheckStock(item.ProductId, item.Quantity)).Returns(true).Verifiable(Times.Once));

            inventoryService.Setup(x => x.CheckStock(lastItem.ProductId, lastItem.Quantity)).Returns(false).Verifiable(Times.Once);

            //Act
            var result = orderProcessor.ProcessOrder(order, cardNumber);

            //Assert
            Assert.False(result);
            inventoryService.Verify();
        }


        [Fact]
        public void ProcessOrder_PaymentFailed_False()
        {
            //Arrange
            var inventoryService = new Mock<IInventoryService>();
            var orderRepository = new Mock<IOrderRepository>();
            var paymentService = new Mock<IPaymentService>();

            var orderProcessor = new OrderProcessor(orderRepository.Object, inventoryService.Object, paymentService.Object);

            var fixture = new Fixture();
            var order = fixture.Create<Order>();
            var cardNumber = fixture.Create<string>();

            inventoryService.Setup(x => x.CheckStock(It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            paymentService.Setup(x => x.ProcessPayment(cardNumber, It.IsAny<decimal>())).Returns(false).Verifiable(Times.Once);

            //Act
            var result = orderProcessor.ProcessOrder(order, cardNumber);

            //Assert
            Assert.False(result);
            paymentService.Verify();
        }


        [Fact]
        public void ProcessOrder_ConditionsMet_True()
        {
            //Arrange
            var inventoryService = new Mock<IInventoryService>();
            var orderRepository = new Mock<IOrderRepository>();
            var paymentService = new Mock<IPaymentService>();

            var orderProcessor = new OrderProcessor(orderRepository.Object, inventoryService.Object, paymentService.Object);

            var fixture = new Fixture();
            var order = fixture.Create<Order>();
            var cardNumber = fixture.Create<string>();


            inventoryService.Setup(x => x.CheckStock(It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            paymentService.Setup(x => x.ProcessPayment(cardNumber, It.IsAny<decimal>())).Returns(true);
            inventoryService.Setup(x => x.ReserveStock(It.IsIn(order.Items.Select(x => x.ProductId)), It.IsIn(order.Items.Select(x => x.Quantity))))
                .Verifiable(Times.Exactly(order.Items.Count));
            orderRepository.Setup(x => x.SaveOrder(order)).Verifiable(Times.Once);


            //Act
            var result = orderProcessor.ProcessOrder(order, cardNumber);

            //Assert
            Assert.True(result);
            inventoryService.Verify();
            orderRepository.Verify();
        }

        public static IEnumerable<object[]> OrderItemsData =>
            [
                            [new OrderItem[] { new() { Quantity = 1, UnitPrice = 30 }, new() { Quantity = 2, UnitPrice = 20 } }, 70m],
                            [new OrderItem[] { new() { Quantity = 3, UnitPrice = 15 }, new() { Quantity = 4, UnitPrice = 10 }, new() { Quantity = 2, UnitPrice = 5 } }, 95m],
                            [new OrderItem[] { new() { Quantity = 5, UnitPrice = 12 } }, 60m]
            ];
            

        [Theory]
        [MemberData(nameof(OrderItemsData))]
        public void ProcessOrder_CorrectTotalPaymentAmount(IEnumerable<OrderItem> items, decimal expectedTotal)
        {
            //Arrange
            var inventoryService = new Mock<IInventoryService>();
            var orderRepository = new Mock<IOrderRepository>();
            var paymentService = new Mock<IPaymentService>();

            var orderProcessor = new OrderProcessor(orderRepository.Object, inventoryService.Object, paymentService.Object);

            var fixture = new Fixture();
            var order = fixture.Build<Order>().With(x => x.Items, [..items]).Create();
            var cardNumber = fixture.Create<string>();

            inventoryService.Setup(x => x.CheckStock(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

            //Act
            _ = orderProcessor.ProcessOrder(order, cardNumber);

            //Assert
            paymentService.Verify(x => x.ProcessPayment(cardNumber, expectedTotal), Times.Once);
        }
    }
}
