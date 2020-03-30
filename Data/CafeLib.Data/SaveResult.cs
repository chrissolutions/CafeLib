using System;

namespace CafeLib.Data
{
    /// <summary>
    /// SaveResult structure.
    /// </summary>
    public struct SaveResult<T>
    {
        public T Id { get; set; }

        public bool Inserted { get; set; }

        public bool Updated => !Inserted;

        public Exception? Exception { get; set; }

        public SaveResult(T id, bool inserted = false)
        {
            Id = id;
            Inserted = inserted;
            Exception = null;
        }

        public SaveResult(Exception ex)
            : this(DefaultSqlId())
        {
            Exception = ex;
        }

        /// <summary>
        /// Check Sql Id type for requiring quotation.
        /// </summary>
        /// <typeparam name="T">identifier type</typeparam>
        /// <returns>return quoted id for certain types</returns>
        private static T DefaultSqlId()
        {
            switch (typeof(T).Name)
            {
                case "String":
                    return (T)(object)"";

                case "Guid":
                    return (T)(object)Guid.Empty;

                case "Int32":
                case "Int64":
                    return (T)(object)0;

                default:
                    throw new NotSupportedException(typeof(T).Name);
            }
        }
    }
}