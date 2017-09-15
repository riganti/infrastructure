using System;
using System.Text;

namespace Riganti.Utils.Infrastructure.Logging
{
    public class DefaultExceptionFormatter : IExceptionFormatter
    {
        public virtual string FormatException(Exception ex)
        {
            var stringBuilder = new StringBuilder();
            FormatException(ex, stringBuilder);

            ex = ex.InnerException;
            while (ex != null)
            {
                stringBuilder.AppendLine("---------- End of inner exception stack trace ----------");
                FormatException(ex, stringBuilder);
                ex = ex.InnerException;
            }

            return stringBuilder.ToString();
        }

        private static void FormatException(Exception ex, StringBuilder stringBuilder)
        {
            stringBuilder.Append(ex.GetType().FullName);
            stringBuilder.Append(": ");
            stringBuilder.AppendLine(ex.Message);
            stringBuilder.AppendLine(ex.StackTrace);

            if (ex.GetType().FullName == "System.Data.Entity.Validation.DbEntityValidationException")
            {
                FormatDbEntityValidationException(ex, stringBuilder);
            }
            stringBuilder.AppendLine();
        }

        private static void FormatDbEntityValidationException(Exception ex, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("EntityValidationErrors: ");
            dynamic e = ex;
            foreach (var error in e.EntityValidationErrors)
            {
                stringBuilder.AppendLine("  Entity: " + error.Entry?.Entity?.GetType().FullName);

                foreach (var message in error.ValidationErrors)
                {
                    stringBuilder.Append("      entity.");
                    stringBuilder.Append(message.PropertyName);
                    stringBuilder.Append(": ");
                    stringBuilder.AppendLine(message.ErrorMessage);
                }
                stringBuilder.AppendLine();
            }
        }
    }
}