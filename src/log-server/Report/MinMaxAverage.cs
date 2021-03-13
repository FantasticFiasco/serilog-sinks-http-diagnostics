namespace LogServer.Report
{
    public class MinMaxAverage
    {
        private readonly object syncRoot = new();

        private int? min;
        private int? max;
        private int count;
        private int sum;

        public int Min
        {
            get
            {
                lock (syncRoot)
                {
                    return min ?? 0;
                }
            }
        }

        public int Max
        {
            get
            {
                lock (syncRoot)
                {
                    return max ?? 0;
                }
            }
        }

        public double Average
        {
            get
            {
                lock (syncRoot)
                {
                    if (count == 0)
                    {
                        return 0;
                    }

                    return (double)sum / count;
                }
            }
        }

        public int Count
        {
            get
            {
                lock (syncRoot)
                {
                    return count;
                }
            }
        }

        public void Update(int newValue)
        {
            lock (syncRoot)
            {
                if (min == null || newValue < min)
                {
                    min = newValue;
                }

                if (max == null || newValue > max)
                {
                    max = newValue;
                }

                count++;
                sum += newValue;
            }
        }
    }
}
