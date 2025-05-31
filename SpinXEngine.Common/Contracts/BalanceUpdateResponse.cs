namespace SpinXEngine.Common.Contracts
{
    public record BalanceUpdateResponse
    {
        public decimal NewBalance { get; init; } = default!;
    }
}
