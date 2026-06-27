# BuildFlow-V3

**Engineering Document Workflow SaaS** — a multi-tenant platform for engineering offices and contracting firms to manage documents through a structured review and approval lifecycle.

Built as a portfolio project to demonstrate professional .NET architecture: Modular Monolith, Domain-Driven Design, Clean Architecture, and CQRS.

---

## Tech Stack

**Backend:** ASP.NET Core 8 · Entity Framework Core 8 · SQL Server · MediatR (CQRS) · FluentValidation · FluentResults · Serilog · JWT
**Frontend:** React 18 · TypeScript · Vite · Tailwind CSS · Zustand · TanStack Query · React Hook Form · Axios
**Testing:** xUnit · FluentAssertions · NSubstitute
**DevOps:** Docker · Docker Compose · GitHub Actions

---

## Architecture

- **Modular Monolith** — strict module boundaries inside a single deployable.
- **Domain-Driven Design** — rich aggregates, value objects, and domain events.
- **Clean Architecture** — dependencies point inward; boundaries enforced at the project-reference level.
- **CQRS** — commands (writes) and queries (reads) separated, organized as vertical slices.
- **Multi-Tenancy** — tenant isolation driven by claims from the JWT.

### Modules

1. **Identity** — Tenants, Users, Roles, JWT authentication.
2. **Projects** — CRUD, status lifecycle, team members.
3. **Documents** — full review workflow: Draft → Review → Approved / Rejected → Superseded.

---

## Solution Structure

```text
src/
├── SharedKernel/                        # Domain primitives (Entity, AggregateRoot, ValueObject, DomainEvent, AppError, auditing)
│
├── BuildingBlocks/
│   └── Application.Abstractions/        # CQRS contracts (ICommand, IQuery, handlers, PagedResult)
│
└── Modules/
    └── Identity/
        ├── BuildFlow.Identity.Domain/        # Tenant & User aggregates, value objects, events, repositories, errors
        └── BuildFlow.Identity.Application/    # Use cases as vertical slices
            ├── Abstractions/                  # ICurrentUserService, IPasswordHasher, IJwtProvider, IUnitOfWork
            ├── EventHandlers/                 # Reactions to domain events
            ├── Features/
            │   ├── Auth/Login/                # Login with account lockout
            │   ├── Tenants/RegisterTenant/    # Register tenant + owner (atomic)
            │   └── Users/ (InviteUser, GetUsers)
            └── DependencyInjection.cs

tests/
└── BuildFlow.Identity.Domain.UnitTests/ # Unit tests for the Identity domain
```

---

## Key Design Decisions

- **Strongly-typed IDs** over raw `Guid` to eliminate primitive obsession.
- **Value objects** (`Email`) so validation happens once and illegal states are unrepresentable.
- **Rich domain model** — entities own their behavior and invariants (account lockout, suspension).
- **Aggregates reference each other by ID**, never by object reference.
- **CQRS with vertical slices** — each use case bundles its command, validator, and handler.
- **Result pattern** (FluentResults) for expected failures; exceptions for the truly exceptional.
- **Unit of Work** wraps EF Core's DbContext behind an abstraction for atomic transactions.
- **Tenant isolation** — handlers derive the tenant from `ICurrentUserService`, never the request.
- **Security-aware** — generic "invalid credentials" to prevent enumeration, temporary lockout, password hashing behind an abstraction.
- **DTOs** so the domain (e.g. password hash) never leaks through the API.

---

## Testing

The domain layer is covered by fast, dependency-free unit tests (xUnit + FluentAssertions). Because the domain has no external dependencies, tests run without a database or mocks, and they double as living documentation of the business rules.

Covered so far:
- **Email** value object — validation, normalization, structural equality.
- **Tenant** aggregate — factory, domain events, suspend/activate, idempotency.
- **User** aggregate — factory, role changes, and the full account-lockout lifecycle.

```bash
dotnet test
```

---

## Build

```bash
dotnet build
```

Requires .NET SDK **8.0.x** (pinned in `global.json`). SQL Server is needed for later phases.

---

## Roadmap

- [x] **Phase 1** — Solution structure, SharedKernel, Application abstractions
- [x] **Phase 2** — Identity domain (aggregates, value objects, events, repositories, errors) + domain unit tests
- [x] **Phase 3** — Identity application (CQRS vertical slices: RegisterTenant, Login, InviteUser, GetUsers; event handlers; DI)
- [ ] **Phase 4** — Identity infrastructure (EF Core, value converters, repository & Unit of Work implementations, hashing, JWT)
- [ ] **Phase 5** — API layer
- [ ] **Phase 6** — Projects module
- [ ] **Phase 7** — Documents module (review workflow)
- [ ] **Phase 8** — MediatR pipeline (validation + logging)
- [ ] **Phase 9** — Docker, CI/CD, documentation

---

## License

MIT
