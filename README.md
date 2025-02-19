# BlinkBank - Online Banking System

BlinkBank is an ASP.NET Core MVC web application that provides a secure and user-friendly electronic wallet and banking experience, enabling users to register, log in, manage their accounts, and perform financial transactions efficiently.

## Features

- **User Authentication**:
  - Registration with email confirmation
  - Login/Logout functionality
  - ASP.NET Core Identity management
  - Password hashing and validation
  - Remember me option for persistent login sessions

- **Account Management**:
  - Bank account creation with initial balance
  - Account details viewing
  - Account modification and deletion
  - Transaction history tracking

- **Financial Operations**:
  - Cash withdrawals with balance limits
  - Deposits with maximum amount restrictions
  - Inter-account transfers
  - Real-time balance updates

- **User Interface**
  - Responsive and modern design
  - Interactive animations and smooth transitions
  - DataTables for transaction history with search and pagination

- **Security**:
  - Authorization requirements for sensitive operations
  - Transaction validation rules
  - Audit trails for all financial transactions

## Technologies Used

- **Frontend:**
  - HTML, CSS, JavaScript
  - Bootstrap for styling
  - jQuery for UI enhancements

- **Backend:**
  - **Framework**: ASP.NET Core 6.0
  - **Authentication**: ASP.NET Core Identity
  - **Architecture**: ASP.NET Core MVC
  - **Programming Language**: C# for server-side logic
  - **Database ORM**: Entity Framework Core for database interactions

- **Database:**
  - SQL Server for storing user and transaction data

## Project Structure
```
BlinkBank/
│── Controllers/        # Handles HTTP requests and application logic
├── Migrations/         # Entity Framework migrations
│── Models/             # Defines data models for the application
│── Views/              # Contains Razor view templates for UI rendering
│── ViewModels/         # View-specific models for passing data to the UI
│── wwwroot/            # Static assets (CSS, JS, images)
│── appsettings.json    # Configuration settings (database, authentication, etc.)
│── Program.cs          # Application entry point
```

## Installation & Setup

### Prerequisites
- .NET 6 SDK
- SQL Server (LocalDB or another instance)
- Visual Studio 2022 or VS Code with C# support

### Installation
1. **Clone the repository**
   ```sh
   git clone https://github.com/ahmadabdelbary2001/MVC-Banking-System.git
   ```

2. **Configure the Database Connection**
   Update the database connection string in `appsettings.json` to match your database configuration:  

   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
   }
   ```

3. **Run database migrations**
   Run the following command in the Package Manager Console to apply migrations:
   ```sh
   Update-Database
   ```
   Alternatively, if using the .NET CLI, run:
   ```sh
   dotnet ef database update
   ```
   
4. **Run the Application**
   Using the .NET CLI:
   ```sh
   dotnet run
   ```

## License 
[MIT License](LICENSE)
