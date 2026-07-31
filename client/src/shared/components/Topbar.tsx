// الشريط العلويّ: عنوان الصفحة، وأيقونتا البحث والإشعارات

interface TopbarProps {
  title: string;
}

export function Topbar({ title }: TopbarProps) {
  return (
    <header className="h-16 border-b border-[var(--color-border-subtle)] flex items-center justify-between px-8">
      <h1 className="text-lg font-semibold text-[var(--color-text-primary)]">
        {title}
      </h1>

      <div className="flex items-center gap-4 text-[var(--color-text-secondary)]">
        {/* أيقونة البحث */}
        <button className="hover:text-[var(--color-text-primary)] transition">
          <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <circle cx="11" cy="11" r="8" />
            <path d="m21 21-4.3-4.3" />
          </svg>
        </button>

        {/* أيقونة الإشعارات */}
        <button className="hover:text-[var(--color-text-primary)] transition">
          <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M6 8a6 6 0 0 1 12 0c0 7 3 9 3 9H3s3-2 3-9" />
            <path d="M10.3 21a1.94 1.94 0 0 0 3.4 0" />
          </svg>
        </button>
      </div>
    </header>
  );
}