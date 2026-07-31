// صفحة الدخول، مصمّمة لتطابق الهويّة البصرية
// خلفية متدرّجة، شعار في الأعلى، بطاقة بظلّ وحوافّ ناعمة

import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useMutation } from "@tanstack/react-query";
import { login } from "./auth.api";

export function LoginPage() {
  // حالة حقول النموذج
  const [slug, setSlug] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const navigate = useNavigate();

  // عملية التغيير: إرسال بيانات الدخول
  const mutation = useMutation({
    mutationFn: login,
    onSuccess: (data) => {
      localStorage.setItem("accessToken", data.accessToken);
      navigate("/dashboard");
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    mutation.mutate({ slug, email, password });
  };

  return (
    // الخلفية: تدرّج خفيف يعطي عمقاً، لا لون مسطّح
    <div
      className="min-h-screen flex flex-col items-center justify-center px-4"
      style={{
        background:
          "radial-gradient(ellipse at top, #16241d 0%, #0d1512 60%)",
      }}
    >
      {/* ── الشعار والعنوان الفرعيّ فوق البطاقة ── */}
      <div className="text-center mb-8">
        <h1 className="text-4xl font-bold tracking-tight text-[var(--color-text-primary)]">
          BuildFlow
        </h1>
        <p className="text-sm text-[var(--color-text-secondary)] mt-2">
          Engineering Document Platform
        </p>
      </div>

      {/* ── البطاقة: حوافّ ناعمة، ظلّ، حدّ خفيف ── */}
      <div className="w-full max-w-md bg-[var(--color-bg-surface)] rounded-2xl shadow-2xl border border-[var(--color-border-subtle)] p-8">
        <h2 className="text-xl font-semibold mb-6 text-[var(--color-text-primary)]">
          Sign in to your workspace
        </h2>

        <form onSubmit={handleSubmit} className="space-y-5">
          {/* حقل مساحة العمل */}
          <div>
            <label className="block mb-2 text-sm font-medium text-[var(--color-text-primary)]">
              Workspace
            </label>
            <input
              type="text"
              value={slug}
              onChange={(e) => setSlug(e.target.value)}
              placeholder="your-company"
              className="w-full bg-[var(--color-bg-elevated)] border border-[var(--color-border-subtle)] rounded-xl px-4 py-3 text-[var(--color-text-primary)] placeholder:text-[var(--color-text-secondary)] focus:outline-none focus:border-[var(--color-accent)] transition"
            />
            {/* تلميحة تحت الحقل */}
            <p className="text-xs text-[var(--color-text-secondary)] mt-1.5">
              Your tenant slug (e.g. al-rashid-eng)
            </p>
          </div>

          {/* حقل البريد */}
          <div>
            <label className="block mb-2 text-sm font-medium text-[var(--color-text-primary)]">
              Email
            </label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="you@company.com"
              className="w-full bg-[var(--color-bg-elevated)] border border-[var(--color-border-subtle)] rounded-xl px-4 py-3 text-[var(--color-text-primary)] placeholder:text-[var(--color-text-secondary)] focus:outline-none focus:border-[var(--color-accent)] transition"
            />
          </div>

          {/* حقل كلمة المرور */}
          <div>
            <label className="block mb-2 text-sm font-medium text-[var(--color-text-primary)]">
              Password
            </label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="w-full bg-[var(--color-bg-elevated)] border border-[var(--color-border-subtle)] rounded-xl px-4 py-3 text-[var(--color-text-primary)] focus:outline-none focus:border-[var(--color-accent)] transition"
            />
          </div>

          {/* رسالة الخطأ */}
          {mutation.isError && (
            <div className="text-red-400 text-sm">
              Login failed. Please check your credentials.
            </div>
          )}

          {/* زرّ الدخول */}
          <button
            type="submit"
            disabled={mutation.isPending}
            className="w-full bg-[var(--color-accent)] hover:bg-[var(--color-accent-hover)] text-white font-medium py-3 rounded-xl transition disabled:opacity-50"
          >
            {mutation.isPending ? "Signing in..." : "Sign In"}
          </button>
        </form>
      </div>
    </div>
  );
}