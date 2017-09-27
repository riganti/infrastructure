using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Riganti.Utils.Infrastructure.Logging
{
    public class DefaultExceptionFormatter : IExceptionFormatter
    {
        public bool DisplayData { get; set; } = true;
        public bool DisplayReflectedProperties { get; set; } = true;
        public bool DisplayStackTrace { get; set; } = true;
        public bool DisplayType { get; set; } = true;

        public List<IExceptionAdapter> ExceptionAdapters { get; } = new List<IExceptionAdapter>
        {
            new DbEntityValidationExceptionAdapter()
        };

        public List<string> IgnoredReflectionProperties { get; } = new List<string>
        {
            nameof(Exception.Source),
            nameof(Exception.Message),
            nameof(Exception.HelpLink),
            nameof(Exception.InnerException),
            nameof(Exception.StackTrace),
            nameof(Exception.Data),
            nameof(Exception.HResult),
            nameof(Exception.HelpLink),
            "Exception.TargetSite"
        };

        public string FormatException(Exception ex)
        {
            var stringBuilder = new StringBuilder();
            FormatException(ex, stringBuilder);

            ex = ex.InnerException;
            while (ex != null)
            {
                stringBuilder.AppendLine("---------- Inner exception ----------");
                FormatException(ex, stringBuilder, true);
                ex = ex.InnerException;
            }

            return stringBuilder.ToString();
        }

        protected virtual void AppendReflectedProperties(StringBuilder sb, Exception exception)
        {
            var type = exception.GetType();
            var properties = type.GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var fields = type.GetTypeInfo().GetFields(BindingFlags.Instance | BindingFlags.Public);

            var ignoredReflectionProperties = IgnoredReflectionProperties;
            foreach (var adapter in ExceptionAdapters)
            {
                adapter.ModifyIgnoredReflectionProperties(exception, ignoredReflectionProperties);
            }

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.CanRead && !ignoredReflectionProperties.Contains(propertyInfo.Name))
                {
                    if (propertyInfo.GetIndexParameters().Length == 0)
                    {
                        object propertyValue;
                        try
                        {
                            propertyValue = propertyInfo.GetValue(exception, null);
                        }
                        catch (TargetInvocationException)
                        {
                            propertyValue = "(access to property failed)";
                        }

                        sb.Append(propertyInfo.Name);
                        sb.Append(": ");
                        sb.Append(propertyValue);
                        sb.AppendLine();
                    }
                }
            }
            foreach (FieldInfo fieldInfo in fields)
            {
                object fieldValue;
                try
                {
                    fieldValue = fieldInfo.GetValue(exception);
                }
                catch (TargetInvocationException)
                {
                    fieldValue = "(access to field failed)";
                }

                sb.Append(fieldInfo.Name);
                sb.Append(": ");
                sb.Append(fieldValue);
                sb.AppendLine();
            }
        }

        protected virtual void FormatException(Exception ex, StringBuilder sb, bool isInnerEx = false)
        {
            if (isInnerEx)
                sb.Append("Message: ");

            sb.AppendLine(ex.Message);

            if (DisplayType)
            {
                sb.Append("Type: ");
                sb.AppendLine(ex.GetType().AssemblyQualifiedName);
            }

            if (DisplayData && ex.Data.Count > 0)
            {
                sb.Append("Data: ");
                AppendFormatedData(sb, ex.Data);
            }

            if (DisplayReflectedProperties)
            {
                AppendReflectedProperties(sb, ex);
            }

            foreach (var adapter in ExceptionAdapters)
            {
                adapter.AppendFormatedDetails(ex, sb);
            }

            if (DisplayStackTrace)
            {
                sb.Append("StackTrace: ");
                sb.AppendLine(ex.StackTrace?.TrimEnd());
            }
        }

        private static void AppendFormatedData(StringBuilder sb, IDictionary data)
        {
            var isFirst = true;

            foreach (DictionaryEntry entry in data)
            {
                if (!isFirst)
                    sb.Append("; ");

                sb.Append(entry.Key);
                sb.Append("=");
                sb.Append(entry.Value);
                isFirst = false;
            }

            sb.AppendLine();
        }
    }
}