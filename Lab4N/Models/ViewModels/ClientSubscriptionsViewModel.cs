namespace Lab4N.Models.ViewModels
{
    public class ClientSubscriptionsViewModel
    {
        public Client Client { get; set; }
        public IEnumerable<BrokerageSubscriptionsViewModel> Subscriptions { get; set; }
    }

}
