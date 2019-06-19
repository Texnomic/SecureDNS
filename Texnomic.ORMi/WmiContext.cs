using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Management.Infrastructure;
using Texnomic.ORMi.Attributes;

namespace Texnomic.ORMi
{
    public class WmiContext
    {
        private readonly CimSession CimSession;


        public WmiContext(string ComputerName = "localhost")
        {
            CimSession = CimSession.Create(ComputerName);
        }

        public IEnumerable<TInstance> Query<TInstance>() where TInstance : WmiInstance, new()
        {
            var Type = new TInstance();

            var Instances = new List<TInstance>();

            var Query = $"SELECT {Type.SearchProperties} FROM {Type.Class}";

            var CimInstances = CimSession.QueryInstances(Type.Namespace, "WQL", Query);

            foreach (var CimInstance in CimInstances)
            {
                var Instance = Load<TInstance>(CimInstance);

                Instances.Add(Instance);
            }

            return Instances;
        }


        public async Task<IEnumerable<TInstance>> QueryAsync<TInstance>() where TInstance : WmiInstance, new()
        {
            return await Task.Run(() => Query<TInstance>());
        }

        private TInstance Load<TInstance>(CimInstance CimInstance) where TInstance : WmiInstance, new()
        {
            var Instance = new TInstance
            {
                Session = CimSession,
                Instance = CimInstance
            };

            var Properties = Instance.GetType().GetProperties();

            foreach (var Property in Properties)
            {
                SetPropertyValue(CimInstance, Property, Instance);
            }

            return Instance;
        }

        private void SetPropertyValue<T>(CimInstance CimInstance, PropertyInfo PropertyInfo, T Object) where T : WmiInstance
        {
            var IgnoreAttribute = PropertyInfo.GetCustomAttribute<WmiIgnore>();

            if (IgnoreAttribute != null) return;

            var PropertyAttribute = PropertyInfo.GetCustomAttribute<WmiProperty>();

            var PropertyName = PropertyAttribute is null ? PropertyInfo.Name : PropertyAttribute.Name;

            var CimProperty = CimInstance.CimInstanceProperties[PropertyName];

            var Value = Convert.ChangeType(CimProperty.Value, CimConverter.GetDotNetType(CimProperty.CimType));

            PropertyInfo.SetValue(Object, Value, null);
        }
    }
}
