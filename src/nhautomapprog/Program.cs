using System;
using System.Reflection;
using NhAutoMap;
using nhautomapprog.Models;

namespace nhautomapprog {
	public class Program {
		private static void Main(string[] args) {
			var config = new NhConfig {
				Name = "NHTest",
				ConnectionString = "Data Source=(local);Initial Catalog=test;Integrated Security = true",
				// It can be defined in another assembly.
				MappingsAssemblies = new Assembly[] {typeof (Product).Assembly},
				MappingsNamespace = typeof (Product).Namespace,
				ValidationDefinitionsNamespace = typeof (Product).Namespace,
				ShowLogs = true,
				DropTablesCreateDbSchema = true,
				OutputXmlMappingsFile = "mappings.xml",
				DbSchemaOutputFile = "db.sql",
				// I want to ignore the base-type, Otherwise NHibernate would see the BaseEntity as an actual entity.
				BaseEntityToIgnore = typeof (BaseEntity),
				MapAllEnumsToStrings = true,
				// Otherwise Enums will be mapped to integers automatically.
				AutoMappingOverride = modelMapper => {
					modelMapper.BeforeMapProperty += (modelInspector, member, map) => {
					// I want all of the "Name" fields to be unique.
					if (member.LocalMember.Name.Equals("Name")) {
							map.Unique(true);
						}
					};
				},
				OnConfigCreated = cfg => {
					// inject stuff to config here
				}
			};

			var addresses = InMemoryDataSource.CreateAddresses();
			var customers = InMemoryDataSource.CreateCustomers(addresses);
			InMemoryDataSource.AddRelatedCustomers(customers);
			var products = InMemoryDataSource.CreateProducts();
			var orders = InMemoryDataSource.CreateOrders(customers, products);

			using (var sessionFactory = config.SetUpSessionFactory())
			using (var session = sessionFactory.OpenSession())
			using (var tx = session.BeginTransaction()) {
				// Save Products
				foreach (var product in products)
					session.SaveOrUpdate(product);

				// Save Orders (also saves Customers and their Addresses & Contacts)
				foreach (var order in orders)
					session.SaveOrUpdate(order);

				tx.Commit();
			}

			Console.WriteLine("Press a key...");
			Console.Read();
		}
	}
}
