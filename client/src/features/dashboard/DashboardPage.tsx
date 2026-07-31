// صفحة لوحة المعلومات المؤقّتة، لاختبار الإطار
// سنبنيها كاملةً لاحقاً

import { AppLayout } from "../../shared/components/AppLayout";

export function DashboardPage() {
  return (
    // نلفّ الصفحة بالتخطيط، ونمرّر عنوانها
    <AppLayout title="Dashboard">
      <h2 className="text-2xl font-bold text-[var(--color-text-primary)]">
        Welcome back 👋
      </h2>
      <p className="text-[var(--color-text-secondary)] mt-2">
        Here's what's happening across your projects today.
      </p>
    </AppLayout>
  );
}