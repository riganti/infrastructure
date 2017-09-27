using System;
using System.Collections.Generic;
using System.Text;

namespace Riganti.Utils.Infrastructure.Logging
{
    /// <summary>
    /// Exception adapter for System.Data.Entity.Validation.DbEntityValidationException.
    /// </summary>
    public class DbEntityValidationExceptionAdapter : IExceptionAdapter
    {
        public void AppendFormatedDetails(Exception exception, StringBuilder sb)
        {
            if (!ShouldApply(exception))
                return;

            sb.AppendLine("EntityValidationErrors: ");
            dynamic e = exception;
            foreach (var error in e.EntityValidationErrors)
            {
                sb.AppendLine("  Entity: " + error.Entry?.Entity?.GetType().FullName);

                foreach (var message in error.ValidationErrors)
                {
                    sb.Append("      entity.");
                    sb.Append(message.PropertyName);
                    sb.Append(": ");
                    sb.AppendLine(message.ErrorMessage);
                }
            }
        }

        public void ModifyIgnoredReflectionProperties(Exception exception, List<string> collection)
        {
            if (ShouldApply(exception))
            {
                collection.Add("EntityValidationErrors");
            }
        }

        private static bool ShouldApply(Exception exception)
        {
            return exception.GetType().FullName == "System.Data.Entity.Validation.DbEntityValidationException";
        }
    }
}