namespace SqliteWrapper {
    class Program {
        static async Task Main(string[] args) {
            SqliteWrapper sql_wrapper = new SqliteWrapper("usersdata.db");

            List<UserModel>? select_users_v1 = await sql_wrapper.select<UserModel>("Users");
            List<UserModel>? select_users_v2 = await sql_wrapper.select<UserModel>("Users", "age > 20");

            int insert_user_v1 = await sql_wrapper.insert<UserModel>("Users",
                new UserModel() { Name = "Test Name 0", Age = 999 });
            int insert_users_v2 = await sql_wrapper.insert<UserModel>("Users",
                new UserModel() { Name = "Test Name 0", Age = 999 },
                new UserModel() { Name = "Test Name 1", Age = 888 });

            int update_user_v1 = await sql_wrapper.update<UserModel>("Users", 
                new UserModel() { Name = "Hello"}, "_id = 1");

            int delete_user_v1 = await sql_wrapper.delete("Users", "Name = 'Test Name 1'");
        }

        public class UserModel {
            public string Name { get; set; }
            public long Age { get; set; }
        }
    }
}