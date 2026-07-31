// الشريط الجانبيّ: التنقّل الرئيسيّ في التطبيق

import { NavLink } from "react-router-dom";
import { useAuth } from "../../features/auth/useAuth";

// عناصر التنقّل، قائمةً ليسهل تعديلها
const navItems = [
  { to: "/dashboard", label: "Dashboard" },
  { to: "/projects", label: "Projects" },
  { to: "/documents", label: "Documents" },
  { to: "/users", label: "Users" },
  { to: "/audit-log", label: "Audit Log" },
];

export function Sidebar() {
  const { user, logout } = useAuth();
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
            <p className="text-xs text-[var(--color-text-secondary)] uppercase">
              {user?.tenantSlug ?? "workspace"}
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
      {/* ── بطاقة المستخدم أسفل الشريط ── */}
      <div className="p-4 border-t border-[var(--color-border-subtle)]">
        <div className="flex items-center gap-3">
          {/* دائرة بحرف الاسم الأوّل */}
          <div className="w-9 h-9 rounded-full bg-[var(--color-purple)] flex items-center justify-center text-white font-medium text-sm">
            {user?.fullName?.charAt(0)?.toUpperCase() ?? "?"}
          </div>

          {/* الاسم والدور */}
          <div className="flex-1 min-w-0">
            <p className="text-sm font-medium text-[var(--color-text-primary)] truncate">
              {user?.fullName ?? "User"}
            </p>
            <p className="text-xs text-[var(--color-text-secondary)]">
              {user?.role ?? ""}
            </p>
          </div>

          {/* زرّ الخروج */}
          <button
            onClick={logout}
            title="Sign out"
            className="text-[var(--color-text-secondary)] hover:text-[var(--color-text-primary)] transition"
          >
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
              <polyline points="16 17 21 12 16 7" />
              <line x1="21" x2="9" y1="12" y2="12" />
            </svg>
          </button>
        </div>
      </div>
    </aside>
  );
}