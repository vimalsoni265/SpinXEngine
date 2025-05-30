namespace SpinXEngine.TestWinLogic.App
{
    public class Player(double balance)
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public double Balance { get; private set; } = balance > 0 ? balance : 0;

        public void Deduct(double amount)
        {
            amount = Math.Ceiling(amount * 100) / 100; // Round to two decimal places
            if (Balance < amount)
                throw new InvalidOperationException("Insufficient balance");

            Balance -= amount;
        }

        public void Credit(double amount)
        {
            amount = Math.Ceiling(amount * 100) / 100; // Round to two decimal places
            Balance += amount;
        }

        public override string ToString()
        {
            return $"Player ID: {Id}, Balance: ${Balance:F2}";
        }
    }
}
