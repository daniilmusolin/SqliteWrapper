using SqliteWrapper;
using System.Diagnostics;

namespace SqliteWrapper {
    class Program {
        static async Task Main(string[] args) {

            SqliteWrapper sql = new SqliteWrapper("usersdata.db");

            //List<UserModel>? users = await sql.get<UserModel>("Users");
            var data = await sql.update<UserModel>("Users", new UserModel() { Name = "Hello"}, "_id = 1");
        }

        public class UserModel {
            public string Name { get; set; }
           
        }
    }
}