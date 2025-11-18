using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.SqlServer;
internal static class SqlQueryExtensions
{
    //TODO: Documentation
    internal static IEnumerable<SqlParameter> GetParameterCollection(this SqlQuery query)
    {
        static SqlParameter GetSqlParameter(ParameterTag parameter, object value)
        {
            SqlParameter sqlParameter = new(parameter, value)
            {
                SqlDbType = parameter.SqlType?.Type ?? (new SqlTypeDefinition(value)).Type
            };

            if (parameter.SqlType?.UseMax is not null)
            {
                if (parameter.SqlType.UseMax)
                    sqlParameter.Size = -1;
            }
            else
            {
                if (parameter.SqlType?.Size is not null)
                    sqlParameter.Size = parameter.SqlType.Size.Value;
                if (parameter.SqlType?.Precision is not null)
                    sqlParameter.Precision = parameter.SqlType.Precision.Value;
                if (parameter.SqlType?.Scale is not null)
                    sqlParameter.Scale = parameter.SqlType.Scale.Value;
            }

            return sqlParameter;
        }

        return query
                    .Parameters
                    .AsEnumerable()
                    .Select(parameter => GetSqlParameter(parameter.Key, parameter.Value));
    }
}
