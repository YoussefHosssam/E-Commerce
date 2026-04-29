namespace E_Commerce.Domain.Enums
{
    public enum StockMovementType
    {
        InitialStock = 1,
        StockIn = 2,
        StockOut = 3,
        Sale = 4,
        Return = 5,
        Damage = 6,
        Adjustment = 7,
        Reservation = 8,
        ReservationReleased = 9
    }
}