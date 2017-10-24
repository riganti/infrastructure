using System.Collections.Generic;
using System.Linq;

namespace Riganti.Utils.Infrastructure.Core
{
    /// <summary>
    /// The group containing nested filters and groups.
    /// </summary>
    public class FilterGroupDTO : FilterDTOBase
    {
        /// <summary>
        /// Gets or sets the logic applied on filters contained.
        /// </summary>
        public FilterLogicType Logic { get; set; } = FilterLogicType.And;

        /// <summary>
        /// Gets or sets the list of nested filters and groups.
        /// </summary>
        public IList<FilterDTOBase> Filters { get; set; } = new List<FilterDTOBase>(0);

        /// <inheritdoc />
        public override bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Returns a list of valid filters in this group.
        /// </summary>
        public IList<FilterDTOBase> GetValidFilters()
        {
            if (Filters == null)
            {
                return new List<FilterDTOBase>(0);
            }

            return Filters
                .Where(x => x.IsValid())
                .ToList();
        }

        /// <summary>
        /// Returns whether this group contains any valid filters.
        /// </summary>
        public bool IsEmpty()
        {
            if (Filters == null)
            {
                return true;
            }

            foreach (var filter in GetValidFilters())
            {
                if (filter is FilterConditionDTO)
                {
                    return false;
                }

                if (filter is FilterGroupDTO group && !group.IsEmpty())
                {
                    return false;
                }
            }

            return true;
        }
    }
}