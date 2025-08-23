# LibraryManagement

A full-stack library management system built with .NET Core Web API and React TypeScript.

## Architecture

- **Backend**: .NET 9 Web API with Clean Architecture (Domain, Application, Infrastructure, API layers)
- **Frontend**: React TypeScript with Vite, React Router, and Tailwind CSS
- **Database**: SQLite with Entity Framework Core
- **Deployment**: Docker support with docker-compose

## Prerequisites

- [Node.js](https://nodejs.org/) (v18 or higher)
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (optional)

## Getting Started

### 1. Server Setup

You can run the server in two ways:

#### Option A: Direct .NET Development (Recommended)

```bash
cd server
dotnet restore
dotnet run --project src/LibraryManagement.API
```

The API will be available at `http://localhost:5105`

#### Option B: Using Docker

```bash
cd server
docker-compose up --build
```

The API will be available at `http://localhost:5000`

### 2. Client Setup

```bash
cd client
npm install
```

#### Configure API Base URL (Optional)

The client comes with a `.env` file pre-configured for Direct .NET (port 5105).

**If using Docker**, edit `client/.env` and switch the commented lines:
```
# Comment out the Direct .NET line and uncomment the Docker line
# VITE_API_BASE_URL=http://localhost:5105/api
VITE_API_BASE_URL=http://localhost:5000/api
```

#### Start the Development Server

```bash
npm run dev
```

The client will be available at `http://localhost:5173`

## API Endpoints

The API provides the following main endpoints:

- `GET /api/books` - Get all books
- `POST /api/books` - Create a new book
- `GET /api/books/{id}` - Get book by ID
- `GET /api/books/search?searchTerm={term}` - Search books

- `GET /api/members` - Get all members
- `POST /api/members` - Register a new member
- `GET /api/members/{id}` - Get member by ID

- `POST /api/borrowingtransactions/borrow` - Borrow a book
- `POST /api/borrowingtransactions/return/{transactionId}` - Return a book

## Development

### Running Tests

```bash
# Backend tests
cd server
dotnet test

# Frontend tests (if available)
cd client
npm test
```

## Port Configuration

| Service | Docker | Direct .NET | Client |
| ------- | ------ | ----------- | ------ |
| API     | 5000   | 5105        | -      |
| Client  | -      | -           | 5173   |

## Troubleshooting

### Database Issues

The SQLite database is automatically created when the server starts. If you need to reset it:

```bash
cd server
rm -rf data/library.db*
dotnet run --project src/LibraryManagement.API
```
