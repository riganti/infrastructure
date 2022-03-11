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

        public Action<TDestinationItem> RemoveFunction { get; set; }

        public bool KeepRemovedItemsInDestinationCollection { get; set; }

        public Func<TDestinationItem, bool> DestinationFilter { get; set; }


        public ICollection<TDestinationItem> Resolve(TSource source, TDestination destination, ICollection<TSourceItem> sourceMember, ICollection<TDestinationItem> destMember, ResolutionContext context)
        {
            var createFunction = CreateFunction ?? context.Mapper.Map<TSourceItem, TDestinationItem>;
            var updateFunction = UpdateFunction ?? ((s, d) => context.Mapper.Map(s, d));
            var removeFunction = RemoveFunction ?? (d => { });


            var sourceKeys = sourceMember.Select(SourceKeySelector).ToList();
            var destinationKeys = destMember.Where(DestinationFilter).Select(DestinationKeySelector).ToList();

            foreach (var sourceItem in sourceMember)
            {
                var key = SourceKeySelector(sourceItem);

                TDestinationItem destItem;
                if (!destinationKeys.Contains(key))
                {
                    // create
                    destItem = createFunction(sourceItem);
                    destMember.Add(destItem);
                }
                else if (!EqualityComparer<TKey>.Default.Equals(key, default(TKey)))
                {
                    // update
                    destItem = destMember.First(i => DestinationKeySelector(i).Equals(key));
                    updateFunction(sourceItem, destItem);
                }
            }

            foreach (var destItem in new List<TDestinationItem>(destMember.Where(DestinationFilter)))
            {
                var key = DestinationKeySelector(destItem);

                if (!sourceKeys.Contains(key))
                {
                    // delete
                    removeFunction(destItem);

                    if (!KeepRemovedItemsInDestinationCollection)
                    {
                        destMember.Remove(destItem);
                    }
                }
            }

            // we cannot return the original collection instance since AutoMapper erases it and then tries to map the results from this method
            return new List<TDestinationItem>(destMember);
        }
    }
}