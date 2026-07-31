// لوحة المعلومات: بطاقات إحصاء، وأقسام حديثة
// نحسب الأعداد من قوائم المشاريع والمستندات

import { useQuery } from "@tanstack/react-query";
import { AppLayout } from "../../shared/components/AppLayout";
import { StatCard } from "./StatCard";
import { getProjects } from "../projects/project.api";
import { getDocuments } from "../documents/document.api";
// ── لوحة قسم حديث: عنوان، ورابط عرض الكل، ومحتوى أو رسالة فراغ ──
import { Link } from "react-router-dom";
import type { ReactNode } from "react";

export function DashboardPage() {
  // ── استعلامان: المشاريع والمستندات ──
  const projectsQuery = useQuery({
    queryKey: ["projects"],
    queryFn: getProjects,
  });

  const documentsQuery = useQuery({
    queryKey: ["documents"],
    queryFn: getDocuments,
  });

  // ── حساب الأعداد من القوائم ──
  // نستعمل قائمة فارغة احتياطاً حتى تصل البيانات
  const projects = projectsQuery.data?.items ?? [];
  const documents = documentsQuery.data?.items ?? [];

  // العدّ حسب الحالة
  const totalProjects = projectsQuery.data?.totalCount ?? 0;
  const activeProjects = projects.filter((p) => p.status === "Active").length;
  const underReviewDocs = documents.filter(
    (d) => d.status === "UnderReview"
  ).length;
  const approvedDocs = documents.filter((d) => d.status === "Approved").length;

  return (
    <AppLayout title="Dashboard">
      {/* الترحيب */}
      <h2 className="text-2xl font-bold text-[var(--color-text-primary)]">
        Welcome back 👋
      </h2>
      <p className="text-[var(--color-text-secondary)] mt-1 mb-8">
        Here's what's happening across your projects today.
      </p>

      {/* ── صفّ البطاقات الأربع ── */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatCard
          value={totalProjects}
          label="Total Projects"
          accentColor="#10b981"
          icon={<IconFolder />}
        />
        <StatCard
          value={activeProjects}
          label="Active Projects"
          hint="Currently running"
          accentColor="#059669"
          icon={<IconActivity />}
        />
        <StatCard
          value={underReviewDocs}
          label="Under Review"
          hint="Awaiting approval"
          accentColor="#b45309"
          icon={<IconClock />}
        />
        <StatCard
          value={approvedDocs}
          label="Approved Docs"
          hint="Ready to use"
          accentColor="#7c3aed"
          icon={<IconCheck />}
        />
      </div>
      {/* ── القسمان الحديثان ── */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 mt-4">
        {/* المشاريع الحديثة */}
        <RecentPanel
          title="Recent Projects"
          viewAllTo="/projects"
          isEmpty={projects.length === 0}
          emptyText="No projects yet"
        >
          {projects.slice(0, 3).map((project) => (
            <div
              key={project.id}
              className="flex items-center justify-between py-3 border-b border-[var(--color-border-subtle)] last:border-0"
            >
              <div>
                <p className="text-sm font-medium text-[var(--color-text-primary)]">
                  {project.name}
                </p>
                <p className="text-xs text-[var(--color-text-secondary)]">
                  {project.memberCount} members
                </p>
              </div>
              <StatusBadge status={project.status} />
            </div>
          ))}
        </RecentPanel>

        {/* المستندات الحديثة */}
        <RecentPanel
          title="Recent Documents"
          viewAllTo="/documents"
          isEmpty={documents.length === 0}
          emptyText="No documents yet"
        >
          {documents.slice(0, 3).map((doc) => (
            <div
              key={doc.id}
              className="flex items-center justify-between py-3 border-b border-[var(--color-border-subtle)] last:border-0"
            >
              <div>
                <p className="text-sm font-medium text-[var(--color-text-primary)]">
                  {doc.title}
                </p>
                <p className="text-xs text-[var(--color-text-secondary)]">
                  DOC · {doc.type}
                </p>
              </div>
              <StatusBadge status={doc.status} />
            </div>
          ))}
        </RecentPanel>
      </div>
    </AppLayout>
  );
}

// ── أيقونات بسيطة، رسوم متّجهة ──
function IconFolder() {
  return (
    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2">
      <path d="M4 20h16a2 2 0 0 0 2-2V8a2 2 0 0 0-2-2h-7.9a2 2 0 0 1-1.69-.9L9.6 3.9A2 2 0 0 0 7.93 3H4a2 2 0 0 0-2 2v13a2 2 0 0 0 2 2Z" />
    </svg>
  );
}

function IconActivity() {
  return (
    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2">
      <path d="M22 12h-4l-3 9L9 3l-3 9H2" />
    </svg>
  );
}

function IconClock() {
  return (
    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2">
      <circle cx="12" cy="12" r="10" />
      <path d="M12 6v6l4 2" />
    </svg>
  );
}

function IconCheck() {
  return (
    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2">
      <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" />
      <path d="m9 11 3 3L22 4" />
    </svg>
  );
}





interface RecentPanelProps {
  title: string;
  viewAllTo: string;
  isEmpty: boolean;
  emptyText: string;
  children: ReactNode;
}

function RecentPanel({
  title,
  viewAllTo,
  isEmpty,
  emptyText,
  children,
}: RecentPanelProps) {
  return (
    <div className="bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-2xl p-6">
      {/* رأس القسم: العنوان ورابط عرض الكل */}
      <div className="flex items-center justify-between mb-4">
        <h3 className="font-semibold text-[var(--color-text-primary)]">
          {title}
        </h3>
        <Link
          to={viewAllTo}
          className="text-xs text-[var(--color-accent)] hover:underline"
        >
          View all →
        </Link>
      </div>

      {/* المحتوى، أو رسالة الفراغ */}
      {isEmpty ? (
        <p className="text-sm text-[var(--color-text-secondary)] text-center py-8">
          {emptyText}
        </p>
      ) : (
        <div>{children}</div>
      )}
    </div>
  );
}

// ── شارة الحالة، ملوّنة حسب القيمة ──
function StatusBadge({ status }: { status: string }) {
  return (
    <span className="text-xs px-2 py-1 rounded bg-[var(--color-bg-elevated)] text-[var(--color-text-secondary)]">
      {status}
    </span>
  );
}