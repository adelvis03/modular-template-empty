using System.Collections.Generic;

namespace TemplateName.Shared.Infrastructure.Modules;

public record ModuleInfo(string Name, IEnumerable<string> Policies);

