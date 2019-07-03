using System;
using Xunit;
using Moq;
using creditsuisse.equityorder.configuration;
using creditsuisse.equityorder.entities;
using creditsuisse.equityorder.services;
using creditsuisse.equityorder.validators;

namespace creditsuisse.equityorder.test.unit
{
    public class EquityOrder
    {
        private Mock<IConfig> configMock;
        private Mock<IOrderService> orderServiceMock;
        private Mock<IEquityOrderValidator> validatorMock;

        private equityorder.EquityOrder _sut;

        public EquityOrder()
        {
            configMock = new Mock<IConfig>();
            orderServiceMock = new Mock<IOrderService>();
            validatorMock = new Mock<IEquityOrderValidator>();

            configMock.Setup(i => i.EquityOrderQuantity).Returns(1);
            configMock.Setup(i => i.EquityOrderThreshold).Returns(10m);


            _sut = new equityorder.EquityOrder(orderServiceMock.Object, configMock.Object, validatorMock.Object);
        }

        #region [ ReceiveTick ]

        [Fact]
        public void When_valid_request_and_price_less_than_threshold_Then_buy_and_raise_orderPlaced_event()
        {
            //  Arrange
            validatorMock.Setup(i => i.IsValid(It.IsAny<string>(), It.IsAny<decimal>())).Returns(true);
            var price = 5m;
            var wasOrderPlacedEventRaised = false;
            OrderPlacedEventArgs eventArgs = null;
            _sut.OrderPlaced += data => { wasOrderPlacedEventRaised = true;
                eventArgs = data;
            };

            //  Act
            _sut.ReceiveTick("some equity code", price);
            
            //  Assert
            orderServiceMock.Verify(o => o.Buy("some equity code", 1, 5m), Times.Once);
            Assert.True(wasOrderPlacedEventRaised);
            Assert.NotNull(eventArgs);
            Assert.Equal("some equity code", eventArgs.EquityCode);
            Assert.Equal(price, eventArgs.Price);
        }

        [Fact]
        public void When_invalid_request_and_price_less_than_threshold_Then_raise_OrderErrored_event()
        {
            //  Arrange
            validatorMock.Setup(i => i.IsValid(It.IsAny<string>(), It.IsAny<decimal>())).Returns(false);
            var price = 5.00m;
            OrderErroredEventArgs eventArgs = null;
            var wasOrderErroredEventRaised = false;
            _sut.OrderErrored += data => { wasOrderErroredEventRaised = true;
                eventArgs = data;
            };

            //  Act
            _sut.ReceiveTick("some equity code", price);

            //  Assert
            orderServiceMock.Verify(o => o.Buy("some equity code", 1, 5m), Times.Never);
            Assert.True(wasOrderErroredEventRaised);

            //  Assert event arguments properties
            Assert.NotNull(eventArgs);
            Assert.Equal("some equity code", eventArgs.EquityCode);
            Assert.Equal(price, eventArgs.Price);
            Assert.NotNull(eventArgs.GetException());
            Assert.Equal("Equity code or price invalid; EquityCode: some equity code, Price: 5.00", eventArgs.GetException().Message);
        }

        [Fact]
        public void When_valid_request_and_price_more_than_threshold_Then_do_not_raise_any_events()
        {
            //  Arrange
            validatorMock.Setup(i => i.IsValid(It.IsAny<string>(), It.IsAny<decimal>())).Returns(true);
            var price = 25.00m;
            var wasOrderErroredEventRaised = false;
            var wasOrderPlacedEventRaised = false;
            _sut.OrderErrored += data => {
                wasOrderErroredEventRaised = true;
            };
            _sut.OrderPlaced += data => {
                wasOrderPlacedEventRaised = true;
            };

            //  Act
            _sut.ReceiveTick("some equity code", price);

            //  Assert
            orderServiceMock.Verify(o => o.Buy("some equity code", 1, 5m), Times.Never);
            Assert.False(wasOrderErroredEventRaised);
            Assert.False(wasOrderPlacedEventRaised);
        }

        [Fact]
        public void When_exception_occurs_Then_raise_OrderErrored_event()
        {
            //  Arrange
            validatorMock.Setup(i => i.IsValid(It.IsAny<string>(), It.IsAny<decimal>())).Throws(new Exception("some exception occurred"));
            var price = 5m;
            OrderErroredEventArgs eventArgs = null;
            var wasOrderErroredEventRaised = false;
            _sut.OrderErrored += data => {
                wasOrderErroredEventRaised = true;
                eventArgs = data;
            };

            //  Act
            _sut.ReceiveTick("some equity code", price);

            //  Assert
            orderServiceMock.Verify(o => o.Buy("some equity code", 1, 5m), Times.Never);
            Assert.True(wasOrderErroredEventRaised);

            //  Assert event arguments properties
            Assert.NotNull(eventArgs);
            Assert.Equal("some equity code", eventArgs.EquityCode);
            Assert.Equal(price, eventArgs.Price);
            Assert.NotNull(eventArgs.GetException());
            Assert.Equal("some exception occurred", eventArgs.GetException().Message);
        }

        #endregion

    }
}
