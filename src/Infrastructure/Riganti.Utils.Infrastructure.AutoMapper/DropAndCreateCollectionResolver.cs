using System;
using System.Collections;
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


        public DropAndCreateCollectionResolver(Func<TSourceItem, TDestinationItem> projection = null, Action<TDestinationItem> removeCallback = null)
        {
            this.Projection = projection ?? Mapper.Map<TSourceItem, TDestinationItem>;
            this.RemoveCallback = removeCallback ?? (_ => {});
        }


        public ICollection<TDestinationItem> Resolve(TSource source, TDestination destination, ICollection<TSourceItem> sourceMember, ICollection<TDestinationItem> destMember, ResolutionContext context)
        {
            foreach (var item in new List<TDestinationItem>(destMember))
            {
                RemoveCallback(item);
            }
            destMember.Clear();

            foreach (var item in sourceMember)
            {
                var destItem = Projection(item);
                destMember.Add(destItem);
            }

            return destMember;
        }
    }
}