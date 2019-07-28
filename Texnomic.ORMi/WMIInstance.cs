using Microsoft.Management.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Texnomic.ORMi.Attributes;

namespace Texnomic.ORMi
{
    public class WmiInstance
    {
        [WmiIgnore]
        internal CimSession Session { get; set; }

        [WmiIgnore]
        internal CimInstance Instance { get; set; }

        protected TResult Execute<TValue, TResult>([CallerMemberName] string Name = default, Dictionary<string, TValue> Parameters = default)
        {
            using var ParametersCollection = new CimMethodParametersCollection();

            Parameters?.ToList()
                       .ConvertAll(Parameter => CimMethodParameter.Create(Parameter.Key, Parameter.Value, CimFlags.Parameter))
                       .ForEach(Parameter => ParametersCollection.Add(Parameter));

            //Instance.CimClass.CimClassMethods[Name].Parameters;

            var MethodResult = Session.InvokeMethod(Instance, Name, ParametersCollection);

            return (TResult)Convert.ChangeType(MethodResult.ReturnValue.Value, CimConverter.GetDotNetType(MethodResult.ReturnValue.CimType));
        }

        internal string Class => GetType().GetCustomAttribute<WmiClass>(true)?.Name;

        internal string Namespace => GetType().GetCustomAttribute<WmiClass>(true)?.Namespace;

        internal string SearchProperties => GetSearchProperties();

        internal string GetSearchProperties()
        {
            var SearchProperties = new List<string>();

            foreach (var Properties in GetType().GetProperties())
            {
                var IgnoreAttribute = Properties.GetCustomAttribute<WmiIgnore>();

                if (IgnoreAttribute != null) continue;

                var PropertyAttribute = Properties.GetCustomAttribute<WmiProperty>();

                if (PropertyAttribute == null)
                {
                    SearchProperties.Add(Properties.Name.ToUpper());
                }
                else
                {
                    SearchProperties.Add(PropertyAttribute.Name.ToUpper());
                }

            }

            return string.Join(", ", SearchProperties);
        }

        protected void SetProperty<T>([CallerMemberName] string Name = default, T Value = default)
        {
            Instance.CimInstanceProperties[Name].Value = Value;
            Instance = Session.ModifyInstance(Instance);
            GetType().GetProperty(Name).SetValue(this, Value);
        }
    }
}
