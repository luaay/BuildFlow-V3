// مكوّن النافذة المنبثقة العامّ
// يعتّم الخلفية، ويعرض المحتوى في بطاقة وسط الشاشة
// نعيد استعماله لكل النوافذ: إنشاء مشروع، مستند، دعوة عضو

import type { ReactNode } from "react";

interface ModalProps {
  isOpen: boolean;       // هل النافذة مفتوحة
  onClose: () => void;   // دالّة الإغلاق
  title: string;         // عنوان النافذة
  children: ReactNode;   // محتوى النافذة
}

export function Modal({ isOpen, onClose, title, children }: ModalProps) {
  // إن كانت مغلقة، لا نعرض شيئاً
  if (!isOpen) return null;

  return (
    // ── الطبقة المعتّمة، تملأ الشاشة ──
    // النقر عليها يغلق النافذة
    <div
      className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 p-4"
      onClick={onClose}
    >
      {/* ── البطاقة، وسط الشاشة ── */}
      {/* stopPropagation يمنع إغلاقها عند النقر داخلها */}
      <div
        className="bg-[var(--color-bg-elevated)] border border-[var(--color-border-subtle)] rounded-2xl shadow-2xl w-full max-w-lg max-h-[90vh] overflow-y-auto"
        onClick={(e) => e.stopPropagation()}
      >
        {/* رأس النافذة: العنوان وزرّ الإغلاق */}
        <div className="flex items-center justify-between p-6 border-b border-[var(--color-border-subtle)]">
          <h3 className="text-lg font-semibold text-[var(--color-text-primary)]">
            {title}
          </h3>
          <button
            onClick={onClose}
            className="text-[var(--color-text-secondary)] hover:text-[var(--color-text-primary)] transition"
          >
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M18 6 6 18M6 6l12 12" />
            </svg>
          </button>
        </div>

        {/* محتوى النافذة */}
        <div className="p-6">{children}</div>
      </div>
    </div>
  );
}