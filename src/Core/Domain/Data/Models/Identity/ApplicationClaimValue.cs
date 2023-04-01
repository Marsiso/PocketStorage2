namespace Domain.Data.Models.Identity
{
    public sealed class ApplicationClaimValue
    {
        #region Public Constructors

        public ApplicationClaimValue()
        {
        }

        public ApplicationClaimValue(string type, string value)
        {
            Type = type;
            Value = value;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Type { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        #endregion Public Properties
    }
}