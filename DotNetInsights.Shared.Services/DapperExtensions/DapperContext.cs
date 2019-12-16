using DotNetInsights.Shared.Contracts.DapperExtensions;
using DotNetInsights.Shared.Services.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace DotNetInsights.Shared.Services.DapperExtensions
{
    public abstract class DapperContext : IDapperContext
    {
        private readonly IFormatProvider _formatProvider;
        private readonly IDbConnection _connection;

        protected DapperContext(IFormatProvider formatProvider, IDbConnection connection)
        {
            _formatProvider = formatProvider;
            _connection = connection;
            MapContext(this);
        }

        public void MapContext(IDapperContext dapperContext = null)
        {
            if (dapperContext == null)
                dapperContext = this;

            
            var suffixAttribute = GetType().GetCustomAttribute<SuffixAttribute>();

            var tableSuffix = suffixAttribute?.Suffix;
            
            var properties = GetProperties(dapperContext.GetType());

            
            foreach (var property in properties)
            {

                var propertyType = property.PropertyType;

                var propertyGenericType = propertyType.GenericTypeArguments;
                var a = typeof(Mapping<>);
                var gA = a.MakeGenericType(propertyGenericType);

                var b = typeof(IMapping<>);
                var gB = b.MakeGenericType(propertyGenericType);
                
                if(propertyType != gB)
                    continue;

                var mappings = propertyGenericType[0].GetCustomAttribute<TableAttribute>();

                var tableName = mappings?.Name ?? property.Name;
                var schema = mappings?.Schema ?? "dbo";

                tableName = $"{tableSuffix}{tableName}";

                property.SetValue(dapperContext, Activator.CreateInstance(gA, _formatProvider, _connection, schema, tableName));
            }
        }

        private IEnumerable<PropertyInfo> GetProperties(Type sourceType)
        {
            return sourceType.GetProperties();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool gc)
        {
            var instanceType = GetType();
            var properties = GetProperties(instanceType);

            foreach (var  property in properties)
            {
                var propertyType = property.PropertyType;

                var propertyGenericType = propertyType.GenericTypeArguments;

                var b = typeof(IMapping<>);
                var gB = b.MakeGenericType(propertyGenericType);

                if(propertyType != gB)
                    continue;

                var propertyValue = property.GetValue(this) as IDisposable;
                propertyValue?.Dispose();
            }

            _connection?.Dispose();
        }
    }
}