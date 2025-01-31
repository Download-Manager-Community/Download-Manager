namespace DownloadManager.Components.Extentions
{
    public static class ExceptionExt
    {
        /// <summary>
        /// Get all inner exceptions of an exception.
        /// </summary>
        /// <param name="ex">Any exception.</param>
        /// <returns>An array of Exceptions contained in the InnerExceptions property of the specified Exception. If there are no inner exceptions, returns null.</returns>
        public static Exception[]? GetInnerExceptions(this Exception ex)
        {
            if (ex == null)
            {
                // If the exception object is null, return null (as there wont be any InnerException)
                return null;
            }

            // Declare variables
            Exception currentException = ex;
            List<Exception> exceptions = new List<Exception>();
            Exception[]? exceptionsArray = null;

            while (currentException != null)
            {
                // Set the current exception to the next inner exception of the current exception
                currentException = currentException.InnerException;
                if (currentException != null)
                {
                    // While there are inner exceptions and they are not null, add them to the list
                    exceptions.Add(currentException);
                }
            }

            // If there are inner exceptions, convert the list to an array
            if (exceptions.Count > 0)
                exceptionsArray = exceptions.ToArray();

            // Return the array of inner exceptions
            // If there are no inner exceptions, the array will be null
            return exceptionsArray;
        }
    }
}
