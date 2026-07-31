// التخطيط العامّ: يجمع الشريط الجانبيّ والعلويّ والمحتوى
// كل صفحة داخلية تعيش داخله، فتحصل على الإطار تلقائياً

import { Sidebar } from "./Sidebar";
import { Topbar } from "./Topbar";
import type { ReactNode } from "react";

interface AppLayoutProps {
  title: string;      // عنوان الصفحة، يظهر في الشريط العلويّ
  children: ReactNode; // محتوى الصفحة نفسها
}

export function AppLayout({ title, children }: AppLayoutProps) {
  return (
    // حاوية أفقية: الشريط الجانبيّ، ثم منطقة المحتوى
    <div className="flex h-screen bg-[var(--color-bg-base)]">
      {/* الشريط الجانبيّ ثابت على الجنب */}
      <Sidebar />

      {/* منطقة المحتوى: عمودية، الشريط العلويّ فوق المحتوى */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* الشريط العلويّ، يستقبل عنوان الصفحة */}
        <Topbar title={title} />

        {/* المحتوى نفسه، قابل للتمرير إن طال */}
        <main className="flex-1 overflow-y-auto p-8">
          {children}
        </main>
      </div>
    </div>
  );
}