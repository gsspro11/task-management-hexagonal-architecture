namespace Poc.Security.Factors.Model.RiskData
{
    public  class PixRiskData: IRiskData
    {
        public string TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<PixRiskDataPhoneNumbers> PhoneNumbers { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal TransactionValue { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
        public string RecipientIdNumber { get; set; }
        public string RecipientIdNumberType { get; set; }
        public string RecipientBranch { get; set; }
        public string RecipientAccountNumber { get; set; }
        public string RecipientUserName { get; set; }
        public string RecipientInstitutionInfo { get; set; }
    }
    public class PixRiskDataPhoneNumbers
    {
        public string CountryCode { get; set; }
        public string AreaCode { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
    }
}
