namespace CLogger
{
    public enum LogLevels
    {
        // Summary:
        //     Everything you want to know about a running block of code.
        Verbose = 0,
        //
        // Summary:
        //     Internal system events that aren't necessarily observable from the outside.
        Debug = 1,
        //
        // Summary:
        //      Informational messages that highlight the progress of the application at coarse-grained level.
        Information = 2,
        //
        // Summary:
        //      Potentially harmful situations.
        Warning = 3,
        //
        // Summary:
        //     Error events that might still allow the application to continue running, invariants are broken or data is lost.
        Error = 4,
        //
        // Summary:
        //   Very severe error events that will presumably lead the application to abort.
        Fatal = 5
    }
}