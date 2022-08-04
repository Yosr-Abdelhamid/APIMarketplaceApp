namespace APIMarketplaceApp.Models
{
    public class RequestPortfeuille
    {
       
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string ExpireDate { get; set; }
        public string CVV { get; set; }
        public string Sold { get; set; }
        public string Id { get; set; }
    }
}