using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Texnomic.SecureDNS.Data;
using Texnomic.SecureDNS.Data.Models;

namespace Texnomic.SecureDNS.Configurations
{
    public class DatabaseConfigurationProvider : IConfigurationProvider
    {
        private readonly ConfigurationDatabaseContext DatabaseContext;

        private readonly Dictionary<string, string> Configurations;

        public DatabaseConfigurationProvider(Action<DbContextOptionsBuilder> OptionsBuilderAction)
        {
            var OptionsBuilder = new DbContextOptionsBuilder<ConfigurationDatabaseContext>();

            OptionsBuilderAction(OptionsBuilder);

            DatabaseContext = new ConfigurationDatabaseContext(OptionsBuilder.Options);

            Configurations = new Dictionary<string, string>();
        }

        public void Set(string Key, string Value)
        {
            Configurations.Add(Key, Value);
            DatabaseContext.Configurations.Add(new Configuration() { Key = Key, Value = Value });
        }

        public void Load()
        {
            DatabaseContext.Database.EnsureCreated();

            Configurations = DatabaseContext.Configurations.Any()
                ? DatabaseContext.Configurations.ToDictionary(Key => Key.Key, Value => Value.Value)
                : CreateAndSaveDefaultValues(DatabaseContext);
        }

        private static Dictionary<string, string> CreateAndSaveDefaultValues(ConfigurationDatabaseContext DatabaseContext)
        {
            var InitialConfigurations = new Dictionary<string, string>
            {
                {"quote1", "I aim to misbehave."},
                {"quote2", "I swallowed a bug."},
                {"quote3", "You can't stop the signal, Mal."}
            };

            DatabaseContext.Configurations.AddRange(InitialConfigurations
                                                    .Select(Config => new Configuration
                                                    {
                                                        Key = Config.Key,
                                                        Value = Config.Value
                                                    })
                                                    .ToArray());

            DatabaseContext.SaveChanges();

            return InitialConfigurations;
        }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> EarlierKeys, string ParentPath)
        {
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public bool TryGet(string Key, out string Value)
        {
            return Configurations.TryGetValue(Key, out Value);
        }
    }
}
