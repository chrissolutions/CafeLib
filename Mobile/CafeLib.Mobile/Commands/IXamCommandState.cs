namespace CafeLib.Mobile.Commands
{
    public interface IXamCommandState
    {
        /// <summary>
        /// Locks the command to prevent execution.
        /// </summary>
        void Lock();

        /// <summary>
        /// Unlocks the command to permit execution.
        /// </summary>
        void Unlock();
    }
}
