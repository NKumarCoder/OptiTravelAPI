# OptiTravelAPI – .NET 10 Modular Travel Consolidation API

A scalable, lightweight, and modular travel consolidation platform built on **.NET 10 LTS** and **ASP.NET Core Web API**.

Designed for deployment on **Docker** and **Vercel Container Runtime** with strict **zero-cost / free-tier** adherence.

---

## 1. Architecture Overview

OptiTravelAPI is structured as a **single deployable ASP.NET Core API application** supporting multiple travel service domains (Bus, Hotel, Flight, Cab, Transfers) with multi-supplier capability for each service.

```
                              ┌────────────────────────────────────────┐
                              │           OptiTravelAPI.Web            │
                              │ (Controllers, Middleware, Swagger, DI) │
                              └──────────────────┬─────────────────────┘
                                                 │
                                                 ▼
                              ┌────────────────────────────────────────┐
                              │  OptiTravelAPI.ConsolidationSupplier   │
                              │ (Service Gateways & Request Forwarder) │
                              └──────────────────┬─────────────────────┘
                                                 │
                                                 ▼
                              ┌────────────────────────────────────────┐
                              │     OptiTravelAPI.ConsolidationLayer   │
                              │  (Business Orchestration & Aggregation)│
                              └──────────────────┬─────────────────────┘
                                                 │
                                                 ▼
                              ┌────────────────────────────────────────┐
                              │       OptiTravelAPI.SupplierLayer      │
                              │   (Supplier Abstractions & Resolvers)  │
                              └──────────────────┬─────────────────────┘
                                                 │
                                                 ▼
                              ┌────────────────────────────────────────┐
                              │        OptiTravelAPI.Suppliers         │
                              │  (Concrete Implementations: TBO, etc.) │
                              └──────────────────┬─────────────────────┘
                                                 │
                                                 ▼
                              ┌────────────────────────────────────────┐
                              │         OptiTravelAPI.Shared           │
                              │  (Constants, Models, Exceptions, Utils)│
                              └────────────────────────────────────────┘
```

### The 4 Mandatory Architectural Layers

1. **OptiTravelAPI.ConsolidationSupplierLayer**
   - Direct recipient of requests from the public API layer.
   - Forwards calls to the appropriate consolidation service.
   - Preserves upstream and domain response contracts without unnecessary mutation.
   - Centralizes service-level communication and error propagation.

2. **OptiTravelAPI.ConsolidationLayer**
   - The core business orchestration engine for all services (Bus, Hotel, Flight, Cab, Transfers).
   - Manages multi-supplier selection, priority routing, parallel fan-out, and result aggregation.

3. **OptiTravelAPI.SupplierLayer**
   - Abstraction bridge between consolidation logic and external suppliers.
   - Defines common supplier contracts (`ISupplier`, `IBusSupplier`, `IHotelSupplier`, etc.).
   - Provides runtime supplier resolution via `ISupplierResolver` and `SupplierResolver`.

4. **OptiTravelAPI.Suppliers**
   - Houses isolated supplier implementations (e.g., `Bus/Tbo/TboBusSupplier`).
   - Completely decoupled: adding, removing, or updating a supplier has zero impact on the controllers or other suppliers.

5. **OptiTravelAPI.Shared**
   - Cross-cutting types, complete Swagger-conforming Bus domain models, standard responses (`HealthResponse`, `ApiInfoResponse`, `ErrorResponse`), custom exceptions (`SupplierException`, `ConsolidationException`), constants, and correlation utilities.

---

## 2. Bus & Content Endpoints (Phase 2)

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/bus/login` | Authenticate with supplier and return original access token. |
| `POST` | `/api/bus/search` | Search buses across trips, preserving filters, tags, and pricing. |
| `POST` | `/api/bus/select` | Retrieve seat layout, coordinates, dimensions, berths, and fares. |
| `POST` | `/api/bus/block` | Temporarily block seats across trips for passenger and POC details. |
| `POST` | `/api/bus/book` | Confirm booking using supplier reference numbers. |
| `POST` | `/api/bus/ticket` | Retrieve comprehensive ticket, passenger, and boarding details. |
| `POST` | `/api/bus/tracking` | Track real-time bus GPS coordinates, driver info, and speed. |
| `GET` | `/api/bus/GetTicketDetails` | Query ticket details by `referanceNumber`. |
| `POST` | `/api/bus/precancel` | Check cancellation policies, fees, and refund amounts prior to cancellation. |
| `POST` | `/api/bus/cancel` | Cancel booked tickets and return cancellation status and refund breakdown. |
| `GET` | `/api/bus/balance` | Query supplier account balance. |
| `GET` | `/api/cdn/cities` | Retrieve reference cities and station points from CDN. |
| `GET` | `/health` | Health check reporting service status and trace ID. |
| `GET` | `/api/info` | Platform information and supported services list. |
| `GET` | `/swagger` | Interactive Swagger UI. |

---

## 3. Configuration & Credentials

Supplier credentials and settings are driven entirely through `appsettings.json` or environment variables:

```json
{
  "Suppliers": {
    "Bus": {
      "TBO": {
        "Enabled": true,
        "Priority": 1,
        "BaseUrl": "https://api.tbo.com",
        "Username": "your-username",
        "Password": "your-password",
        "TimeoutSeconds": 60
      }
    }
  }
}
```

### Environment Variable Overrides for Production / Vercel
- `Suppliers__Bus__TBO__BaseUrl` = `https://api.tbo.com`
- `Suppliers__Bus__TBO__Username` = `your-username`
- `Suppliers__Bus__TBO__Password` = `your-password`
- `PORT` = `8080` (or injected dynamically by Vercel / Cloud Run)

---

## 4. Multi-Supplier Routing

The system defaults to `TBO` for all bus operations. To route requests to an alternative bus supplier (e.g. future `RedBus` or `Abhibus`), clients simply provide the `X-Supplier` HTTP header:

```http
POST /api/bus/search HTTP/1.1
X-Supplier: RedBus
Content-Type: application/json
```

---

## 5. Local Execution & Testing

### Restore & Build
```bash
dotnet restore
dotnet build
```

### Run All Tests (27 Unit & Integration Tests)
```bash
dotnet test
```

### Run Locally
```bash
dotnet run --project src/OptiTravelAPI.Web/OptiTravelAPI.Web.csproj
```
Visit Swagger UI at `http://localhost:8080/swagger`.

---

## 6. Docker Commands

### Standard Docker Build
```bash
docker build -t optitravelapi:latest -f Dockerfile .
```

### Standard Docker Run
```bash
docker run -d -p 8080:8080 -e PORT=8080 --name optitravelapi optitravelapi:latest
```

---

## 7. Vercel Container Runtime Compatibility

- **Stateless**: Zero local disk dependencies, zero required paid databases, zero Redis/RabbitMQ requirements.
- **Port Binding**: Respects `$PORT` environment variable provided by Vercel container runtime.
- **Memory Footprint**: Base memory is ~50 MB, well within the 1024 MB free tier allowance.
- **Known Free-Tier Limit**: Vercel Hobby serverless container executions timeout at 15 seconds. Upstream TBO booking operations taking longer than 15s may hit platform timeout; in such scenarios, configure supplier timeout accordingly or consider Vercel Pro/background jobs if asynchronous callback patterns are needed.
