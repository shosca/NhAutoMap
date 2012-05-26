using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;

namespace NhAutoMap {
	public class NhConfig {
		public string Name { get; set; }

		public Action<ModelMapper> AutoMappingOverride { set; get; }

		public Action<Configuration> OnConfigCreated { set; get; }

		public bool MapAllEnumsToStrings { set; get; }

		public Type BaseEntityToIgnore { set; get; }

		public string ConnectionString { set; get; }

		public string DbSchemaOutputFile { set; get; }

		public bool DropTablesCreateDbSchema { set; get; }

		public Assembly[] MappingsAssemblies { set; get; }

		public string MappingsNamespace { set; get; }

		public string OutputXmlMappingsFile { set; get; }

		public bool ShowLogs { set; get; }

		public string ValidationDefinitionsNamespace { set; get; }

		public ISessionFactory SetUpSessionFactory() {
			var config = ReadConfigFromCacheFileOrBuildIt();
			var sessionFactory = config.BuildSessionFactory();
			CreateDbSchema(config);
			return sessionFactory;
		}

		private Configuration BuildConfiguration() {
			var config = InitConfiguration();
			var mapping = GetMappings();
			config.AddDeserializedMapping(mapping, Name);
			if (OnConfigCreated != null) {
				OnConfigCreated(config);
			}
			return config;
		}

		private void CreateDbSchema(Configuration cfg) {
			if (!DropTablesCreateDbSchema) return;
			new SchemaExport(cfg).SetOutputFile(DbSchemaOutputFile).Create(script: true, export: true);
		}

		private void DefineBaseClass(ConventionModelMapper mapper) {
			if (BaseEntityToIgnore == null) return;
			mapper.IsEntity((type, declared) =>
				BaseEntityToIgnore.IsAssignableFrom(type) &&
				BaseEntityToIgnore != type &&
				!type.IsInterface);
			mapper.IsRootEntity((type, declared) => type.BaseType == BaseEntityToIgnore);
		}

		private HbmMapping GetMappings() {
			//Using the built-in auto-mapper
			var mapper = new ConventionModelMapper();
			DefineBaseClass(mapper);
			var allEntities =
				MappingsAssemblies.SelectMany(a => a.GetTypes()).Where(t => t.Namespace == MappingsNamespace).ToList();
			mapper.AddAllManyToManyRelations(allEntities);
			mapper.ApplyNamingConventions();
			if (MapAllEnumsToStrings) mapper.MapAllEnumsToStrings();
			if (AutoMappingOverride != null) AutoMappingOverride(mapper);
			var mapping = mapper.CompileMappingFor(allEntities);
			ShowOutputXmlMappings(mapping);
			return mapping;
		}

		private Configuration InitConfiguration() {
			var configure = new Configuration();
			configure.SessionFactoryName("BuildIt");

			configure.DataBaseIntegration(db => {
				db.ConnectionProvider<DriverConnectionProvider>();
				db.Dialect<MsSql2008Dialect>();
				db.Driver<Sql2008ClientDriver>();
				db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
				db.IsolationLevel = IsolationLevel.ReadCommitted;
				db.ConnectionString = ConnectionString;
				db.Timeout = 10;
				db.BatchSize = 20;

				if (ShowLogs) {
					db.LogFormattedSql = true;
					db.LogSqlInConsole = true;
					db.AutoCommentSql = false;
				}
			});

			return configure;
		}

		private Configuration ReadConfigFromCacheFileOrBuildIt() {
			Configuration nhConfigurationCache;
			var nhCfgCache = new ConfigurationFileCache(this);
			var cachedCfg = nhCfgCache.LoadConfigurationFromFile();
			if (cachedCfg == null) {
				nhConfigurationCache = BuildConfiguration();
				nhCfgCache.SaveConfigurationToFile(nhConfigurationCache);
			} else {
				nhConfigurationCache = cachedCfg;
			}
			return nhConfigurationCache;
		}

		private void ShowOutputXmlMappings(HbmMapping mapping) {
			if (!ShowLogs) return;
			var outputXmlMappings = mapping.AsString();
			Console.WriteLine(outputXmlMappings);
			File.WriteAllText(OutputXmlMappingsFile, outputXmlMappings);
		}
	}
}
