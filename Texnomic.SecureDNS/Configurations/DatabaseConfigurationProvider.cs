using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Texnomic.SecureDNS.Data;
using Texnomic.SecureDNS.Data.Models;

namespace Texnomic.SecureDNS.Configurations
{
    public class DatabaseConfigurationProvider : IConfigurationProvider
    {
        private readonly DatabaseContext DatabaseContext;
        private readonly ConfigurationReloadToken ConfigurationReloadToken;
        private Dictionary<string, string> Configurations;

        public DatabaseConfigurationProvider(Action<DbContextOptionsBuilder> OptionsBuilderAction)
        {
            var OptionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

            OptionsBuilderAction(OptionsBuilder);

            DatabaseContext = new DatabaseContext(OptionsBuilder.Options);

            Configurations = new Dictionary<string, string>();

            ConfigurationReloadToken = new ConfigurationReloadToken();

            //TODO Implement ConfigurationReloadToken
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

        private static Dictionary<string, string> CreateAndSaveDefaultValues(DatabaseContext DatabaseContext)
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
            Configurations.TryGetValue(ParentPath, out string Value);

            return Value is null ? new List<string>() : new List<string>() { Value };
        }

        public IChangeToken GetReloadToken()
        {
            return ConfigurationReloadToken;
        }

        public bool TryGet(string Key, out string Value)
        {
            return Configurations.TryGetValue(Key, out Value);
        }
    }
}
