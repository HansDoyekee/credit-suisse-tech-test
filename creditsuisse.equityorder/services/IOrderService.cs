﻿namespace creditsuisse.equityorder.services
{
    public interface IOrderService
    {
        void Buy(string equityCode, int quantity, decimal price);
        void Sell(string equityCode, int quantity, decimal price);
    }
}