using System;
using System.Collections.Generic;
using System.Text;

namespace fitnes3.Services
{
    public interface IDispatcherTimerService
    {
        void StartTimer(TimeSpan interval, Func<bool> callback);
    }
}
