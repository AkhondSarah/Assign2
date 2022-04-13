namespace Lab4N.Models
{
    public class AdsBrokerage
    {
        public int ID { get; set; }

        public int AdvertisementID { get; set; }

        public Advertisement Advertisement { get; set; }

        public string BrokerageId { get; set; }

        public Brokerage Brokerages { get; set; }
    }
}
