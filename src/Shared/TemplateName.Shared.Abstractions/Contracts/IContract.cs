using System;
using System.Collections.Generic;

namespace TemplateName.Shared.Abstractions.Contracts;

public interface IContract
{
    Type Type { get; }
    public IEnumerable<string> Required { get; }
}



