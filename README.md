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
- **Domain-Driven Design** — rich domain model with aggregates, value objects, and domain events.
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
├── SharedKernel/                    # Domain primitives shared across modules
│   └── Domain/
│       ├── Entity.cs                # Generic, identity-based equality
│       ├── AggregateRoot.cs         # Consistency boundary + domain events
│       ├── ValueObject.cs           # Structural equality
│       ├── IDomainEvent.cs          # Event contract (EventId, OccurredOnUtc)
│       └── Auditing/
│           ├── IAuditableEntity.cs  # Created/Modified tracking
│           └── ISoftDelete.cs       # Soft-delete contract
│
└── BuildingBlocks/
    └── Application.Abstractions/    # CQRS contracts (depends on SharedKernel)
        ├── ICommand.cs
        ├── ICommandHandler.cs
        ├── IQuery.cs
        ├── IQueryHandler.cs
        └── PagedResult.cs
```

---

## Key Design Decisions

- **Strongly-typed IDs** over raw `Guid` to eliminate primitive obsession and catch argument mix-ups at compile time.
- **Result pattern** (FluentResults) for expected failures; exceptions reserved for the truly exceptional.
- **Domain events** dispatched after a successful save, so events are never published for changes that failed to persist.
- **UTC everywhere** — all timestamps stored in UTC, converted only at the presentation boundary.
- **Reproducible builds** — SDK version pinned via `global.json`.

---

## Build Prerequisites

- .NET SDK **8.0.x** (pinned in `global.json`)
- SQL Server (for later phases)

```bash
dotnet build
```

---

## Roadmap

- [x] **Phase 1** — Solution structure, SharedKernel, Application abstractions
- [ ] **Phase 2** — Identity domain
- [ ] **Phase 3** — Identity application (CQRS handlers)
- [ ] **Phase 4** — Identity infrastructure (EF Core)
- [ ] **Phase 5** — API layer
- [ ] **Phase 6** — Projects module
- [ ] **Phase 7** — Documents module (review workflow)
- [ ] **Phase 8** — MediatR pipeline (validation + logging)
- [ ] **Phase 9** — Docker, CI/CD, documentation

---

## License

MIT