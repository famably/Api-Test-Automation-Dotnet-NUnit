# API Testing Framework with Embedded Mock Server

## Overview
This repository provides a fully self-contained API testing framework built with **C# (.NET 8)**.  
It includes an automated test suite and an embedded **mock API server** powered by ASP.NET Core Minimal API, enabling reliable and repeatable API testing without relying on external services or network connectivity.

The framework is designed as a reusable foundation for teams building API automation pipelines, CI-driven contract testing, or local development tools that need predictable API responses.

---

## Features

- **Self-hosted Mock API Server**  
  The project spins up an in-process ASP.NET Core Minimal API that emulates typical REST endpoints.  
  This allows the test suite to run consistently in any environment—locally, CI pipelines, containers, or restricted networks.

- **End-to-End API Test Coverage**  
  Automated tests validate CRUD-style operations using **NUnit**, **RestSharp**, and **FluentAssertions**.

- **Independent from External APIs**  
  All API calls run against `http://localhost:5055`, making the framework reliable, deterministic, and offline-capable.

- **Production-Oriented Architecture**  
  The structure mirrors real-world API automation setups, making it easy to extend with additional endpoints, scenarios, or data.

---

## Tech Stack

| Component | Technology |
|----------|------------|
| Language | C# (.NET 8) |
| Test Framework | NUnit |
| HTTP Client | RestSharp |
| Assertion Library | FluentAssertions |
| Mock API Hosting | ASP.NET Core Minimal API |
| SDK | `Microsoft.NET.Sdk.Web` |

---

## Architecture

The framework adopts a clean separation between **test execution** and **API hosting**:

1. **Mock Server Initialization**  
   The test suite boots an in-process Minimal API app using `WebApplication.CreateBuilder()`.

2. **Route Simulation**  
   The mock server defines structured endpoints including:
   - `GET /api/users`
   - `GET /api/users/{id}`
   - `POST /api/users`
   - `PUT /api/users/{id}`
   - `DELETE /api/users/{id}`

3. **Predictable JSON Responses**  
   Each endpoint returns deterministic JSON objects, ideal for verification and regression testing.

4. **Automated API Tests**  
   Tests use RestSharp to execute requests and assert behavior with FluentAssertions.

5. **Graceful Shutdown**  
   When testing completes, the mock server terminates cleanly.

---

## Scenarios Included

| # | Description | Method | Endpoint | Expected Status |
|---|-------------|---------|----------|-----------------|
| 1 | Retrieve a list of users | GET | `/api/users` | 200 |
| 2 | Retrieve a single user | GET | `/api/users/2` | 200 |
| 3 | Create a user | POST | `/api/users` | 201 |
| 4 | Update a user | PUT | `/api/users/2` | 200 |
| 5 | Delete a user | DELETE | `/api/users/2` | 204 |

---

## Getting Started

### Install .NET 8 SDK
Download from:  
https://dotnet.microsoft.com/en-us/download/dotnet/8.0

Verify installation:
```bash
dotnet --version
```

### Restore Dependencies
```bash
dotnet restore
```

### Execute the Test Suite
```bash
dotnet test
```

Expected output:
```bash
Build succeeded.
Test run for ApiTests.dll (.NET 8.0)
Total tests: 5
Passed: 5
Test Run Successful.
```

---

## Extensibility

This framework is designed to scale as projects grow:

- Additional REST endpoints  
- More complex payloads  
- Negative/error scenarios  
- Authentication workflows  
- Contract testing  
- CI/CD integration  
- Docker-based test execution  

---

## Summary

This repository demonstrates a production-ready approach to API test automation using a fully isolated and self-contained architecture.  
It is reliable, maintainable, and suitable for real-world engineering teams that need predictable and repeatable API validation.
