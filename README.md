# Zoo Assignment API

A .NET 10.0 API for managing zoo animal feeding costs and calculations.

## Overview

The Zoo Assignment API provides endpoints to retrieve and calculate animal feeding costs based on animal types, food requirements, and pricing data. The system supports three animal categories: carnivores, herbivores, and omnivores with different feeding requirements.

## API Endpoints

### Get Total Monthly Cost
**Endpoint:** `GET /api/zoo/getDailyCost`

**Description:** Retrieves the total monthly feeding cost for all animals in the zoo, calculated based on daily food consumption.

**Request:**
```
GET http://localhost:5000/api/zoo/getDailyCost
```

**Response (200 OK):**
```json
{
  "totalMonthlyCost": 1500.50
}
```

**Response Details:**
- `totalMonthlyCost`: The sum of all animals' daily feeding costs multiplied by 30 days (monthly total)

**Error Response (500 Internal Server Error):**
The endpoint returns a 500 status code if an error occurs during processing.

## Running the Application

### Prerequisites
- .NET 10.0 SDK or later
- Data files (included in project):
  - `animals.csv` - Animal data
  - `prices.txt` - Food pricing
  - `zoo.xml` - Additional zoo data

### Start the API
```powershell
dotnet run
```

The API will start on `https://localhost:5001` or `http://localhost:5000` (depending on configuration).

### Swagger Documentation
In development mode, Swagger UI is available at the application root:
- **URL:** `http://localhost:5000/` (development) or `https://localhost:5001/` (production)

## Architecture

The project follows a layered architecture:
- **ZooAssignment.Api** - HTTP endpoints and request handling
- **ZooAssignment.BusinessLayer** - Business logic and service implementations
- **ZooAssignment.DataAccessLayer** - Data access and entity models

## Data Models

### Supported Animal Types
- **Carnivores** - Meat-only diet
- **Herbivores** - Plant-only diet
- **Omnivores** - Mixed diet (meat and fruit)

### Cost Calculation
Daily feeding costs are calculated based on:
1. Animal weight
2. Daily food requirement (in kg)
3. Food type pricing (per kg)

## Testing

The project includes comprehensive unit tests:
```powershell
dotnet test
```

All tests are located in the `tests/ZooAssignment.Tests` folder with 100% pass rate.
