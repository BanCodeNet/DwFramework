using System;

namespace DwFramework.Core
{
    public sealed class NotFoundException : BaseException
    {
        public string FoundThings { get; init; }

        public NotFoundException(string foundThings) : base(401, $"无法找到\"{foundThings}\"")
        {
            FoundThings = foundThings;
        }
    }
}