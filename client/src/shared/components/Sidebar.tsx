// الشريط الجانبيّ: التنقّل الرئيسيّ في التطبيق

import { NavLink } from "react-router-dom";

// عناصر التنقّل، قائمةً ليسهل تعديلها
const navItems = [
  { to: "/dashboard", label: "Dashboard" },
  { to: "/projects", label: "Projects" },
  { to: "/documents", label: "Documents" },
  { to: "/users", label: "Users" },
  { to: "/audit-log", label: "Audit Log" },
];

export function Sidebar() {
  return (
    <aside className="w-64 h-screen bg-[var(--color-bg-surface)] border-l border-[var(--color-border-subtle)] flex flex-col">
      {/* رأس الشريط: الشعار واسم المستأجر */}
      <div className="p-6 border-b border-[var(--color-border-subtle)]">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-lg bg-[var(--color-accent)] flex items-center justify-center font-bold text-white">
            BF
          </div>
          <div>
            <p className="font-bold text-[var(--color-text-primary)]">
              BuildFlow
            </p>
            <p className="text-xs text-[var(--color-text-secondary)]">
              WORKSPACE
            </p>
          </div>
        </div>
      </div>

      {/* قائمة التنقّل */}
      <nav className="flex-1 p-4 space-y-1">
        {navItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) =>
              `block px-4 py-2.5 rounded-lg text-sm transition ${
                isActive
                  ? "bg-[var(--color-bg-elevated)] text-[var(--color-text-primary)] font-medium"
                  : "text-[var(--color-text-secondary)] hover:bg-[var(--color-bg-elevated)]"
              }`
            }
          >
            {item.label}
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}