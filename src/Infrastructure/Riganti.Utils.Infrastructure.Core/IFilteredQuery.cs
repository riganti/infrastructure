namespace Riganti.Utils.Infrastructure.Core
{
    public interface IFilteredQuery<TResult, TFilter> : IQuery<TResult>
    {

        TFilter Filter { get; set; }

    }
}