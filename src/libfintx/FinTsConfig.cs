namespace libfintx
{
    public static class FinTsConfig
    {
        /// <summary>	
        /// Enable / Disable Tracing	
        /// </summary>	
        public static void Tracing(bool Enabled, bool Formatted = false, int maxFileSizeMB = 10)
        {
            Trace.Enabled = Enabled;
            Trace.Formatted = Formatted;
            Trace.MaxFileSize = maxFileSizeMB;
        }

        /// <summary>	
        /// Enable / Disable Debugging	
        /// </summary>	
        public static void Debugging(bool Enabled)
        {
            DEBUG.Enabled = Enabled;
        }

        /// <summary>	
        /// Enable / Disable Logging	
        /// </summary>	
        public static void Logging(bool Enabled, int maxFileSizeMB = 10)
        {
            Log.Enabled = Enabled;
            Log.MaxFileSize = maxFileSizeMB;
        }

        public static string Buildname { get; set; } = "libfintx";

        public static string Version { get; set; } = "0.0.1";

        /// <summary>
        /// Produktregistrierungsnummer. Replace it with you own id if available.
        /// </summary>
        public static string ProductId = "9FA6681DEC0CF3046BFC2F8A6";
    }
}
