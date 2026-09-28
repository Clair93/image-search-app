namespace ImageSearch.Core
{
    public abstract record Result<TData, TError>
    {
        private Result()
        {
        }

        public sealed record Success(TData Value) : Result<TData, TError>;

        public sealed record Error(TError Value) : Result<TData, TError>;
    }
}
