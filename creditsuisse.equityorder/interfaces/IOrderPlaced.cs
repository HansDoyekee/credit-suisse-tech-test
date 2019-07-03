using creditsuisse.equityorder.handlers;

namespace creditsuisse.equityorder.interfaces
{
    public interface IOrderPlaced
    {
        event OrderPlacedEventHandler OrderPlaced;
    }
}