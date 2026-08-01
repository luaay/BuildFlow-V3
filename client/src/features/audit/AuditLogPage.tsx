// صفحة سجلّ التدقيق، مؤقّتة
// النظام الكامل يُبنى لاحقاً: خلفيةً ثم واجهة

import { AppLayout } from "../../shared/components/AppLayout";

export function AuditLogPage() {
  return (
    <AppLayout title="Audit Log">
      <div className="flex flex-col items-center justify-center py-24 text-center">
        {/* أيقونة */}
        <div className="w-16 h-16 rounded-2xl bg-[var(--color-bg-elevated)] flex items-center justify-center mb-4">
          <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="var(--color-text-secondary)" strokeWidth="2">
            <path d="M3 3v18h18" />
            <path d="m7 12 4-4 4 4 4-4" />
          </svg>
        </div>
        <h2 className="text-xl font-semibold text-[var(--color-text-primary)]">
          Audit Log
        </h2>
        <p className="text-[var(--color-text-secondary)] mt-2 max-w-md">
          A full activity audit trail is coming soon.
        </p>
      </div>
    </AppLayout>
  );
}