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
- **CQRS** — commands (writes) and queries (reads) separated into distinct models.
- **Multi-Tenancy** — tenant isolation driven by claims from the JWT.

### Modules

1. **Identity** — Tenants, Users, Roles, JWT authentication.
2. **Projects** — CRUD, status lifecycle, team members.
3. **Documents** — full review workflow: Draft → Review → Approved / Rejected → Superseded.

---

## Solution Structure

```text
src/
├── SharedKernel/                        # Domain primitives shared across modules
│   └── Domain/
│       ├── Entity.cs                    # Generic, identity-based equality
│       ├── AggregateRoot.cs             # Consistency boundary + domain events
│       ├── ValueObject.cs               # Structural equality
│       ├── IDomainEvent.cs / DomainEvent.cs
│       ├── AppError.cs                  # Structured error with code
│       └── Auditing/ (IAuditableEntity, ISoftDelete)
│
├── BuildingBlocks/
│   └── Application.Abstractions/        # CQRS contracts (ICommand, IQuery, handlers, PagedResult)
│
└── Modules/
    └── Identity/
        └── BuildFlow.Identity.Domain/
            ├── Tenants/                 # Tenant aggregate, TenantId, repository, enums, events
            ├── Users/                   # User aggregate, UserId, Email VO, repository, enums, events
            └── Errors/                  # IdentityErrors (coded, security-aware)
```

---

## Key Design Decisions

- **Strongly-typed IDs** (`TenantId`, `UserId`) over raw `Guid` to eliminate primitive obsession.
- **Value objects** (`Email`) so validation happens once and illegal states are unrepresentable.
- **Rich domain model** — entities own their behavior and invariants (e.g. account lockout logic).
- **Aggregates reference each other by ID**, never by object reference.
- **Result pattern** (FluentResults) for expected failures; exceptions for the truly exceptional.
- **Temporary account lockout** after repeated failed logins, like ASP.NET Core Identity.
- **Security-aware errors** — generic "invalid email or password" to prevent user enumeration.
- **UTC everywhere**; **reproducible builds** via `global.json`.

---

## Build

```bash
dotnet build
```

Requires .NET SDK **8.0.x** (pinned in `global.json`). SQL Server is needed for later phases.

---

## Roadmap

- [x] **Phase 1** — Solution structure, SharedKernel, Application abstractions
- [x] **Phase 2** — Identity domain (Tenant & User aggregates, value objects, events, repositories, errors)
- [ ] **Phase 3** — Identity application (CQRS handlers, validation)
- [ ] **Phase 4** — Identity infrastructure (EF Core, value converters, repository implementations)
- [ ] **Phase 5** — API layer
- [ ] **Phase 6** — Projects module
- [ ] **Phase 7** — Documents module (review workflow)
- [ ] **Phase 8** — MediatR pipeline (validation + logging)
- [ ] **Phase 9** — Docker, CI/CD, documentation

---

## License

MIT
