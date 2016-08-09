using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace Riganti.Utils.Infrastructure.AutoMapper
{
    public class SyncByKeyCollectionResolver<TSource, TSourceItem, TDestination, TDestinationItem, TKey>
        : IMemberValueResolver<TSource, TDestination, ICollection<TSourceItem>, ICollection<TDestinationItem>>

    {

        public Func<TSourceItem, TKey> SourceKeySelector { get; set; }

        public Func<TDestinationItem, TKey> DestinationKeySelector { get; set; }

        public Func<TSourceItem, TDestinationItem> CreateFunction { get; set; }

        public Action<TSourceItem, TDestinationItem> UpdateFunction { get; set; }

        public Action<TSourceItem, TDestinationItem> RemoveFunction { get; set; }

        public bool KeepRemovedItemsInDestinationCollection { get; set; }


        public ICollection<TDestinationItem> Resolve(TSource source, TDestination destination, ICollection<TSourceItem> sourceMember, ICollection<TDestinationItem> destMember, ResolutionContext context)
        {
            var sourceKeys = sourceMember.ToDictionary(SourceKeySelector, i => i);
            var destinationKeys = destMember.ToDictionary(DestinationKeySelector, i => i);

            foreach (var sourceItem in sourceMember)
            {
                var key = SourceKeySelector(sourceItem);

                TDestinationItem destItem;
                if (!destinationKeys.TryGetValue(key, out destItem))
                {
                    // create
                    destItem = CreateFunction(sourceItem);
                    destMember.Add(destItem);
                }
                else
                {
                    // update
                    UpdateFunction(sourceItem, destItem);
                }
            }

            foreach (var destItem in new List<TDestinationItem>(destMember))
            {
                var key = DestinationKeySelector(destItem);

                TSourceItem sourceItem;
                if (!sourceKeys.TryGetValue(key, out sourceItem))
                {
                    // delete
                    RemoveFunction(sourceItem, destItem);

                    if (!KeepRemovedItemsInDestinationCollection)
                    {
                        destMember.Remove(destItem);
                    }
                }
            }

            return destMember;
        }
    }
}