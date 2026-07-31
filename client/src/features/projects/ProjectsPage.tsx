// قائمة المشاريع: بطاقات، وتبويبات تصفية بالحالة، وبحث

import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { AppLayout } from "../../shared/components/AppLayout";
import { getProjects } from "./project.api";

import { Modal } from "../../shared/components/Modal";
import { createProject } from "./project.api";

import type { ReactNode } from "react";

// ── حالات المشروع للتبويبات، تطابق الخلفية ──
const statusTabs = ["All", "Draft", "Active", "OnHold", "Completed", "Cancelled"];

export function ProjectsPage() {
  // حالة التبويب المختار، تبدأ بالكل
  const [activeTab, setActiveTab] = useState("All");

  // هل نافذة الإنشاء مفتوحة
  const [isModalOpen, setIsModalOpen] = useState(false);

  // حالة نصّ البحث
  const [search, setSearch] = useState("");

  const { data, isLoading, isError } = useQuery({
    queryKey: ["projects"],
    queryFn: getProjects,
  });

  const allProjects = data?.items ?? [];

  // ── التصفية: بالحالة ثم بالبحث ──
  const projects = allProjects
    // صفِّ بالحالة، إلا إن كان التبويب "الكل"
    .filter((p) => activeTab === "All" || p.status === activeTab)
    // صفِّ بالبحث على الاسم والرمز
    .filter((p) => {
      if (!search.trim()) return true;
      const term = search.toLowerCase();
      return (
        p.name.toLowerCase().includes(term) ||
        p.code.toLowerCase().includes(term)
      );
    });

  return (
    <AppLayout title="Projects">
      {/* رأس الصفحة */}
      <div className="flex items-center justify-between mb-6">
        <div>
          <h2 className="text-2xl font-bold text-[var(--color-text-primary)]">
            Projects
          </h2>
          <p className="text-sm text-[var(--color-text-secondary)] mt-1">
            {data?.totalCount ?? 0} total projects
          </p>
        </div>
        <button
          onClick={() => setIsModalOpen(true)}
          className="bg-[var(--color-accent)] hover:bg-[var(--color-accent-hover)] text-white text-sm font-medium px-4 py-2.5 rounded-xl transition"
        >
          + New Project
        </button>
      </div>

      {/* ── شريط البحث والتبويبات ── */}
      <div className="flex flex-col md:flex-row md:items-center gap-4 mb-6">
        {/* حقل البحث */}
        <div className="relative flex-1 max-w-md">
          <input
            type="text"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Search projects..."
            className="w-full bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-xl pl-10 pr-4 py-2.5 text-sm text-[var(--color-text-primary)] placeholder:text-[var(--color-text-secondary)] focus:outline-none focus:border-[var(--color-accent)] transition"
          />
          {/* أيقونة البحث داخل الحقل */}
          <svg
            className="absolute left-3 top-1/2 -translate-y-1/2 text-[var(--color-text-secondary)]"
            width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"
          >
            <circle cx="11" cy="11" r="8" />
            <path d="m21 21-4.3-4.3" />
          </svg>
        </div>

        {/* التبويبات */}
        <div className="flex items-center gap-1 flex-wrap">
          {statusTabs.map((tab) => (
            <button
              key={tab}
              onClick={() => setActiveTab(tab)}
              className={`text-sm px-3 py-1.5 rounded-lg transition ${
                activeTab === tab
                  ? "bg-[var(--color-accent)] text-white font-medium"
                  : "text-[var(--color-text-secondary)] hover:bg-[var(--color-bg-elevated)]"
              }`}
            >
              {tab}
            </button>
          ))}
        </div>
      </div>

      {/* الحالات */}
      {isLoading && (
        <p className="text-[var(--color-text-secondary)]">Loading projects...</p>
      )}
      {isError && <p className="text-red-400">Failed to load projects.</p>}

      {/* القائمة */}
      {!isLoading && !isError && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {projects.map((project) => (
            <Link
              key={project.id}
              to={`/projects/${project.id}`}
              className="bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-2xl p-5 hover:border-[var(--color-accent)] transition"
            >
              <div className="flex items-start justify-between mb-4">
                <div className="w-11 h-11 rounded-xl bg-[var(--color-accent)] flex items-center justify-center text-white font-bold text-sm">
                  {project.code.slice(0, 2).toUpperCase()}
                </div>
                <StatusBadge status={project.status} />
              </div>
              <h3 className="font-semibold text-[var(--color-text-primary)]">
                {project.name}
              </h3>
              <p className="text-sm text-[var(--color-text-secondary)] mt-1">
                {project.clientName}
              </p>
              <div className="flex items-center justify-between mt-4 pt-4 border-t border-[var(--color-border-subtle)] text-xs text-[var(--color-text-secondary)]">
                <span>{project.memberCount} members</span>
                <span>
                  {project.budgetAmount.toLocaleString()} {project.budgetCurrency}
                </span>
              </div>
            </Link>
          ))}
        </div>
      )}

      {/* رسالة الفراغ */}
      {!isLoading && !isError && projects.length === 0 && (
        <div className="text-center py-16">
          <p className="text-[var(--color-text-secondary)]">No projects found</p>
        </div>
      )}
      {/* نافذة إنشاء المشروع */}
      <CreateProjectModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
      />
    </AppLayout>
  );
}

// شارة الحالة الملوّنة
function StatusBadge({ status }: { status: string }) {
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

// ── نافذة إنشاء المشروع ──
function CreateProjectModal({
  isOpen,
  onClose,
}: {
  isOpen: boolean;
  onClose: () => void;
}) {
  // عميل الاستعلامات، لتحديث القائمة بعد الإنشاء
  const queryClient = useQueryClient();

  // حالة الحقول الستّة الظاهرة
  const [name, setName] = useState("");
  const [code, setCode] = useState("");
  const [clientName, setClientName] = useState("");
  const [location, setLocation] = useState("");
  const [budget, setBudget] = useState("");
  const [currency, setCurrency] = useState("USD");

  // عملية التغيير: إنشاء المشروع
  const mutation = useMutation({
    mutationFn: createProject,
    onSuccess: () => {
      // أبطِل استعلام المشاريع، فيُعاد جلبه بالمشروع الجديد
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      onClose();       // أغلِق النافذة
      resetForm();     // فرّغ الحقول
    },
  });

  // تفريغ الحقول
  const resetForm = () => {
    setName("");
    setCode("");
    setClientName("");
    setLocation("");
    setBudget("");
    setCurrency("USD");
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    mutation.mutate({
      name,
      code,
      clientName,
      location,
      budget: Number(budget) || 0,
      currency,
      description: "",     // غير ظاهر في التصميم، نرسله فارغاً
      startDate: null,
      endDate: null,
    });
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="New Project">
      <form onSubmit={handleSubmit} className="space-y-4">
        {/* اسم المشروع */}
        <Field label="Project Name" required>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Bridge Rehabilitation Project"
            className={inputClass}
          />
        </Field>

        {/* الرمز والعميل، جنباً إلى جنب */}
        <div className="grid grid-cols-2 gap-4">
          <Field label="Code" required>
            <input
              type="text"
              value={code}
              onChange={(e) => setCode(e.target.value)}
              placeholder="BRP-2024"
              className={inputClass}
            />
          </Field>
          <Field label="Client Name">
            <input
              type="text"
              value={clientName}
              onChange={(e) => setClientName(e.target.value)}
              placeholder="Ministry of Transport"
              className={inputClass}
            />
          </Field>
        </div>

        {/* الموقع */}
        <Field label="Location">
          <input
            type="text"
            value={location}
            onChange={(e) => setLocation(e.target.value)}
            placeholder="Baghdad, Iraq"
            className={inputClass}
          />
        </Field>

        {/* الميزانية والعملة */}
        <div className="grid grid-cols-2 gap-4">
          <Field label="Budget">
            <input
              type="number"
              value={budget}
              onChange={(e) => setBudget(e.target.value)}
              placeholder="0"
              className={inputClass}
            />
          </Field>
          <Field label="Currency">
            <input
              type="text"
              value={currency}
              onChange={(e) => setCurrency(e.target.value)}
              className={inputClass}
            />
          </Field>
        </div>

        {/* رسالة الخطأ */}
        {mutation.isError && (
          <p className="text-red-400 text-sm">
            Failed to create project. Please check the fields.
          </p>
        )}

        {/* الأزرار */}
        <div className="flex items-center justify-end gap-3 pt-2">
          <button
            type="button"
            onClick={onClose}
            className="text-sm text-[var(--color-text-secondary)] hover:text-[var(--color-text-primary)] px-4 py-2 transition"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={mutation.isPending}
            className="bg-[var(--color-accent)] hover:bg-[var(--color-accent-hover)] text-white text-sm font-medium px-5 py-2 rounded-xl transition disabled:opacity-50"
          >
            {mutation.isPending ? "Creating..." : "Create Project"}
          </button>
        </div>
      </form>
    </Modal>
  );
}

// ── مكوّن حقل: عنوان فوق محتوى ──
function Field({
  label,
  required,
  children,
}: {
  label: string;
  required?: boolean;
  children: ReactNode;
}) {
  return (
    <div>
      <label className="block mb-1.5 text-sm font-medium text-[var(--color-text-primary)]">
        {label} {required && <span className="text-red-400">*</span>}
      </label>
      {children}
    </div>
  );
}

// صنف الحقول المشترك، نعرّفه مرّةً
const inputClass =
  "w-full bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-xl px-4 py-2.5 text-sm text-[var(--color-text-primary)] placeholder:text-[var(--color-text-secondary)] focus:outline-none focus:border-[var(--color-accent)] transition";