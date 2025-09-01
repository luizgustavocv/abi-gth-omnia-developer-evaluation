# Developer Evaluation Project

The DeveloperStore.Sales API was built to handle sales records within the DeveloperStore ecosystem, applying Domain-Driven Design (DDD) practices.

## Technologies

- .NET 8.0
- C#
- Docker
- Docker Compose
- PostgreSQL

## Requirements

Before running the application, make sure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

## Running the Project

1. Clone the repository:
   ```bash
   git clone https://github.com/luizgustavocv/abi-gth-omnia-developer-evaluation
   cd abi-gth-omnia-developer-evaluation
   ```

2. Start the application with Docker Compose:
   ```bash
   docker-compose up --build
   ```
3. Open the Swagger documentation in your browser:
   ```bash
   http://localhost:8080/swagger
   ```

## Testing

You can try out the available endpoints directly through Swagger.

## Sales Endpoints

For all endpoints, use Content-Type: application/json.

#### Create
```http
POST /api/Sales

{
  "customerId": "a73196b7-33a0-4d66-850f-64de6d9bf679",
  "customerName": "John Doe",
  "branchId": "9904ab36-121b-4844-96a3-f9df3e85ee8a",
  "branchName": "Main",
  "items": [
    {
      "productId": "05cb7ddb-e4f8-4095-a023-24e182f1ad34",
      "productName": "Beer",
      "unitPrice": 9.99,
      "quantity": 10
    },
	{
      "productId": "9528eade-18b3-4371-aaba-b66ae3ee87ab",
      "productName": "Wine",
      "unitPrice": 100,
      "quantity": 1
    }
  ]
}
```

#### Get
```http
GET /api/Sales/{id}
```

#### Update
```http
PUT /api/Sales/{id}

{
  "id": "d637eaa9-f1cc-4a90-b75e-a954e673671b",
  "itemsToAdd": [
    {
      "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "productName": "Water",
      "unitPrice": 5,
      "quantity": 2
    }
  ],
  "itemsToUpdate": [
    {
      "productId": "05cb7ddb-e4f8-4095-a023-24e182f1ad34",
      "quantity": 20
    }
  ],
  "productIdsToRemove": [
    "9528eade-18b3-4371-aaba-b66ae3ee87ab"
  ]
}
```

#### Cancel
```http
POST /api/Sales/{id}/cancel

{
  "reason": "Customer changed mind"
}
```

#### Delete
```http
DELETE /api/Sales/{id}
```