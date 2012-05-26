using System.Collections.Generic;

namespace nhautomapprog.Models {
	public class Customer : BaseEntity {
		public Customer() {
			Orders = new List<Order>();
			Address = new Address();
			RelatedCustomers = new List<Customer>();
		}

		public virtual string Name { set; get; }

		public virtual Level Level { set; get; }

		public virtual Address Address { set; get; } // Many-to-one Association

		public virtual InterestComponent Interest { set; get; } // Component mapping

		public virtual IList<Order> Orders { set; get; } // One-to-many Association

		public virtual IList<Customer> RelatedCustomers { set; get; } // A self-referencing object
	}
}
