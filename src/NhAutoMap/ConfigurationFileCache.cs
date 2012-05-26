using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using NHibernate.Cfg;

namespace NhAutoMap {
	public class ConfigurationFileCache {
		private readonly string _cacheFile;
		private readonly Assembly[] _assemblies;


		public ConfigurationFileCache(NhConfig config) {
			_cacheFile = config.Name + ".cfg";
			_assemblies = config.MappingsAssemblies;
			if (HttpContext.Current != null) //for the web apps
				_cacheFile = HttpContext.Current.Server.MapPath(
					string.Format("~/App_Data/{0}", Path.GetFileName(_cacheFile))
				);
		}

		public void DeleteCacheFile() {
			if (File.Exists(_cacheFile))
				File.Delete(_cacheFile);
		}

		public bool IsConfigurationFileValid {
			get {
				if (!File.Exists(_cacheFile))
					return false;
				var configInfo = new FileInfo(_cacheFile);

				if (configInfo.Length < 5*1024)
					return false;

				return _assemblies.Any(a => {
					var asmInfo = new FileInfo(a.Location);
					return configInfo.LastWriteTimeUtc >= asmInfo.LastWriteTimeUtc;
				});
			}
		}

		public void SaveConfigurationToFile(Configuration configuration) {
			using (var file = File.Open(_cacheFile, FileMode.Create)) {
				var bf = new BinaryFormatter();
				bf.Serialize(file, configuration);
			}
		}

		public Configuration LoadConfigurationFromFile() {
			if (!IsConfigurationFileValid)
				return null;

			using (var file = File.Open(_cacheFile, FileMode.Open, FileAccess.Read)) {
				var bf = new BinaryFormatter();
				return bf.Deserialize(file) as Configuration;
			}
		}
	}
}
