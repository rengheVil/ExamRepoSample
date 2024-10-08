using System;
using Microsoft.Data.SqlClient;

namespace EmployeeCRUDApp
{
    class Program
    {

        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear(); // Clear the console before showing the menu
                Console.WriteLine("\n--- Car Management ---");
                Console.WriteLine("1. Create Car");
                Console.WriteLine("2. List Cars");
                Console.WriteLine("3. Update Car");
                Console.WriteLine("4. Delete Car");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option: ");
                string option = Console.ReadLine();
                using (var connection = OpenConnection())
                {
                    switch (option)
                    {
                        case "1":
                            Console.Clear(); // Clear the screen before creating an Car
                            CreateCar(connection);
                            break;
                        case "2":
                            Console.Clear(); // Clear the screen before listing Cars
                            ListCars(connection);
                            break;
                        case "3":
                            Console.Clear(); // Clear the screen before updating an Car
                            UpdateCar(connection);
                            break;
                        case "4":
                            Console.Clear(); // Clear the screen before deleting an Car
                            DeleteCar(connection);
                            break;
                        case "5":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                    if (!exit)
                    {
                        Console.WriteLine("\nPress Enter to continue...");
                        Console.ReadLine(); // Pause for user to see the result before clearing
                    }
                }
            }
        }
        // Open the SQLite connection
        static SqlConnection OpenConnection()
        {
            var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=CarDb;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";
            var connection = new SqlConnection(connectionString);
            connection.Open();

            string createTableQuery = @"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Cars')
        BEGIN
            CREATE TABLE Cars (
                CarID INT PRIMARY KEY IDENTITY(1,1),
                FirstName NVARCHAR(100) NOT NULL,
                LastName NVARCHAR(100) NOT NULL,
                DateOfBirth DATE NOT NULL
            );
        END";

            using (var command = new SqlCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            return connection;
        }

        // Create a new Car
        static void CreateCar(SqlConnection connection)
        {
            Console.Write("Enter First Name: ");
            string firstName = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            string lastName = Console.ReadLine();
            Console.Write("Enter Date of Birth (yyyy-mm-dd): ");
            string dob = Console.ReadLine();

            string insertQuery = "INSERT INTO Cars (FirstName, LastName, DateOfBirth) VALUES(@FirstName, @LastName, @DateOfBirth)";
            using (var command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@DateOfBirth", dob);
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Car created successfully.");
        }
        // List all Cars
        static void ListCars(SqlConnection connection)
        {
            string selectQuery = "SELECT * FROM Cars";
            using (var command = new SqlCommand(selectQuery, connection))
            using (var reader = command.ExecuteReader())
            {
                Console.WriteLine("\n--- Car List ---");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["CarID"]}, Name: {reader["FirstName"]}{reader["LastName"]}, DOB: {reader["DateOfBirth"]} ");
                }
            }
        }
        // Update an Car
        static void UpdateCar(SqlConnection connection)
        {
            Console.Write("Enter Car ID to update: ");
            int CarID = int.Parse(Console.ReadLine());
            Console.Write("Enter new First Name: ");
            string newFirstName = Console.ReadLine();
            Console.Write("Enter new Last Name: ");
            string newLastName = Console.ReadLine();
            string updateQuery = "UPDATE Cars SET FirstName = @FirstName, LastName = @LastName WHERE CarID = @CarID";
            using (var command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@FirstName", newFirstName);
                command.Parameters.AddWithValue("@LastName", newLastName);
                command.Parameters.AddWithValue("@CarID", CarID);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Car updated successfully.");
                }
                else
                {
                    Console.WriteLine("Car not found.");
                }
            }
        }
        // Delete an Car
        static void DeleteCar(SqlConnection connection)
        {
            Console.Write("Enter Car ID to delete: ");
            int CarID = int.Parse(Console.ReadLine());
            string deleteQuery = "DELETE FROM Cars WHERE CarID = @CarID";
            using (var command = new SqlCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@CarID", CarID);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Car deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Car not found.");
                }
            }
        }
    }
}

