using System;

namespace QuickAccess.Parser
{
    [Flags]
    public enum ParsingOptions
    {
        None = 0,
        Cache = 0x01,
        PrintSteps = 0x02,
    }
}