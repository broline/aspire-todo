using System;

namespace Todo.Common
{
    public interface IClock
    {
        DateTimeOffset UtcNow { get; }
    }

    public class SystemClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }

    public class FrozenClock : IClock
    {
        private DateTimeOffset _now = DateTimeOffset.UtcNow;

        public void SetUtcNow(DateTimeOffset now)
        {
            _now = now;
        }

        public DateTimeOffset UtcNow => _now;

        public DateTimeOffset AdvanceSeconds(int seconds)
        {
            return AdvanceTime(TimeSpan.FromSeconds(seconds));
        }

        public DateTimeOffset AdvanceMinutes(int minutes)
        {
            return AdvanceTime(TimeSpan.FromMinutes(minutes));
        }

        public DateTimeOffset AdvanceHours(int hours)
        {
            return AdvanceTime(TimeSpan.FromHours(hours));
        }

        public DateTimeOffset AdvanceDays(int days)
        {
            return AdvanceTime(TimeSpan.FromDays(days));
        }

        public DateTimeOffset AdvanceTime(TimeSpan span)
        {
            this.SetUtcNow(_now.Add(span));
            return this._now;
        }
    }
}
