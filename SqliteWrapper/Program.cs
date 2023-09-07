using SqliteWrapper;

namespace SqliteWrapper {
    class Program {
        static async Task Main(string[] args) {

            SqliteWrapper sql = new SqliteWrapper("usersdata.db");

            List<UserModel>? users = await sql.get<UserModel>("Users");
            bool is_insert = await sql.insert("Users", new UserModel() { Name = "Dima", Age = 17 }));
        }

        public class UserModel {
            public string Name { get; set; }
            public long Age { get; set; }
        }
    }
}