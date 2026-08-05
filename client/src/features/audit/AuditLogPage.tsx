// صفحة سجلّ التدقيق Audit Log: قائمة الأحداث، مع كشف التغييرات
// كل صفّ مكوّن مستقلّ يدير فتح تفاصيله

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { AppLayout } from "../../shared/components/AppLayout";
import { getAuditLog } from "./audit.api";
import type { AuditEntry } from "./audit.api";

export function AuditLogPage() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ["audit"],
    queryFn: getAuditLog,
  });

  const entries = data?.items ?? [];

  return (
    <AppLayout title="Audit Log">
      {/* رأس الصفحة */}
      <div className="mb-6">
        <h2 className="text-2xl font-bold text-[var(--color-text-primary)]">
          Audit Log
        </h2>
        <p className="text-sm text-[var(--color-text-secondary)] mt-1">
          {data?.totalCount ?? 0} total events recorded
        </p>
      </div>

      {/* الحالات */}
      {isLoading && (
        <p className="text-[var(--color-text-secondary)]">Loading audit log...</p>
      )}
      {isError && <p className="text-red-400">Failed to load audit log.</p>}

      {/* القائمة: كل صفّ مكوّن مستقلّ */}
      {!isLoading && !isError && (
        <div className="space-y-3">
          {entries.map((entry) => (
            <AuditRow key={entry.id} entry={entry} />
          ))}
        </div>
      )}

      {/* رسالة الفراغ */}
      {!isLoading && !isError && entries.length === 0 && (
        <div className="text-center py-16">
          <p className="text-[var(--color-text-secondary)]">
            No audit events yet
          </p>
        </div>
      )}
    </AppLayout>
  );
}

// ── صفّ حدث تدقيق، مع كشف التغييرات Show changes ──
function AuditRow({ entry }: { entry: AuditEntry }) {
  // هل التفاصيل مفتوحة
  const [isOpen, setIsOpen] = useState(false);

  const newFields = parseToFields(entry.newValues);
  const oldFields = parseToFields(entry.oldValues);

  return (
    <div className="bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-2xl p-4">
      <div className="flex items-start justify-between">
        {/* يسار: العملية والكيان والتفاصيل */}
        <div className="flex items-start gap-3">
          {/* أيقونة الكيان */}
          <div className="w-10 h-10 rounded-xl bg-[var(--color-bg-elevated)] flex items-center justify-center text-[var(--color-text-secondary)] mt-0.5">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
              <path d="M14 2v6h6" />
            </svg>
          </div>

          <div>
            {/* شارة العملية واسم الكيان والأعمدة المتغيّرة */}
            <div className="flex items-center gap-2">
              <ActionBadge action={entry.action} />
              <span className="font-medium text-[var(--color-text-primary)]">
                {entry.entityName}
              </span>
              {entry.changedColumns && (
                <span className="text-xs text-[var(--color-text-secondary)]">
                  {entry.changedColumns}
                </span>
              )}
            </div>

            {/* المعرّف والفاعل والعنوان */}
            <p className="text-xs text-[var(--color-text-secondary)] mt-1">
              {entry.entityId.slice(0, 8)}...
              {entry.userId && <> · by {entry.userId.slice(0, 8)}...</>}
              {entry.ipAddress && <> · IP: {entry.ipAddress}</>}
            </p>

            {/* زرّ كشف التغييرات */}
            <button
              onClick={() => setIsOpen(!isOpen)}
              className="text-xs text-[var(--color-accent)] hover:underline mt-2"
            >
              {isOpen ? "▲ Hide changes" : "▼ Show changes"}
            </button>

            {/* تفاصيل التغييرات، تظهر عند الفتح */}
{isOpen && (
              <div className="mt-3 p-4 bg-[var(--color-bg-base)] rounded-lg text-xs">
                {/* القيم القديمة، للتعديل والحذف */}
                {oldFields && oldFields.length > 0 && (
                  <div className="mb-3">
                    <p className="text-red-400 font-medium mb-1.5">Before</p>
                    <FieldTable fields={oldFields} />
                  </div>
                )}

                {/* القيم الجديدة، للإنشاء والتعديل */}
                {newFields && newFields.length > 0 && (
                  <div>
                    <p className="text-green-400 font-medium mb-1.5">After</p>
                    <FieldTable fields={newFields} />
                  </div>
                )}

                {/* لا تفاصيل */}
                {!oldFields && !newFields && (
                  <span className="text-[var(--color-text-secondary)]">
                    No detailed changes recorded.
                  </span>
                )}
              </div>
            )}
          </div>
        </div>

        {/* يمين: الوقت النسبيّ */}
        <span className="text-xs text-[var(--color-text-secondary)] whitespace-nowrap">
          {formatRelativeTime(entry.occurredAt)}
        </span>
      </div>
    </div>
  );
}

// ── شارة العملية Action badge، ملوّنة حسب النوع ──
function ActionBadge({ action }: { action: string }) {
  const colorMap: Record<string, string> = {
    Created: "bg-green-500/20 text-green-400",
    Updated: "bg-orange-500/20 text-orange-400",
    Deleted: "bg-red-500/20 text-red-400",
  };
  const color = colorMap[action] ?? "bg-gray-500/20 text-gray-300";
  return (
    <span className={`text-xs px-2 py-0.5 rounded-lg font-medium ${color}`}>
      {action}
    </span>
  );
}

// ── تنسيق الوقت النسبيّ Relative time: منذ كم ──
function formatRelativeTime(iso: string): string {
  const then = new Date(iso).getTime();
  const now = Date.now();
  const diffMs = now - then;

  const minutes = Math.floor(diffMs / 60000);
  if (minutes < 1) return "just now";
  if (minutes < 60) return `${minutes}m ago`;

  const hours = Math.floor(minutes / 60);
  if (hours < 24) return `${hours}h ago`;

  const days = Math.floor(hours / 24);
  return `${days}d ago`;
}

// ── فكّ JSON بأمان Safe parse، يرجع null عند الفشل ──
// ── فكّ JSON بأمان Safe parse، يرجع نصّاq منسّقاq أو null ──
// ── فكّ JSON إلى أزواج حقل وقيمة، مقروءة للمستخدم ──
// نفكّ الكائنات المتداخلة مثل المعرّف { value: ... } إلى قيمتها
function parseToFields(
  value: string | null
): { key: string; value: string }[] | null {
  if (!value) return null;
  try {
    const obj = JSON.parse(value);
    return Object.entries(obj).map(([key, val]) => ({
      key,
      value: formatValue(val),
    }));
  } catch {
    return null;
  }
}

// نحوّل قيمةq إلى نصّ مقروء، فاكّاq التداخل
function formatValue(val: unknown): string {
  if (val === null || val === undefined) return "—";

  // كائن معرّف أو رمز: { value: "..." }، نعرض قيمته الداخلية
  if (typeof val === "object" && val !== null && "value" in val) {
    return String((val as { value: unknown }).value);
  }

  // كائن آخر: نعرضه نصّاq مختصراq
  if (typeof val === "object") return JSON.stringify(val);

  return String(val);
}

// ── جدول حقل وقيمة، لعرض التغييرات مقروءةq ──
function FieldTable({
  fields,
}: {
  fields: { key: string; value: string }[];
}) {
  return (
    <div className="space-y-1">
      {fields.map((f) => (
        <div key={f.key} className="flex gap-3">
          {/* اسم الحقل */}
          <span className="text-[var(--color-text-secondary)] min-w-32">
            {f.key}
          </span>
          {/* قيمته */}
          <span className="text-[var(--color-text-primary)] break-all">
            {f.value}
          </span>
        </div>
      ))}
    </div>
  );
}