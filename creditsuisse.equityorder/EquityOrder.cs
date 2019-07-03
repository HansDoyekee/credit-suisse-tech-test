using System;
using System.Runtime.InteropServices;
using creditsuisse.equityorder.configuration;
using creditsuisse.equityorder.entities;
using creditsuisse.equityorder.handlers;
using creditsuisse.equityorder.interfaces;
using creditsuisse.equityorder.services;
using creditsuisse.equityorder.validators;
using Microsoft.Win32.SafeHandles;

namespace creditsuisse.equityorder
{
    public class EquityOrder : IEquityOrder, IDisposable
    {
        private readonly IOrderService _orderService;
        private readonly IConfig _config;
        private readonly IEquityOrderValidator _validator;
        private bool _disposed;
        private readonly SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);

        public event OrderPlacedEventHandler OrderPlaced;
        public event OrderErroredEventHandler OrderErrored;

        
        public EquityOrder(IOrderService orderService, IConfig config, IEquityOrderValidator validator)
        {
            _orderService = orderService;
            _config = config;
            _validator = validator;
        }

        public void ReceiveTick(string equityCode, decimal price)
        {
            using (this)
            {
                try
                {
                    if (_validator.IsValid(equityCode, price))
                    {
                        if (price < _config.EquityOrderThreshold)
                        {
                            _orderService.Buy(equityCode, _config.EquityOrderQuantity, price);
                            OrderPlaced?.Invoke(new OrderPlacedEventArgs(equityCode, price));
                        }
                    }
                    else
                    {
                        OrderErrored?.Invoke(new OrderErroredEventArgs(equityCode, price, 
                            new ArgumentException($"Equity code or price invalid; EquityCode: {equityCode}, Price: {price}")));
                    }
                }
                catch (Exception exception)
                {
                    OrderErrored?.Invoke(new OrderErroredEventArgs(equityCode, price, exception));
                }
            }
        }

        #region [ IDisposable implementation ]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _handle.Dispose();
            }

            _disposed = true;
        }
        #endregion
    }
}