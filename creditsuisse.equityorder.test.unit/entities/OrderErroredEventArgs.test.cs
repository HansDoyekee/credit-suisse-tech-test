using System;
using Xunit;

namespace creditsuisse.equityorder.test.unit.entities
{
    public class OrderErroredEventArgs
    {
        #region [ EquityCode ]
        [Fact]
        public void Property_EquityCode_Returns_Correct_Value()
        {
            var sut = new equityorder.entities.OrderErroredEventArgs("some equity code", decimal.MaxValue, new Exception());

            Assert.Equal("some equity code", sut.EquityCode);
        }

        [Fact]
        public void Property_EquityCode_Is_Not_Settable()
        {
            var equityCodeProperty = typeof(equityorder.entities.OrderErroredEventArgs).GetProperty("EquityCode");
            if (equityCodeProperty == null)
            {
                throw new Exception("Property EquityCode does not exist in class OrderErroredEventArgs");
            }
            var setter = equityCodeProperty.GetSetMethod(true);

            Assert.Null(setter);
        }
        #endregion

        #region [ Price ]
        [Fact]
        public void Property_Price_Returns_Correct_Value()
        {
            var sut = new equityorder.entities.OrderErroredEventArgs(null, 2.256m, new Exception());

            Assert.Equal(2.256m, sut.Price);
        }

        [Fact]
        public void Property_Price_Is_Not_Settable()
        {
            var priceProperty = typeof(equityorder.entities.OrderErroredEventArgs).GetProperty("Price");
            if (priceProperty == null)
            {
                throw new Exception("Property Price does not exist in class OrderErroredEventArgs");
            }
            var setter = priceProperty.GetSetMethod(true);

            Assert.Null(setter);
        }
        #endregion
    }
}
