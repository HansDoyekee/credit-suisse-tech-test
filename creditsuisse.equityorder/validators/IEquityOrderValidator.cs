namespace creditsuisse.equityorder.validators
{
    public interface IEquityOrderValidator
    {
        bool IsValid(string equityCode, decimal price);
    }
}
