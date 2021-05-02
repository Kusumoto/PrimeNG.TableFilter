namespace PrimeNG.TableFilter
{
    public enum SortingEnumeration
    {
        OrderByAsc = 1,
        OrderByDesc = -1
    }

    public enum OperatorEnumeration
    {
        And = 1,
        Or = 2,
        None = 3
    }

    public static class OperatorConstant
    {
        private const string And = "and";
        private const string Or = "or";

        public static OperatorEnumeration ConvertOperatorEnumeration(string value)
        {
            switch (value.ToLower())
            {
                case And:
                    return OperatorEnumeration.And;
                case Or:
                    return OperatorEnumeration.Or;
                default:
                    return OperatorEnumeration.None;
            }
        }
    }
}