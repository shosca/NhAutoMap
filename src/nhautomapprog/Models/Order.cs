using System;
using System.Collections.Generic;

namespace nhautomapprog.Models {
	public class Order : BaseEntity {
		public Order() {
			Products = new List<Product>();
			Customer = new Customer();
		}

		public virtual DateTime DateTime { set; get; }

		public virtual Customer Customer { set; get; }

		// Many-to-one Association        

		public virtual IList<Product> Products { set; get; } // Many-to-many Association        
	}
}
