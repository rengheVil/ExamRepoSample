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
                Console.WriteLine("\n--- Employee Management ---");
                Console.WriteLine("1. Create Employee");
                Console.WriteLine("2. List Employees");
                Console.WriteLine("3. Update Employee");
                Console.WriteLine("4. Delete Employee");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option: ");
                string option = Console.ReadLine();
                using (var connection = OpenConnection())
                {
                    switch (option)
                    {
                        case "1":
                            Console.Clear(); // Clear the screen before creating an employee
                            CreateEmployee(connection);
                            break;
                        case "2":
                            Console.Clear(); // Clear the screen before listing employees
                            ListEmployees(connection);
                            break;
                        case "3":
                            Console.Clear(); // Clear the screen before updating an employee
                            UpdateEmployee(connection);
                            break;
                        case "4":
                            Console.Clear(); // Clear the screen before deleting an employee
                            DeleteEmployee(connection);
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
            var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=EmployeeDb;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";
            var connection = new SqlConnection(connectionString);
            connection.Open();

            string createTableQuery = @"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Employees')
        BEGIN
            CREATE TABLE Employees (
                EmployeeID INT PRIMARY KEY IDENTITY(1,1),
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

        // Create a new employee
        static void CreateEmployee(SqlConnection connection)
        {
            Console.Write("Enter First Name: ");
            string firstName = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            string lastName = Console.ReadLine();
            Console.Write("Enter Date of Birth (yyyy-mm-dd): ");
            string dob = Console.ReadLine();

            string insertQuery = "INSERT INTO Employees (FirstName, LastName, DateOfBirth) VALUES(@FirstName, @LastName, @DateOfBirth)";
            using (var command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@DateOfBirth", dob);
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Employee created successfully.");
        }
        // List all employees
        static void ListEmployees(SqlConnection connection)
        {
            string selectQuery = "SELECT * FROM Employees";
            using (var command = new SqlCommand(selectQuery, connection))
            using (var reader = command.ExecuteReader())
            {
                Console.WriteLine("\n--- Employee List ---");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["EmployeeID"]}, Name: {reader["FirstName"]}{reader["LastName"]}, DOB: {reader["DateOfBirth"]} ");
                }
            }
        }
        // Update an employee
        static void UpdateEmployee(SqlConnection connection)
        {
            Console.Write("Enter Employee ID to update: ");
            int employeeID = int.Parse(Console.ReadLine());
            Console.Write("Enter new First Name: ");
            string newFirstName = Console.ReadLine();
            Console.Write("Enter new Last Name: ");
            string newLastName = Console.ReadLine();
            string updateQuery = "UPDATE Employees SET FirstName = @FirstName, LastName = @LastName WHERE EmployeeID = @EmployeeID";
            using (var command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@FirstName", newFirstName);
                command.Parameters.AddWithValue("@LastName", newLastName);
                command.Parameters.AddWithValue("@EmployeeID", employeeID);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Employee updated successfully.");
                }
                else
                {
                    Console.WriteLine("Employee not found.");
                }
            }
        }
        // Delete an employee
        static void DeleteEmployee(SqlConnection connection)
        {
            Console.Write("Enter Employee ID to delete: ");
            int employeeID = int.Parse(Console.ReadLine());
            string deleteQuery = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";
            using (var command = new SqlCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@EmployeeID", employeeID);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Employee deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Employee not found.");
                }
            }
        }
    }
}

