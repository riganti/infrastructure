using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.Execution;

namespace Riganti.Utils.Infrastructure.AutoMapper
{
    public class SyncByKeyCollectionResolver<TSource, TSourceItem, TDestination, TDestinationItem, TKey>
        : IMemberValueResolver<TSource, TDestination, ICollection<TSourceItem>, ICollection<TDestinationItem>>

    {

        public Func<TSourceItem, TKey> SourceKeySelector { get; set; }

        public Func<TDestinationItem, TKey> DestinationKeySelector { get; set; }

        public Func<TSourceItem, TDestinationItem> CreateFunction { get; set; }

        public Action<TSourceItem, TDestinationItem> UpdateFunction { get; set; }

        public Action<TDestinationItem> RemoveFunction { get; set; }

        public bool KeepRemovedItemsInDestinationCollection { get; set; }


        public ICollection<TDestinationItem> Resolve(TSource source, TDestination destination, ICollection<TSourceItem> sourceMember, ICollection<TDestinationItem> destMember, ResolutionContext context)
        {
            var sourceKeys = sourceMember.Select(SourceKeySelector).ToList();
            var destinationKeys = destMember.Select(DestinationKeySelector).ToList();

            foreach (var sourceItem in sourceMember)
            {
                var key = SourceKeySelector(sourceItem);

                TDestinationItem destItem;
                if (!destinationKeys.Contains(key))
                {
                    // create
                    destItem = CreateFunction(sourceItem);
                    destMember.Add(destItem);
                }
                else
                {
                    // update
                    destItem = destMember.First(i => DestinationKeySelector(i).Equals(key));
                    UpdateFunction(sourceItem, destItem);
                }
            }

            foreach (var destItem in new List<TDestinationItem>(destMember))
            {
                var key = DestinationKeySelector(destItem);

                if (!sourceKeys.Contains(key))
                {
                    // delete
                    RemoveFunction(destItem);

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