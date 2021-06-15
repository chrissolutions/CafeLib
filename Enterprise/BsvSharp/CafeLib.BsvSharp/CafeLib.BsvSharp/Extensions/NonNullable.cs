using System;
using System.Diagnostics;

namespace CafeLib.BsvSharp.Extensions
{
    // ReSharper disable once UseNameofExpression
    [DebuggerDisplay("Value = {Value}")]
    public struct NonNullable<T> : IEquatable<T>, IEquatable<NonNullable<T>> where T : class
    {
        /// <summary>
        /// NonNullable value.
        /// </summary>
        public bool HasValue => Value != null;

        /// <summary>
        /// NonNullable value.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// NonNullable constructor.
        /// </summary>
        /// <param name="value"></param>
        public NonNullable(T value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Determimes whether the values are equal.
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>true if equals otherwise false.</returns>
        public override bool Equals(object obj)
        {
            while (true)
            {
                switch (obj)
                {
                    case null:
                        return false;

                    case T x:
                        return Value.Equals(x);

                    case NonNullable<T> x:
                        obj = x;
                        break;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Determimes whether the values are equal.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(T other) => Value.Equals(other);

        /// <summary>
        /// Determimes whether the values are equal.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(NonNullable<T> other) => Equals(other.Value);

        /// <summary>
        /// Gets the hashcode.
        /// </summary>
        /// <returns>hashcode</returns>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Gets string representation.
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString() => Value.ToString();

        /// <summary>
        /// Implicit cast a value of type T to non-nullable wrapper.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator NonNullable<T>(T value) => new NonNullable<T>(value);

        /// <summary>
        /// Implicit cast non-nullable to value of type T.
        /// </summary>
        /// <param name="wrapper">non-nullable wrapper</param>
        public static implicit operator T(NonNullable<T> wrapper) => wrapper.Value;

        public static bool operator ==(NonNullable<T> a, NonNullable<T> b) => a.Equals(b);
        public static bool operator !=(NonNullable<T> a, NonNullable<T> b) => !(a == b);
        public static bool operator ==(NonNullable<T> a, T b) => a.Equals(b);
        public static bool operator !=(NonNullable<T> a, T b) => !(a == b);
        public static bool operator ==(T a, NonNullable<T> b) => b.Equals(a);
        public static bool operator !=(T a, NonNullable<T> b) => !(a == b);
    }
}