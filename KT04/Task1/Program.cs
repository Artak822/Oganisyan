using System;
using System.IO;

class LogWriter : IDisposable
{   
    private readonly ILog _log;
    private readonly ILogWriter _writer;
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public LogWriter(ILog log,  ILogWriter writer)
    {
        _log = log;
        _writer = writer;
    }
    //хз крч я че то затупил, похоже на бред(((
}
