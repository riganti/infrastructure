namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// The condition to match against source objects.
    /// </summary>
    public class FilterConditionDTO : FilterDTOBase
    {
        /// <summary>
        /// Gets or sets name of the field.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets name of the field displayed to users.
        /// </summary>
        public string FieldDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the filter operator to apply.
        /// </summary>
        public FilterOperatorType? Operator { get; set; }

        /// <summary>
        /// Gets or sets format string used to display the filter value.
        /// </summary>
        public string FormatString { get; set; }

        /// <summary>
        /// Gets or sets the value to match.
        /// </summary>
        public object Value { get; set; }

        /// <inheritdoc />
        public override bool IsValid()
        {
            if (Operator == null)
            {
                return false;
            }

            if (Value == null)
            {
                return Operator == FilterOperatorType.True ||
                    Operator == FilterOperatorType.False ||
                    Operator == FilterOperatorType.Null ||
                    Operator == FilterOperatorType.NotNull;
            }

            return true;
        }
    }
}