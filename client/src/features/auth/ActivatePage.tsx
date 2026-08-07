// صفحة تفعيل الحساب: المدعوّ يضع كلمة مروره بالرمز من الرابط
// الرمز يُقرأ من معامل العنوان، وكلمة المرور من المستخدم

import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { useSearchParams, useNavigate, Link } from "react-router-dom";
import { activateUser } from "../users/user.api";

export function ActivatePage() {
  const navigate = useNavigate();
  // نقرأ الرمز من معامل العنوان token
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token") ?? "";

  const [password, setPassword] = useState("");
  const [confirm, setConfirm] = useState("");

  const mutation = useMutation({
    mutationFn: () => activateUser(token, password),
    onSuccess: () => navigate("/login"),
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    mutation.mutate();
  };

  // لا رمز في الرابط، رسالة خطأ
  if (!token) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-[radial-gradient(circle_at_top,#16241d,#0d1512)] p-4">
        <div className="w-full max-w-md bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-2xl p-8 text-center">
          <p className="text-red-400">Invalid activation link.</p>
          <Link to="/login" className="text-[var(--color-accent)] hover:underline mt-4 inline-block">
            Back to sign in
          </Link>
        </div>
      </div>
    );
  }

  // هل كلمتا المرور متطابقتان
  const passwordsMatch = password === confirm;

  return (
    <div className="min-h-screen flex items-center justify-center bg-[radial-gradient(circle_at_top,#16241d,#0d1512)] p-4">
      <div className="w-full max-w-md bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-2xl p-8">
        <h1 className="text-2xl font-bold text-[var(--color-text-primary)] mb-1">
          Activate your account
        </h1>
        <p className="text-sm text-[var(--color-text-secondary)] mb-6">
          Set a password to finish setting up your account
        </p>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block mb-1.5 text-sm font-medium text-[var(--color-text-primary)]">
              New Password
            </label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className={inputClass}
            />
          </div>

          <div>
            <label className="block mb-1.5 text-sm font-medium text-[var(--color-text-primary)]">
              Confirm Password
            </label>
            <input
              type="password"
              value={confirm}
              onChange={(e) => setConfirm(e.target.value)}
              className={inputClass}
            />
          </div>

          {/* كلمتان غير متطابقتين */}
          {confirm && !passwordsMatch && (
            <p className="text-red-400 text-sm">Passwords do not match.</p>
          )}

          {mutation.isError && (
            <p className="text-red-400 text-sm">
              Activation failed. The link may be invalid or expired.
            </p>
          )}

          <button
            type="submit"
            disabled={mutation.isPending || !passwordsMatch || password.length < 8}
            className="w-full bg-[var(--color-accent)] hover:bg-[var(--color-accent-hover)] text-white font-medium py-2.5 rounded-xl transition disabled:opacity-50"
          >
            {mutation.isPending ? "Activating..." : "Activate account"}
          </button>
        </form>
      </div>
    </div>
  );
}

const inputClass =
  "w-full bg-[var(--color-bg-base)] border border-[var(--color-border-subtle)] rounded-xl px-4 py-2.5 text-sm text-[var(--color-text-primary)] focus:outline-none focus:border-[var(--color-accent)] transition";