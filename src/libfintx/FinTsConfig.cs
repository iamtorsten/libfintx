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
    }
}