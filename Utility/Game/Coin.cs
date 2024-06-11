public class Coin
{
    private SecureInt coins;

    public Coin(int value)
    {
        coins = new SecureInt(value);
    }

    public int GetCoins()
    {
        return coins.GetValue();
    }

    public void AddCoins(int value)
    {
        coins += new SecureInt(value);
    }

    public void RemoveCoins(int value)
    {
        coins -= new SecureInt(value);
    }

    public void SetCoins(int value)
    {
        coins = new SecureInt(value);
    }

    private class SecureInt
    {
        private int offset;
        private int value;

        public SecureInt(int value)
        {
            this.offset = UnityEngine.Random.Range(34, 5678);
            this.value = value + offset;
        }

        public int GetValue()
        {
            return value - offset;
        }

        public static SecureInt operator +(SecureInt a, SecureInt b)
        {
            return new SecureInt(a.GetValue() + b.GetValue());
        }

        public static SecureInt operator -(SecureInt a, SecureInt b)
        {
            return new SecureInt(a.GetValue() - b.GetValue());
        }
    }
}
