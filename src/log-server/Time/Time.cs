using System;

namespace LogServer.Time
{
    public class Clock
    {
        public virtual DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}
