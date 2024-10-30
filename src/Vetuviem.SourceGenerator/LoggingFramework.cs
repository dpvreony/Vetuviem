namespace Vetuviem.SourceGenerator
{
    /// <summary>
    /// Represents the logging framework csproj setting,
    /// </summary>
    public enum LoggingFramework
    {
        /// <summary>
        /// No logging framework to be generated.
        /// </summary>
        None,

        /// <summary>
        /// Generate integration for Splat.
        /// </summary>
        Splat,

        /// <summary>
        /// Generate Microsoft Extensions Logging.
        /// </summary>
        MicrosoftExtensionsLogging
    }
}

