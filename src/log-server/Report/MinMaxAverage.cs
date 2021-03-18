namespace LogServer.Report
{
    public class MinMaxAverage
    {
        private readonly object _syncRoot = new();

        private int? _min;
        private int? _max;
        private int _count;
        private int _sum;

        public int Min
        {
            get
            {
                lock (_syncRoot)
                {
                    return _min ?? 0;
                }
            }
        }

        public int Max
        {
            get
            {
                lock (_syncRoot)
                {
                    return _max ?? 0;
                }
            }
        }

        public double Average
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_count == 0)
                    {
                        return 0;
                    }

                    return (double)_sum / _count;
                }
            }
        }

        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    return _count;
                }
            }
        }

        public void Update(int newValue)
        {
            lock (_syncRoot)
            {
                if (_min == null || newValue < _min)
                {
                    _min = newValue;
                }

                if (_max == null || newValue > _max)
                {
                    _max = newValue;
                }

                _count++;
                _sum += newValue;
            }
        }
    }
}
