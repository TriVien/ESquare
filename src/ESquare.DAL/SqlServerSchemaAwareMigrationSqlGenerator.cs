using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESquare.DAL
{
    public class SqlServerSchemaAwareMigrationSqlGenerator : SqlServerMigrationSqlGenerator
    {
        private readonly string _schema;

        public SqlServerSchemaAwareMigrationSqlGenerator(string schema)
        {
            _schema = schema;
        }

        protected override string Name(string name)
        {
            string[] nameParts = name.Split('.');
            string newName;

            switch (nameParts.Length)
            {
                case 1:
                    newName = $"[{_schema}].[{nameParts[0]}]";
                    break;

                case 2:
                    newName = $"[{_schema}].[{nameParts[1]}]";
                    break;

                case 3:
                    newName = $"[{_schema}].[{nameParts[1]}].[{nameParts[2]}]";
                    break;

                default:
                    throw new NotSupportedException();
            }

            return newName;
        }
    }
}
