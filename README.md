# SqliteWrapper 
SqliteWrapper is a small library, CRUD queries wrapped on top ADO.NET SQLite, which allows you to match data using reflection, creating a query in one line of code.

## Usage Example
```csharp
using SqliteWrapper;

// ....

// Initialization of the object, you need to pass the path to the database
SqliteWrapper sql_wrapper = new SqliteWrapper("usersdata.db");

// Using the 'Select' query with or without a condition
List<UserModel>? select_users_v1 = await sql_wrapper.select<UserModel>("Users");
List<UserModel>? select_users_v2 = await sql_wrapper.select<UserModel>("Users", "age > 20");

// Using the 'Insert' query, you can add the nth number of elements
int insert_user_v1 = await sql_wrapper.insert<UserModel>("Users",
  new UserModel() { Name = "Test Name 0", Age = 999 });
int insert_users_v2 = await sql_wrapper.insert<UserModel>("Users",
  new UserModel() { Name = "Test Name 0", Age = 999 },
  new UserModel() { Name = "Test Name 1", Age = 888 });

// Using the 'Update' request, you must pass the model with the already changed data and condition
int update_user_v1 = await sql_wrapper.update<UserModel>("Users", 
  new UserModel() { Name = "Hello"}, "_id = 1");

// Using the 'Delete' query, the simplest of all methods that accepts the condition of deleting an item from the database
int delete_user_v1 = await sql_wrapper.delete("Users", "Name = 'Test Name 1'");

```


## Demonstration of functionality by photo
![image](https://github.com/daniilmusolin/SqliteWrapper/assets/106406473/fea26472-c753-4e16-94dd-6535fd4b6911)
