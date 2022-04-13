namespace Lab4N.Models
{
    public class Subscription
    {
        public int ClientId
        {
            get;
            set;
        }

        public string BrokerageId
        {
            get;
            set;
        }
        public Brokerage Brokerages { get; set; }
        public Client Clients { get; set; }
    }
}
