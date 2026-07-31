// تفصيل المشروع، بالهويّة الجديدة داخل الإطار
// نبدأ بالعرض، ثم نضيف أزرار الحالة وإدارة الأعضاء

import { useParams, Link } from "react-router-dom";

import { AppLayout } from "../../shared/components/AppLayout";


import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";


import { useState } from "react";
import { Modal } from "../../shared/components/Modal";
import { getProject, changeProjectStatus, addProjectMember } from "./project.api";
import { getUsers } from "../users/user.api";

export function ProjectDetailPage() {
  const { id } = useParams<{ id: string }>();

  const { data, isLoading, isError } = useQuery({
    queryKey: ["project", id],
    queryFn: () => getProject(id!),
  });

  const queryClient = useQueryClient();

  // عملية تغيير الحالة
  const statusMutation = useMutation({
    mutationFn: (targetStatus: number) =>
      changeProjectStatus(id!, targetStatus),
    onSuccess: () => {
      // أبطِل التفصيل والقائمة، فيُعاd جلبهما بالحالة الجديدة
      queryClient.invalidateQueries({ queryKey: ["project", id] });
      queryClient.invalidateQueries({ queryKey: ["projects"] });
    },
  });
// هل نافذة إضافة العضt مفتوحة
  const [isMemberModalOpen, setIsMemberModalOpen] = useState(false);
  // حالة التحميل
  if (isLoading) {
    return (
      <AppLayout title="Projects">
        <p className="text-[var(--color-text-secondary)]">Loading project...</p>
      </AppLayout>
    );
  }

  // حالة الخطأ
  if (isError || !data) {
    return (
      <AppLayout title="Projects">
        <p className="text-red-400">Failed to load project.</p>
        <Link to="/projects" className="text-[var(--color-accent)] hover:underline mt-2 inline-block">
          Back to Projects
        </Link>
      </AppLayout>
    );
  }

  return (
    <AppLayout title="Projects">
      {/* رابط العودة */}
      <Link
        to="/projects"
        className="text-sm text-[var(--color-text-secondary)] hover:text-[var(--color-text-primary)] transition inline-flex items-center gap-1 mb-6"
      >
        ← Back to Projects
      </Link>

      {/* ── رأس التفصيل: الشارات والاسم ── */}
      <div className="flex items-start justify-between mb-8">
        <div>
          {/* شارتا الرمز والحالة */}
          <div className="flex items-center gap-2 mb-3">
            <span className="text-xs px-2.5 py-1 rounded-lg bg-[var(--color-accent)]/20 text-[var(--color-accent)] font-medium">
              {data.code}
            </span>
            <StatusBadge status={data.status} />
          </div>

          {/* الاسم والوصف */}
          <h2 className="text-2xl font-bold text-[var(--color-text-primary)]">
            {data.name}
          </h2>
          {data.description && (
            <p className="text-[var(--color-text-secondary)] mt-1">
              {data.description}
            </p>
          )}
        </div>

        {/* أزرار الحالة، تتغيّر حسب الحالة الحالية */}
        <div className="flex items-center gap-2">
          <StatusActions
            status={data.status}
            onChange={(target) => statusMutation.mutate(target)}
            isPending={statusMutation.isPending}
          />
        </div>
      </div>

      {/* ── بطاقات المعلومات ── */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
        <InfoCard label="Client" value={data.clientName} />
        <InfoCard label="Location" value={data.location ?? "—"} />
        <InfoCard
          label="Budget"
          value={`${data.budgetAmount.toLocaleString()} ${data.budgetCurrency}`}
        />
      </div>

      {/* ── قسم أعضاء الفريق ── */}
      <div className="bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-2xl p-6">
        <div className="flex items-center justify-between mb-4">
          <h3 className="font-semibold text-[var(--color-text-primary)]">
            Team Members ({data.members.length})
          </h3>
          <button
            onClick={() => setIsMemberModalOpen(true)}
            className="text-sm text-[var(--color-accent)] hover:underline"
          >
            + Add Member
          </button>
        </div>

        <div className="space-y-2">
          {data.members.map((member) => (
            <div
              key={member.userId}
              className="flex items-center gap-3 p-3 bg-[var(--color-bg-elevated)] rounded-xl"
            >
              {/* دائرة بحرف، مؤقّتاً من المعرّف */}
              <div className="w-9 h-9 rounded-full bg-[var(--color-purple)] flex items-center justify-center text-white text-sm font-medium">
                {member.userId.charAt(0).toUpperCase()}
              </div>
              <div className="flex-1">
                {/* المعرّف الخام مؤقّتاً، سنحسّنه لاحقاً */}
                <p className="text-sm text-[var(--color-text-primary)]">
                  {member.userId.slice(0, 8)}...
                </p>
              </div>
              <span className="text-xs text-[var(--color-text-secondary)]">
                {member.role}
              </span>
            </div>
          ))}
        </div>
      </div>
      {/* نافذة إضافة عضt */}
      <AddMemberModal
        projectId={id!}
        isOpen={isMemberModalOpen}
        onClose={() => setIsMemberModalOpen(false)}
      />
    </AppLayout>
  );
}

// ── بطاقة معلومة ──
function InfoCard({ label, value }: { label: string; value: string }) {
  return (
    <div className="bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-2xl p-5">
      <p className="text-xs text-[var(--color-text-secondary)] mb-1">{label}</p>
      <p className="font-medium text-[var(--color-text-primary)]">{value}</p>
    </div>
  );
}

// شارة الحالة الملوّنة، نفس ألوان القائمة
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

// ── أزرار الحالة، تعرض الانتقالات المشروعة فقط ──
// يطابq آلة حالات المشروع في الخلفية
function StatusActions({
  status,
  onChange,
  isPending,
}: {
  status: string;
  onChange: (targetStatus: number) => void;
  isPending: boolean;
}) {
  // أرقام الحالات، تطابق تعداد الخلفية
  const STATUS = {
    Active: 2,
    OnHold: 3,
    Completed: 4,
  };

  // زرّ أخضر، للإجراء الرئيسيّ
  const primaryBtn =
    "bg-[var(--color-accent)] hover:bg-[var(--color-accent-hover)] text-white text-sm font-medium px-4 py-2 rounded-xl transition disabled:opacity-50";

  // زرّ ثانويّ، بحدّ
  const secondaryBtn =
    "border border-[var(--color-border-subtle)] text-[var(--color-text-primary)] hover:bg-[var(--color-bg-elevated)] text-sm font-medium px-4 py-2 rounded-xl transition disabled:opacity-50";

  // ── الأزرار حسب الحالة ──
  // Planning: يُفعَّل
  if (status === "Planning") {
    return (
      <button
        onClick={() => onChange(STATUS.Active)}
        disabled={isPending}
        className={primaryBtn}
      >
        Activate
      </button>
    );
  }

  // Active: يُعلَّق أو يُكمَل
  if (status === "Active") {
    return (
      <>
        <button
          onClick={() => onChange(STATUS.OnHold)}
          disabled={isPending}
          className={secondaryBtn}
        >
          Put On Hold
        </button>
        <button
          onClick={() => onChange(STATUS.Completed)}
          disabled={isPending}
          className={primaryBtn}
        >
          Complete
        </button>
      </>
    );
  }

  // OnHold: يُفعَّل ثانيةً
  if (status === "OnHold") {
    return (
      <button
        onClick={() => onChange(STATUS.Active)}
        disabled={isPending}
        className={primaryBtn}
      >
        Reactivate
      </button>
    );
  }

  // Completed و Cancelled: لا أزرار، حالات نهائية
  return null;
}

// ── نافذة إضافة عضt للمشروع ──
function AddMemberModal({
  projectId,
  isOpen,
  onClose,
}: {
  projectId: string;
  isOpen: boolean;
  onClose: () => void;
}) {
  const queryClient = useQueryClient();

  // المستخدم والدور المختاران
  const [userId, setUserId] = useState("");
  const [role, setRole] = useState("1"); // Lead افتراضياً

  // جلب المستخدمين، لملء القائمة
  const usersQuery = useQuery({
    queryKey: ["users"],
    queryFn: getUsers,
  });

  const users = usersQuery.data?.items ?? [];

  // عملية إضافة العضt
  const mutation = useMutation({
    mutationFn: () => addProjectMember(projectId, userId, Number(role)),
    onSuccess: () => {
      // أبطِل التفصيل، فيظهر العضt الجديد
      queryClient.invalidateQueries({ queryKey: ["project", projectId] });
      onClose();
      setUserId("");
      setRole("1");
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!userId) return; // لا نرسل بلا مستخدم مختار
    mutation.mutate();
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Add Team Member">
      <form onSubmit={handleSubmit} className="space-y-4">
        {/* اختيار المستخدم */}
        <Field label="User">
          <select
            value={userId}
            onChange={(e) => setUserId(e.target.value)}
            className={inputClass}
          >
            <option value="">Select a user...</option>
            {users.map((u) => (
              <option key={u.id} value={u.id}>
                {u.fullName} — {u.email}
              </option>
            ))}
          </select>
        </Field>

        {/* اختيار الدور */}
        <Field label="Role">
          <select
            value={role}
            onChange={(e) => setRole(e.target.value)}
            className={inputClass}
          >
            <option value="1">Lead</option>
            <option value="2">Engineer</option>
            <option value="3">Reviewer</option>
            <option value="4">Viewer</option>
          </select>
        </Field>

        {/* رسالة الخطأ */}
        {mutation.isError && (
          <p className="text-red-400 text-sm">Failed to add member.</p>
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
            disabled={mutation.isPending || !userId}
            className="bg-[var(--color-accent)] hover:bg-[var(--color-accent-hover)] text-white text-sm font-medium px-5 py-2 rounded-xl transition disabled:opacity-50"
          >
            {mutation.isPending ? "Adding..." : "Add Member"}
          </button>
        </div>
      </form>
    </Modal>
  );
}

// ── مكوّن حقل، وصنف الحقول ──
function Field({
  label,
  children,
}: {
  label: string;
  children: React.ReactNode;
}) {
  return (
    <div>
      <label className="block mb-1.5 text-sm font-medium text-[var(--color-text-primary)]">
        {label}
      </label>
      {children}
    </div>
  );
}

const inputClass =
  "w-full bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-xl px-4 py-2.5 text-sm text-[var(--color-text-primary)] focus:outline-none focus:border-[var(--color-accent)] transition";