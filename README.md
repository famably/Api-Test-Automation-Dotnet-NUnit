# API Test Automation (Self-Contained Mock Setup)

This project implements automated API tests for the **ReqRes API** scenarios described in the Abel & Cole technical assessment.  
It is written in **C# (.NET 8)** using **NUnit**, **RestSharp**, and **FluentAssertions**, and includes a built-in **mock API server** implemented with **ASP.NET Core Minimal API**.

The mock server replicates the `/api/users` endpoints from `https://reqres.in`, allowing the entire test suite to run **offline** and **independently of network or proxy restrictions**.

---

## 🧰 Tech Stack

| Component | Technology |
|------------|-------------|
| Language | C# (.NET 8) |
| Test Framework | NUnit |
| HTTP Client | RestSharp |
| Assertions | FluentAssertions |
| Hosting / Mock API | ASP.NET Core Minimal API |
| SDK | `Microsoft.NET.Sdk.Web` |

---


---

## 🧩 Why `Microsoft.NET.Sdk.Web` is used

The test project uses the **Web SDK** instead of the regular Class Library SDK so that it can host an in-process ASP.NET Core mock server using `WebApplication.CreateBuilder()`.  
This makes the suite fully self-contained—no need for a separate API or an internet connection.  
All requests are served locally at `http://localhost:5055` during test execution.

---

## 🚀 How It Works

1. **One-time setup**: before any tests run, the suite spins up a lightweight mock server using `WebApplication`.
2. The server defines endpoints identical to the original ReqRes ones:
   - `GET /api/users`
   - `GET /api/users/{id}`
   - `POST /api/users`
   - `PUT /api/users/{id}`
   - `DELETE /api/users/{id}`
3. Each test case uses **RestSharp** to call these endpoints locally.
4. The server returns predictable JSON responses for verification.
5. After all tests complete, the mock server shuts down automatically.

---

## 🧾 Scenarios Covered

| # | Scenario | Endpoint | Method | Expected Status |
|---|-----------|-----------|--------|-----------------|
| 1 | Get list of users | `/api/users` | GET | 200 |
| 2 | Get single user | `/api/users/2` | GET | 200 |
| 3 | Create user | `/api/users` | POST | 201 |
| 4 | Update user | `/api/users/2` | PUT | 200 |
| 5 | Delete user | `/api/users/2` | DELETE | 204 |

---

## 🖥️ Environment Setup

### 1️⃣ Install .NET 8 SDK
Download and install from:  
👉 https://dotnet.microsoft.com/en-us/download/dotnet/8.0  

Verify:
```bash
dotnet --version
```

### 2️⃣ Restore dependencies
Run inside the project folder:
```bash
dotnet restore
```

### 3️⃣ Run the tests
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

All tests execute against the embedded mock API—no internet required.
