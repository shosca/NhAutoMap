using System.Collections.Generic;

namespace nhautomapprog.Models {
	public class Product : BaseEntity {
		public Product() {
			Orders = new List<Order>();
		}

		public virtual string Name { set; get; }

		public virtual IList<Order> Orders { set; get; } // Many-to-many Association        
	}
}
