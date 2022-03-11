using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace Riganti.Utils.Infrastructure.AutoMapper
{
    public class DropAndCreateCollectionResolver<TSource, TSourceItem, TDestination, TDestinationItem>
        : IMemberValueResolver<TSource, TDestination, ICollection<TSourceItem>, ICollection<TDestinationItem>>
    {
        public Action<TDestinationItem> RemoveCallback { get; }

        public Func<TSourceItem, TDestinationItem> Projection { get; }

        public Func<TDestinationItem, bool> DestinationFilter { get; }

        public DropAndCreateCollectionResolver(Func<TSourceItem, TDestinationItem> projection = null,
                                               Action<TDestinationItem> removeCallback = null,
                                               Func<TDestinationItem, bool> destinationFilter = null)
        {
            this.Projection = projection;
            this.RemoveCallback = removeCallback;
            this.DestinationFilter = destinationFilter;
        }

        public ICollection<TDestinationItem> Resolve(TSource source,
                                                     TDestination destination,
                                                     ICollection<TSourceItem> sourceMember,
                                                     ICollection<TDestinationItem> destMember,
                                                     ResolutionContext context)
        {
            var projection = Projection ?? context.Mapper.Map<TSourceItem, TDestinationItem>;
            var removeCallback = RemoveCallback ?? (_ => { });
            var destinationFilter = DestinationFilter ?? (_ => true);

            foreach (var item in new List<TDestinationItem>(destMember.Where(destinationFilter)))
            {
                removeCallback(item);
                destMember.Remove(item);
            }

            foreach (var item in sourceMember)
            {
                var destItem = projection(item);
                destMember.Add(destItem);
            }

            return new List<TDestinationItem>(destMember);
        }
    }
}