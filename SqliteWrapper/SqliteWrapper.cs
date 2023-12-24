using Microsoft.Data.Sqlite;
using System.Reflection;

namespace SqliteWrapper {
    public class SqliteWrapper {
        private readonly string _path_connect_database;
        public SqliteWrapper(string path_connect_database) {
            _path_connect_database = $"Data Source={path_connect_database}";
        }

        /// <summary
        /// 'Select' request returning List<TModel>
        /// </summary>
        /// <typeparam name="TModel">Model, for column selection and data matching</typeparam>
        /// <param name="table">Table name</param>
        /// <param name="where">
        /// The query condition, the word "where" does not need to be entered, it is generated independently
        /// </param>
        /// <returns>Returns a List<TModel> of all found data by condition</returns>

        public async Task<List<TModel>?> select<TModel>(
            string table,
            string? where = null) where TModel : class {
            string full_command = string.Empty;
            string[] properties = typeof(TModel).GetProperties().Select(value => value.Name).ToArray();
            full_command = (properties.Length == 0 ? null : $"select {string.Join(", ", properties)}")
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
        /// The 'insert' query returns the number of successfully inserted values added
        /// </summary>
        /// <typeparam name="TModel">Model, for column selection and data matching</typeparam>
        /// <param name="table">Table name</param>
        /// <param name="model">Model, for column selection and data matching</param>
        /// <returns>Returns the number of successfully inserted values</returns>
        public async Task<int> insert<TModel>(
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
                return await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// The 'insert' query returns the number of successfully inserted values added
        /// </summary>
        /// <typeparam name="TModel">Model, for column selection and data matching</typeparam>
        /// <param name="table">Table name</param>
        /// <param name="model">Model, for column selection and data matching</param>
        /// <returns>Returns the number of successfully inserted values</returns>
        public async Task<int> insert<TModel>(
            string table,
            params TModel[] models) where TModel : class {
            string full_command = string.Empty;
            PropertyInfo[] property_info = typeof(TModel).GetProperties();
            if (property_info.Length == 0) throw new ArgumentNullException(nameof(property_info));
            string[] properties = property_info.Select(value => value.Name).ToArray();
            full_command = string.IsNullOrEmpty(table) ? table : $"insert into {table} "
                ?? throw new ArgumentNullException(nameof(table));
            full_command = (properties.Length == 0
                ? null : $"{full_command}({string.Join(", ", properties)}) values ")
                ?? throw new ArgumentNullException(nameof(properties));
            for (int index = 0; index < models.Length; index++) {
                string values = "(";
                for (int index_property = 0; index_property < property_info.Length; index_property++) {
                    PropertyInfo property = property_info[index_property];

                    object? data = property.GetValue(models[index]);
                    string value = property.PropertyType == typeof(string) ? $"'{data}'" : $"{data}";
                    values += (index_property + 1 == property_info.Length) ?
                        $"{value}{(index + 1 == models.Length ? ")" : "), ")} " : $"{value}, "; 
                }
                full_command += values;
            }
            using (SqliteConnection connection = new SqliteConnection(_path_connect_database)) {
                connection.Open();
                SqliteCommand command = new SqliteCommand(full_command, connection);
                return await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// The "update" request returns the number of successfully updated values
        /// </summary>
        /// <typeparam name="TModel">Model, for column selection and data matching</typeparam>
        /// <param name="table">Table name</param>
        /// <param name="model">Model, for column selection and data matching</param>
        /// <returns>Returns the number of successfully updated values</returns>
        public async Task<int> update<TModel>(
            string table,
            TModel model,
            string where) where TModel : class {
            string full_command = string.Empty;
            PropertyInfo[] property_info = typeof(TModel).GetProperties();
            if (property_info.Length == 0) throw new ArgumentNullException(nameof(property_info));
            full_command = string.IsNullOrEmpty(table) ? table : $"update {table} set "
                ?? throw new ArgumentNullException(nameof(table));
            for (int index_property = 0; index_property < property_info.Length; index_property++) {
                PropertyInfo property = property_info[index_property];

                object? data = property.GetValue(model);
                string value = property.PropertyType == typeof(string) ? $"'{data}'" : $"{data}";
                full_command += (index_property + 1 == property_info.Length) 
                    ? $" {property.Name} = {value}" : $"{property.Name} ={value},";
            }
            full_command = string.IsNullOrEmpty(where) ? where : $"{full_command} where {where}"
                ?? throw new ArgumentNullException(nameof(where));
            using (SqliteConnection connection = new SqliteConnection(_path_connect_database)) {
                connection.Open();
                SqliteCommand command = new SqliteCommand(full_command, connection);
                return await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// The "delete" query returns the number of successfully deleted values
        /// </summary>
        /// <param name="table">Table name</param>
        /// <param name="where">
        /// The query condition, the word "where" does not need to be entered,
        /// it is generated independently</param>
        /// <returns>Returns the number of successfully deleted values</returns>
        public async Task<int> delete(
            string table,
            string where) {
            string full_command = string.Empty;
            full_command = string.IsNullOrEmpty(table) ? table : $"delete from {table}"
                ?? throw new ArgumentNullException(nameof(table));
            full_command = string.IsNullOrEmpty(where) ? where : $"{full_command} where {where}"
                ?? throw new ArgumentNullException(nameof(where));
            using (SqliteConnection connection = new SqliteConnection(_path_connect_database)) {
                connection.Open();
                SqliteCommand command = new SqliteCommand(full_command, connection);
                return await command.ExecuteNonQueryAsync();
            }
        }
    }
}
