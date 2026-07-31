// قائمة المشاريع، بالهويّة الجديدة داخل الإطار
// نبدأ بالعرض الأساسيّ، ونضيف التصفية والبحث لاحقاً

import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { AppLayout } from "../../shared/components/AppLayout";
import { getProjects } from "./project.api";

export function ProjectsPage() {
  // استعلام جلب المشاريع
  const { data, isLoading, isError } = useQuery({
    queryKey: ["projects"],
    queryFn: getProjects,
  });

  const projects = data?.items ?? [];

  return (
    <AppLayout title="Projects">
      {/* ── رأس الصفحة: العنوان والعدّاد، وزرّ الإنشاء ── */}
      <div className="flex items-center justify-between mb-6">
        <div>
          <h2 className="text-2xl font-bold text-[var(--color-text-primary)]">
            Projects
          </h2>
          <p className="text-sm text-[var(--color-text-secondary)] mt-1">
            {data?.totalCount ?? 0} total projects
          </p>
        </div>

        {/* زرّ الإنشاء، أخضر، سنربطه بالنافذة لاحقاً */}
        <button className="bg-[var(--color-accent)] hover:bg-[var(--color-accent-hover)] text-white text-sm font-medium px-4 py-2.5 rounded-xl transition">
          + New Project
        </button>
      </div>

      {/* ── حالة التحميل ── */}
      {isLoading && (
        <p className="text-[var(--color-text-secondary)]">Loading projects...</p>
      )}

      {/* ── حالة الخطأ ── */}
      {isError && (
        <p className="text-red-400">Failed to load projects.</p>
      )}

      {/* ── القائمة ── */}
      {!isLoading && !isError && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {projects.map((project) => (
            <Link
              key={project.id}
              to={`/projects/${project.id}`}
              className="bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-2xl p-5 hover:border-[var(--color-accent)] transition"
            >
              {/* رأس البطاقة: رمز المشروع وشارة الحالة */}
              <div className="flex items-start justify-between mb-4">
                {/* مربّع برمز المشروع */}
                <div className="w-11 h-11 rounded-xl bg-[var(--color-accent)] flex items-center justify-center text-white font-bold text-sm">
                  {project.code.slice(0, 2).toUpperCase()}
                </div>
                <StatusBadge status={project.status} />
              </div>

              {/* اسم المشروع والعميل */}
              <h3 className="font-semibold text-[var(--color-text-primary)]">
                {project.name}
              </h3>
              <p className="text-sm text-[var(--color-text-secondary)] mt-1">
                {project.clientName}
              </p>

              {/* أسفل البطاقة: الأعضhtml والميزانية */}
              <div className="flex items-center justify-between mt-4 pt-4 border-t border-[var(--color-border-subtle)] text-xs text-[var(--color-text-secondary)]">
                <span>{project.memberCount} members</span>
                <span>
                  {project.budgetAmount.toLocaleString()}{" "}
                  {project.budgetCurrency}
                </span>
              </div>
            </Link>
          ))}
        </div>
      )}

      {/* ── رسالة الفراغ ── */}
      {!isLoading && !isError && projects.length === 0 && (
        <div className="text-center py-16">
          <p className="text-[var(--color-text-secondary)]">No projects found</p>
        </div>
      )}
    </AppLayout>
  );
}

// ── شارة الحالة، ملوّنة حسب القيمة ──
function StatusBadge({ status }: { status: string }) {
  // نختار اللون حسب الحالة
  const colorMap: Record<string, string> = {
    Draft: "bg-gray-500/20 text-gray-300",
    Active: "bg-green-500/20 text-green-400",
    OnHold: "bg-yellow-500/20 text-yellow-400",
    Completed: "bg-blue-500/20 text-blue-400",
    Cancelled: "bg-red-500/20 text-red-400",
  };

  const color = colorMap[status] ?? "bg-gray-500/20 text-gray-300";

  return (
    <span className={`text-xs px-2.5 py-1 rounded-lg font-medium ${color}`}>
      {status}
    </span>
  );
}