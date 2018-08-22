using System;

namespace Riganti.Utils.Infrastructure.EntityFramework
{
    public class ChildCommitPendingException : Exception
    {
        public ChildCommitPendingException()
            : base("Some of the unit of works requested commit! Ensure that commit is called on root unit of work as well.")
        {
        }
    }
}