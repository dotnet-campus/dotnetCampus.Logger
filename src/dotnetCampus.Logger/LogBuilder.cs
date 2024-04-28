using System.Collections.Generic;

namespace dotnetCampus.Logging;

public class LogBuilder
{
    private List<ILogger> _writers = [];

    public CompositeLogger Build()
    {
        var logger = new CompositeLogger();
        return logger;
    }
}
