using System;

namespace Phase.Mediators
{
    public class CommandCancelledException : Exception
    {
        public CommandCancelledException(string details) : base(details)
        {
        }
    }
}