namespace creditsuisse.equityorder.configuration
{
    public interface IConfig
    {
        decimal EquityOrderThreshold { get; set; }
        int EquityOrderQuantity { get; set; }
    }
}
