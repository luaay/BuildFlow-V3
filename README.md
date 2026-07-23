# BuildFlow-V3
[![CI](https://github.com/luaay/BuildFlow-V3/actions/workflows/ci.yml/badge.svg)](https://github.com/luaay/BuildFlow-V3/actions/workflows/ci.yml)

**Engineering Document Workflow SaaS** — a multi-tenant platform for engineering offices and contracting firms to manage documents through a structured review and approval lifecycle.

Built as a portfolio project to demonstrate professional .NET architecture: Modular Monolith, Domain-Driven Design, Clean Architecture, and CQRS.

> 🎥 **Video series:** This codebase is taught step by step on YouTube — [BuildVision Eng: Luay](https://www.youtube.com/@BuildVisionEngLuay), playlist [Enterprise API Development](https://www.youtube.com/watch?v=BtDY8nP7s-s&list=PLvQd5bL8gZSsOxYZfdleDcERevGIcvYbm) (currently in Arabic; an English series is planned).

---

## Tech Stack

**Backend:** ASP.NET Core 8 · Entity Framework Core 8 · SQL Server · MediatR (CQRS) · FluentValidation · FluentResults · Serilog · JWT · BCrypt
**Frontend:** React 18 · TypeScript · Vite · Tailwind CSS · Zustand · TanStack Query · React Hook Form · Axios
**Testing:** xUnit · FluentAssertions · NSubstitute
**DevOps:** Docker · Docker Compose · GitHub Actions

---

## Architecture

- **Modular Monolith** — strict module boundaries inside a single deployable.
- **Domain-Driven Design** — rich aggregates, value objects, and domain events.
- **Clean Architecture** — dependencies point inward; boundaries enforced at the project-reference level.
- **CQRS** — commands and queries separated, organized as vertical slices.
- **Multi-Tenancy** — tenant isolation driven by claims from the JWT, enforced down to the database with composite unique indexes.

### Modules

1. **Identity** — Tenants, Users, Roles, JWT authentication.
2. **Projects** — CRUD, status lifecycle, team members.
3. **Documents** — full review workflow: Draft → Review → Approved / Rejected → Superseded.

---

## Running with Docker

The entire system — API and database — runs with a single command.

**Prerequisites:** Docker Desktop running.

1. Create a `.env` file in the repository root:

DB_PASSWORD=your_strong_password_here
JWT_SECRET=your_long_secret_signing_key_here

2. Start everything:

```bash
docker compose up -d --build
```

The API listens on `http://localhost:8080`. The database schema is created automatically on startup.

To stop and remove the containers:

```bash
docker compose down
```

> Note: `.env` is git-ignored and never committed. Secrets stay local.

---

## Solution Structure

```text
src/
├── SharedKernel/                        # Domain primitives (Entity, AggregateRoot, ValueObject, DomainEvent, AppError, auditing)
│
├── BuildingBlocks/
│   └── Application.Abstractions/        # CQRS contracts (ICommand, IQuery, handlers, PagedResult)
│
├── Modules/
│   └── Identity/
│       ├── BuildFlow.Identity.Domain/        # Aggregates, value objects, events, repositories, errors
│       ├── BuildFlow.Identity.Application/    # Use cases as vertical slices, abstractions, event handlers
│       └── BuildFlow.Identity.Infrastructure/ # EF Core, value converters, repositories, Unit of Work, BCrypt, JWT, migrations
│
└── BuildFlow.Api/                       # API host — composition root, Minimal API endpoints, auth, error translation, Swagger

tests/
└── BuildFlow.Identity.Domain.UnitTests/ # Unit tests for the Identity domain
```

---

## Key Design Decisions

- **Strongly-typed IDs** over raw `Guid`, persisted via EF Core value converters.
- **Value objects** (`Email`) so validation happens once and illegal states are unrepresentable.
- **Rich domain model** — entities own their behavior and invariants (account lockout, suspension).
- **Aggregates reference each other by ID**, never by object reference.
- **CQRS with vertical slices** — each use case bundles its command, validator, and handler.
- **Result pattern** (FluentResults) for expected failures; exceptions for the truly exceptional.
- **Unit of Work** wraps EF Core's DbContext and dispatches domain events only after a successful save.
- **Soft delete** via a global query filter; **per-tenant email uniqueness** via a composite unique index.
- **Tenant isolation** — handlers derive the tenant from `ICurrentUserService`, never the request.
- **Security** — BCrypt password hashing (work factor 12), signed JWTs carrying the tenant, generic "invalid credentials" to prevent enumeration, temporary account lockout.
- **Reproducible builds** — SDK pinned via `global.json`, EF tools pinned as a local tool.

---

## API Layer (Identity Module)

The API host (`BuildFlow.Api`) is the composition root: the only project that wires all layers together. It exposes the Identity module over HTTP using **Minimal APIs**, one endpoint class per vertical slice (REPR pattern).

### Endpoints

| Method | Route                   | Auth      | Purpose                                    |
|--------|-------------------------|-----------|--------------------------------------------|
| POST   | `/api/tenants/register` | Anonymous | Create a tenant and its owner user         |
| POST   | `/api/auth/login`       | Anonymous | Authenticate and issue a JWT               |
| GET    | `/api/users`            | Bearer    | List users in the caller's tenant (paged)  |
| POST   | `/api/users/invite`     | Bearer    | Invite a new user into the caller's tenant |

### Authentication

- JWT bearer authentication; validation parameters bound to a single `Jwt` options source shared with the token provider.
- Inbound claim mapping disabled (`MapInboundClaims = false`) so claim names are preserved as issued; `NameClaimType` and `RoleClaimType` set explicitly.
- `ClockSkew` set to zero for exact expiry.
- `ICurrentUserService` reads the `sub` and `tenant` claims from `HttpContext` and exposes strongly-typed `UserId` and `TenantId`.

### Multi-tenancy enforcement

Protected endpoints never accept a tenant parameter. The tenant is read from the authenticated token, making cross-tenant access structurally impossible rather than relying on convention.

### Error handling

Failed `Result`s are translated centrally to **RFC 7807 ProblemDetails**, mapping by `AppError` code with suffix matching (for example `*.NotFound` maps to 404 and `*.AlreadyExists` maps to 409). The stable error code is attached as a ProblemDetails extension so callers branch on the code, not the message.

### Configuration & secrets

- Secrets (connection string, JWT signing key) are stored in **user-secrets** in development; non-secret values such as token expiry live in `appsettings.json`.
- The connection string key is module-scoped: `ConnectionStrings:IdentityDb`.

### Logging

Structured logging via **Serilog**: a bootstrap logger for startup, sinks and levels read from `appsettings.json`, Console plus a daily rolling File, and request logging.

### API documentation

Swagger UI is available in Development at `/swagger`, with a Bearer security definition so protected endpoints can be tested directly from the browser.

### Run locally

```powershell
dotnet run --project src\BuildFlow.Api\BuildFlow.Api.csproj --launch-profile https
```

Then open `https://localhost:7124/swagger`.

---

## Testing

The domain layer is covered by fast, dependency-free unit tests (xUnit + FluentAssertions). Because the domain has no external dependencies, tests run without a database or mocks, and they double as living documentation of the business rules.

Covered so far:
- **Email** value object — validation, normalization, structural equality.
- **Tenant** aggregate — factory, domain events, suspend/activate, idempotency.
- **User** aggregate — factory, role changes, and the full account-lockout lifecycle.

Broader unit and integration testing (repositories and full-path tests via Testcontainers and WebApplicationFactory) are consolidated in Phase 10, once all modules exist.

```bash
dotnet test
```

---

## Build & Database

```bash
dotnet build
```

Requires .NET SDK **8.0.x** (pinned in `global.json`) and SQL Server.

Apply the database schema with the pinned local EF tool:

```bash
dotnet tool restore
dotnet ef database update --project src/Modules/Identity/BuildFlow.Identity.Infrastructure --startup-project src/BuildFlow.Api
```

---

## Roadmap

- [x] **Phase 1** — Solution structure, SharedKernel, Application abstractions
- [x] **Phase 2** — Identity domain (aggregates, value objects, events, repositories, errors) + domain unit tests
- [x] **Phase 3** — Identity application (CQRS vertical slices, event handlers, DI)
- [x] **Phase 4** — Identity infrastructure (EF Core, value converters, repositories, Unit of Work, BCrypt, JWT, initial migration)
- [x] **Phase 5** — API layer (Minimal APIs, JWT auth, current-user service, central error translation, Serilog, Swagger)
- [x] **Phase 6** — Projects module: domain (+18 unit tests), application (7 vertical slices), infrastructure (EF Core, migration applied), and API (all 7 endpoints: create, list, get-by-id, update, change-status, add-member, remove-member). Verified end-to-end via Swagger, including lifecycle transition guards and the last-lead invariant.
- [x] **Phase 7** — Documents module: review workflow (draft → under review → approved/rejected → archived), single assigned reviewer enforced in the aggregate, multi-version documents, and modification guards protecting review integrity.
- [x] **Phase 8** — MediatR pipeline: logging, validation (short-circuits invalid commands before the handler), and performance behaviors, registered per module.
- [ ] **Phase 9** — Docker, CI/CD, documentation
- [ ] **Phase 10** — Integration testing (Testcontainers + WebApplicationFactory) across all modules
- [ ] **Phase 11** — OpenTelemetry (distributed tracing)
- [ ] **Phase 12** — Redis caching (optional)
- [ ] **Phase 13** — Azure deployment (optional)

### Cross-cutting improvements (layered on top of the phases above)

- [x] **Structured logging** — request-context enrichment middleware pushing UserId and TenantId into the Serilog LogContext; machine/thread enrichers; console and rolling-file sinks with property output.
- [x] **Expanded unit tests** — Identity domain (25 tests) and Projects domain (44 tests: factory, full lifecycle, member rules incl. last-lead on both paths, and direct Money/ProjectCode value-object tests).
- [x] **Integration testing** — full-path tests against a real SQL Server via Testcontainers + WebApplicationFactory: tenant registration persistence, the complete register→login→access-protected authentication flow, and cross-tenant isolation.
- [x] **Containerization** — multi-stage Dockerfile and docker-compose running the API alongside SQL Server; secrets supplied through environment variables, schema created at startup. The whole system runs with a single command.
- [x] **CI pipeline** — GitHub Actions workflow building the solution in Release and running all domain unit tests on every push and pull request.
---

## Video Series

This project is built and explained step by step on YouTube, focusing on architectural decision-making rather than syntax alone.

- **Channel:** [BuildVision Eng: Luay](https://www.youtube.com/@BuildVisionEngLuay)
- **Playlist:** [Enterprise API Development](https://www.youtube.com/watch?v=BtDY8nP7s-s&list=PLvQd5bL8gZSsOxYZfdleDcERevGIcvYbm)

The current series is in Arabic; an English series is planned.

---

## License

MIT
