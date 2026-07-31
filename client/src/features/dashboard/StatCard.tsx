// بطاقة إحصاء: تعرض رقماً وعنواناً وأيقونة ملوّنة
// مكوّن واحد نعيد استعماله لكل البطاقات الأربع

import type { ReactNode } from "react";

// ما تستقبله البطاقة
interface StatCardProps {
  icon: ReactNode;        // الأيقونة
  value: number;          // الرقم الكبير
  label: string;          // العنوان تحته
  hint?: string;          // سطر وصفيّ اختياريّ
  accentColor: string;    // لون خلفية الأيقونة
}

export function StatCard({
  icon,
  value,
  label,
  hint,
  accentColor,
}: StatCardProps) {
  return (
    <div className="bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-2xl p-6">
      {/* الأيقونة في مربّع ملوّن */}
      <div
        className="w-12 h-12 rounded-xl flex items-center justify-center mb-4"
        style={{ backgroundColor: accentColor }}
      >
        {icon}
      </div>

      {/* الرقم الكبير */}
      <p className="text-3xl font-bold text-[var(--color-text-primary)]">
        {value}
      </p>

      {/* العنوان */}
      <p className="text-sm text-[var(--color-text-secondary)] mt-1">
        {label}
      </p>

      {/* السطر الوصفيّ، إن وُجد */}
      {hint && (
        <p className="text-xs text-[var(--color-text-secondary)] mt-2">
          {hint}
        </p>
      )}
    </div>
  );
}