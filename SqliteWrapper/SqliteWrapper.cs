using Microsoft.Data.Sqlite;
using System.Reflection;

namespace SqliteWrapper {
    public class SqliteWrapper {

        private readonly string _path_connect_database;
        
        public SqliteWrapper(string path_connect_database) {
            _path_connect_database = $"Data Source={path_connect_database}";
        }


        /// <summary>
        /// Select request returning List<TModel>
        /// </summary>
        /// <typeparam name="TModel">Model, for column selection and data matching</typeparam>
        /// <param name="table">Table name</param>
        /// <param name="where">
        /// The query string, 'where' does not need to be entered,
        /// it is generated independently
        /// </param>
        /// <returns>Returns a List<TModel> of all found data by condition</returns>

        public async Task<List<TModel>?> get<TModel>(
            string table,
            string? where = null) where TModel : class {

            string full_command = string.Empty;
            string[] properties = typeof(TModel).GetProperties()
                .Select(value => value.Name).ToArray();

            full_command = (properties.Length == 0
                ? null : $"select {string.Join(", ", properties)}")
                ?? throw new ArgumentNullException(nameof(properties));

            full_command = string.IsNullOrEmpty(table) ? table : $"{full_command} from {table}"
                ?? throw new ArgumentNullException(nameof(table));

            full_command = string.IsNullOrEmpty(where) ? full_command : $"{full_command} where {where}";

            using (SqliteConnection connection = new SqliteConnection(_path_connect_database)) {
                connection.Open();
                SqliteCommand command = new SqliteCommand(full_command, connection);

                using (var reader = await command.ExecuteReaderAsync()) {
                    if (!reader.HasRows) return null;

                    List<TModel> result = new List<TModel>();

                    while (reader.Read()) {

                        TModel model_istance = Activator.CreateInstance<TModel>();
                        foreach (string parameter in properties) {
                            PropertyInfo? property = typeof(TModel).GetProperty(parameter);
                            if (property is null) continue;

                            object value = reader[parameter];
                            property.SetValue(model_istance, value);
                        }
                        result.Add(model_istance);

                    }
                    return result.Count == 0 ? null: result;
                }
            }
        }

        /// <summary>
        /// An insert request returning a bool. True - the data was successfully
        /// added, False - the data was not added
        /// </summary>
        /// <typeparam name="TModel">Model, for column selection and data matching</typeparam>
        /// <param name="table">Table name</param>
        /// <param name="model">Model, for column selection and data matching</param>
        /// <returns>
        /// True - the data was successfully
        /// added, False - the data was not added
        /// </returns>
        public async Task<bool> insert<TModel>(
            string table,
            TModel model) where TModel : class {

            string full_command = string.Empty;

            PropertyInfo[] property_info = typeof(TModel).GetProperties();
            string[] properties = property_info.Select(value => value.Name).ToArray();

            full_command = string.IsNullOrEmpty(table) ? table : $"insert into {table} "
                ?? throw new ArgumentNullException(nameof(table));

            full_command = (properties.Length == 0
                ? null : $"{full_command}({string.Join(", ", properties)}) values (@{string.Join(", @", properties)})")
                ?? throw new ArgumentNullException(nameof(properties));

            using (SqliteConnection connection = new SqliteConnection(_path_connect_database)) {
                connection.Open();
                SqliteCommand command = new SqliteCommand(full_command, connection);

                for (int index = 0; index < property_info.Length; index++) {
                    SqliteParameter sql_parameter = new SqliteParameter($"@{properties[index]}",
                        property_info[index].GetValue(model));

                    command.Parameters.Add(sql_parameter);
                }

                return await command.ExecuteNonQueryAsync() == 1 ? true : false;
            }
        }
    }
}
