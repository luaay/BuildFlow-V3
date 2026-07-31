// صفحة المستخدمين: بطاقات الأعضاء بدوائرهم وشاراتهم
// نبدأ بالعرض، ثم نضيف نافذة الدعوة

import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { AppLayout } from "../../shared/components/AppLayout";
import { Modal } from "../../shared/components/Modal";
import { getUsers, inviteUser } from "./user.api";

export function UsersPage() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ["users"],
    queryFn: getUsers,
  });

  const users = data?.items ?? [];

  const [isInviteOpen, setIsInviteOpen] = useState(false);

  return (
    <AppLayout title="Users">
      {/* رأس الصفحة */}
      <div className="flex items-center justify-between mb-6">
        <div>
          <h2 className="text-2xl font-bold text-[var(--color-text-primary)]">
            Team Members
          </h2>
          <p className="text-sm text-[var(--color-text-secondary)] mt-1">
            {data?.totalCount ?? 0} users in your workspace
          </p>
        </div>

        {/* زرّ الدعوة، بنفسجيّ، سنربطه بالنافذة لاحقاً */}
        <button
          onClick={() => setIsInviteOpen(true)}
          className="bg-[var(--color-purple)] hover:bg-[var(--color-purple-hover)] text-white text-sm font-medium px-4 py-2.5 rounded-xl transition"
        >
          + Invite Member
        </button>
      </div>

      {/* الحالات */}
      {isLoading && (
        <p className="text-[var(--color-text-secondary)]">Loading users...</p>
      )}
      {isError && <p className="text-red-400">Failed to load users.</p>}

      {/* شبكة بطاقات المستخدمين */}
      {!isLoading && !isError && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {users.map((user) => (
            <div
              key={user.id}
              className="bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-2xl p-5"
            >
              <div className="flex items-center gap-3 mb-3">
                {/* دائرة بحرف الاسم الأوّل */}
                <div className="w-12 h-12 rounded-full bg-[var(--color-purple)] flex items-center justify-center text-white font-semibold">
                  {user.fullName.charAt(0).toUpperCase()}
                </div>
                <div className="flex-1 min-w-0">
                  <p className="font-semibold text-[var(--color-text-primary)] truncate">
                    {user.fullName}
                  </p>
                  <p className="text-xs text-[var(--color-text-secondary)] truncate">
                    {user.email}
                  </p>
                </div>
              </div>

              {/* الشارات: الدور والحالة */}
              <div className="flex items-center gap-2">
                <RoleBadge role={user.role} />
                <span className="text-xs text-[var(--color-text-secondary)]">
                  {user.status}
                </span>
              </div>
            </div>
          ))}
        </div>
      )}
      {/* نافذة دعوة مستخدم */}
      <InviteMemberModal
        isOpen={isInviteOpen}
        onClose={() => setIsInviteOpen(false)}
      />
    </AppLayout>
  );
}

// ── شارة الدور، ملوّنة حسب الدور ──
function RoleBadge({ role }: { role: string }) {
  const colorMap: Record<string, string> = {
    Owner: "bg-purple-500/20 text-purple-400",
    Admin: "bg-red-500/20 text-red-400",
    Manager: "bg-orange-500/20 text-orange-400",
    Member: "bg-green-500/20 text-green-400",
    Viewer: "bg-gray-500/20 text-gray-300",
  };
  const color = colorMap[role] ?? "bg-gray-500/20 text-gray-300";
  return (
    <span className={`text-xs px-2.5 py-1 rounded-lg font-medium ${color}`}>
      {role}
    </span>
  );
}

// ── نافذة دعوة مستخدم جديد ──
function InviteMemberModal({
  isOpen,
  onClose,
}: {
  isOpen: boolean;
  onClose: () => void;
}) {
  const queryClient = useQueryClient();

  // حقول النموذج، اسمان منفصلان كالتصميم
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [role, setRole] = useState("4"); // Member افتراضياً
  const [password, setPassword] = useState("");

  const mutation = useMutation({
    mutationFn: inviteUser,
    onSuccess: () => {
      // أبطِل قائمة المستخدمين، فيظهر الجديد
      queryClient.invalidateQueries({ queryKey: ["users"] });
      onClose();
      resetForm();
    },
  });

  const resetForm = () => {
    setFirstName("");
    setLastName("");
    setEmail("");
    setRole("4");
    setPassword("");
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    mutation.mutate({
      email,
      // ندمج الاسمين في اسم كامل، كما يتوقّع الخادم
      fullName: `${firstName} ${lastName}`.trim(),
      initialPassword: password,
      role: Number(role),
    });
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Invite Member">
      <form onSubmit={handleSubmit} className="space-y-4">
        {/* الاسمان جنباً إلى جنب */}
        <div className="grid grid-cols-2 gap-4">
          <Field label="First Name">
            <input
              type="text"
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
              className={inputClass}
            />
          </Field>
          <Field label="Last Name">
            <input
              type="text"
              value={lastName}
              onChange={(e) => setLastName(e.target.value)}
              className={inputClass}
            />
          </Field>
        </div>

        {/* البريد */}
        <Field label="Email" required>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="nour@company.com"
            className={inputClass}
          />
        </Field>

        {/* الدور، أدوار المستأجر */}
        <Field label="Role">
          <select
            value={role}
            onChange={(e) => setRole(e.target.value)}
            className={inputClass}
          >
            <option value="2">Admin</option>
            <option value="3">Manager</option>
            <option value="4">Member</option>
            <option value="5">Viewer</option>
          </select>
        </Field>

        {/* كلمة المرور المؤقّتة */}
        <Field label="Temporary Password" required>
          <input
            type="text"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className={inputClass}
          />
        </Field>

        {mutation.isError && (
          <p className="text-red-400 text-sm">Failed to invite member.</p>
        )}

        {/* الأزرhd */}
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
            {mutation.isPending ? "Sending..." : "Send Invite"}
          </button>
        </div>
      </form>
    </Modal>
  );
}

// ── مكوّن حقل، وصنف الحقول ──
function Field({
  label,
  required,
  children,
}: {
  label: string;
  required?: boolean;
  children: React.ReactNode;
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

const inputClass =
  "w-full bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-xl px-4 py-2.5 text-sm text-[var(--color-text-primary)] placeholder:text-[var(--color-text-secondary)] focus:outline-none focus:border-[var(--color-accent)] transition";