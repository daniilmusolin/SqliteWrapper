using SqliteWrapper;
using System.Diagnostics;

namespace SqliteWrapper {
    class Program {
        static async Task Main(string[] args) {

            SqliteWrapper sql = new SqliteWrapper("usersdata.db");

            //List<UserModel>? users = await sql.get<UserModel>("Users");
            bool is_insert = await sql.insert("Users", new UserModel() { Name = "Dima", Age = 17 },
                new UserModel() { Name = "Test", Age = 20 }, new UserModel() { Name = "Igor", Age = 33 });
            Console.WriteLine(is_insert);
        }

        public class UserModel {
            public string Name { get; set; }
            public long Age { get; set; }
        }
    }
}