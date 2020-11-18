namespace SecureMVCApp.Models
{
    public class PurchaseOrder
    {
        private static int nextId = 1;
        public PurchaseOrder(double amount,string description, string department)
        {
            Id = nextId++;
            Amount = amount;
            Description = description;
            Department = department;
        }
        public int Id { get;  }
        public double Amount { get; set; }
        public string Description { get; set; }

        public string  Department { get;  }
    }
}