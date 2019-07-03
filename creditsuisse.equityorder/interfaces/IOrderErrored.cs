using creditsuisse.equityorder.handlers;

namespace creditsuisse.equityorder.interfaces
{
    public interface IOrderErrored
    {
        event OrderErroredEventHandler OrderErrored;
    }
}