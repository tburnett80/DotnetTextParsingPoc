using System;

namespace ParsingLib
{
    /// <summary>
    /// Result wrapper to maintain exception and result outcome across layers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ResultWrapper<T> where T: class
    {
        public bool IsFailure { get; set; }

        public T Data { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }
    }
}
