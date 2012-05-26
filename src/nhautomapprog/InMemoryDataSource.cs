using System;
using System.Collections.Generic;
using System.Linq;
using nhautomapprog.Models;

namespace nhautomapprog {
	public class InMemoryDataSource {
		public static IList<Address> CreateAddresses() {
			return new List<Address> {
				new Address {
				StreetAddress = "Main Street 1",
				City = "City 1",
				StateName = "SN 1",
				Zip = "12345"
			},
			new Address {
				StreetAddress = "Main Street 2",
				City = "City 2",
				StateName = "SN 2",
				Zip = "11223"
			},
			new Address {
				StreetAddress = "Main Street 3",
				City = "City 3",
				StateName = "SN 3",
				Zip = "22445"
			},
			new Contact {
					StreetAddress = "Main Street 4",
					City = "City 4",
					StateName = "SN 4",
					Zip = "14567",
					Email = "tst@site.com",
					Mobile = "123-123-123",
					Phone = "12345678"
				}
			};
		}

		public static IList<Customer> CreateCustomers(IList<Address> addresses) {
			var i = 1;
			return addresses.Select(address => new Customer {
				Name = "Inc. " + (i++),
				Address = address,
				Level = i % 2 == 0 ? Level.Bronze : Level.Silver,
				Interest = new InterestComponent {
					Interest1 = "Cooking", Interest2 = "Skiing"
				}
			}).ToList();
		}

		public static void AddRelatedCustomers(IList<Customer> customers) {
			var i = 1;
			foreach (var customer in customers) {
				customer.RelatedCustomers.Add(new Customer {
						Name = "Related Inc. " + (i++),
						Address = customer.Address,
						Level = i % 2 == 0 ? Level.Gold : Level.Bronze,
						Interest = new InterestComponent {
						Interest1 = "Reading",
						Interest2 = "Hiking"
					}
				});
			}
		}

		public static IList<Order> CreateOrders(IList<Customer> customers, IList<Product> products) {
			var results = new List<Order>();
			var random = new Random(DateTime.Now.Millisecond);

			foreach (var customer in customers) {
				var order = new Order {
					Products = new List<Product> {
						products[random.Next(2, 5)],
						products[random.Next(1, 9)]
					},
					DateTime = DateTime.Now,
					Customer = customer
				};

				foreach (var product in order.Products) {
					product.Orders.Add(order);
				}
				customer.Orders.Add(order);

				results.Add(order);
			}
			return results;
		}

		public static IList<Product> CreateProducts() {
			var results = new List<Product>();
			for (var i = 0; i < 10; i++) {
				var newProduct = new Product { Name = string.Format("Widget, Type {0:D2}", i) };
				results.Add(newProduct);
			}
			return results;
		}
	}
}
