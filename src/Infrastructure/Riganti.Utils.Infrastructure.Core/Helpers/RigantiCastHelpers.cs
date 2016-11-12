namespace Riganti.Utils.Infrastructure.Core
{
    public static class RigantiCastHelpers
    {
        public static TOut CastTo<TOut>(this object original) where TOut : class
        {
            return (TOut) original;
        }

        public static TOut CastAs<TOut>(this object original) where TOut : class
        {
            return original as TOut;
        }
    }
}