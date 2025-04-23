# White Lagoon

**White Lagoon** is a resort booking website that allows users to reserve villas for specific durations. The system supports two user roles: **Admin** and **Customer**. **Admins** can manage villa listings and amenities, while **Customers** can browse available villas, register for an account, and make bookings with integrated Stripe payment.

## Features

### Authentication & Roles
- **Customer**:  
  - Register & login  
  - View available villas  
  - Book villas for specific nights

- **Admin**:  
  - Create, Read, Update, and Delete villas  
  - Manage amenities associated with each villa

> Role management is handled using **ASP.NET Core Identity**.

### Payment Integration

- Stripe is used to securely handle customer payments.

## Technologies Used

- **Backend:** ASP.NET Core MVC  
- **Frontend:** Razor Views, HTML, CSS, JavaScript, jQuery Validation  
- **Database:** SQL Server  
- **ORM:** Entity Framework Core  
- **Authentication:** ASP.NET Core Identity  
- **Architecture:** Clean Architecture + Repository Pattern  
- **Payment:** Stripe

## Getting Started

To run this project locally:

1. Clone the repository.
2. Create a `appsettings.json` file in the web project and add the following content:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_Database_Connection_String"
  },
  "Stripe": {
    "SecretKey": "Stripe_Secret_Key",
    "PublishableKey": "Stripe_Publishable_Key"
  }
}
```
> You need to create a [Stripe](https://stripe.com/) account to obtain the API keys.

3. Run the project using Visual Studio or `dotnet run` from the terminal.

## Project Status

This project is currently in **development**. More features and enhancements will be added in the near future.