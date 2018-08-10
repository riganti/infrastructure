namespace Riganti.Utils.Infrastructure.EntityFrameworkCore
{
    /// <summary>
    /// Prescription of this interface is used for children commit requests. When an unsuccessful
    /// attempt is made, the calling unit of work is using parents resources and it's commit is
    /// therefore skipped. Unit of work provider implementing this interface is responsible for
    /// tracking of such attempts and provide information whether commit has been requested by any
    /// child unit of works.
    /// </summary>
    public interface ICheckChildCommitUnitOfWorkProvider
    {
        /// <summary>
        /// Flag used for tracking of children commit calls.
        /// </summary>
        bool CommitRequested { get; }

        /// <summary>
        /// Called by unit of work. Used for simple commit calls tracking. In case of reused context,
        /// the commit gets skipped and <see cref="CommitRequested" /> flag is set to true. When
        /// called by root unit of work, this flag gets reset.
        /// </summary>
        /// <param name="commitAttemptSuccess"> 
        /// Indicates whether the called commit went through or got skipped due to being child's commit.
        /// </param>
        void CommitAttempt(bool commitAttemptSuccess);
    }
}