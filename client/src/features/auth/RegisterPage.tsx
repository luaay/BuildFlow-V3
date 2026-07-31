// صفحة التسجيل: إنشاء مساحة عمل جديدة
// ستّة حقول، ندمج الاسم الأوّل والأخير في اسم كامل للخادم

import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useMutation } from "@tanstack/react-query";
import { register } from "./auth.api";

export function RegisterPage() {
  // ── حالة الحقول الستّة ──
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [companyName, setCompanyName] = useState("");
  const [slug, setSlug] = useState("");

  const navigate = useNavigate();

  // عملية التغيير: إنشاء المستأجر
  const mutation = useMutation({
    mutationFn: register,
    onSuccess: () => {
      // بعد النجاح، إلى الدخول ليدخل بحسابه الجديد
      navigate("/login");
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    // ندمج الاسم الأوّل والأخير، ونثبّت الخطّة
    mutation.mutate({
      tenantName: companyName,
      slug,
      plan: 1,
      ownerEmail: email,
      ownerPassword: password,
      ownerFullName: `${firstName} ${lastName}`.trim(),
    });
  };

  return (
    <div
      className="min-h-screen flex flex-col items-center justify-center px-4 py-8"
      style={{
        background:
          "radial-gradient(ellipse at top, #16241d 0%, #0d1512 60%)",
      }}
    >
      {/* الشعار والعنوان الفرعيّ */}
      <div className="text-center mb-8">
        <h1 className="text-4xl font-bold tracking-tight text-[var(--color-text-primary)]">
          Create your workspace
        </h1>
        <p className="text-sm text-[var(--color-text-secondary)] mt-2">
          Get started with BuildFlow
        </p>
      </div>

      {/* البطاقة */}
      <div className="w-full max-w-md bg-[var(--color-bg-surface)] rounded-2xl shadow-2xl border border-[var(--color-border-subtle)] p-8">
        <form onSubmit={handleSubmit} className="space-y-5">
          {/* الاسم الأوّل والأخير، جنباً إلى جنب */}
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block mb-2 text-sm font-medium text-[var(--color-text-primary)]">
                First Name
              </label>
              <input
                type="text"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
                placeholder="Mohammed"
                className="w-full bg-[var(--color-bg-elevated)] border border-[var(--color-border-subtle)] rounded-xl px-4 py-3 text-[var(--color-text-primary)] placeholder:text-[var(--color-text-secondary)] focus:outline-none focus:border-[var(--color-accent)] transition"
              />
            </div>
            <div>
              <label className="block mb-2 text-sm font-medium text-[var(--color-text-primary)]">
                Last Name
              </label>
              <input
                type="text"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
                placeholder="Hassan"
                className="w-full bg-[var(--color-bg-elevated)] border border-[var(--color-border-subtle)] rounded-xl px-4 py-3 text-[var(--color-text-primary)] placeholder:text-[var(--color-text-secondary)] focus:outline-none focus:border-[var(--color-accent)] transition"
              />
            </div>
          </div>

          {/* البريد */}
          <div>
            <label className="block mb-2 text-sm font-medium text-[var(--color-text-primary)]">
              Work Email
            </label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="ali@company.com"
              className="w-full bg-[var(--color-bg-elevated)] border border-[var(--color-border-subtle)] rounded-xl px-4 py-3 text-[var(--color-text-primary)] placeholder:text-[var(--color-text-secondary)] focus:outline-none focus:border-[var(--color-accent)] transition"
            />
          </div>

          {/* كلمة المرور */}
          <div>
            <label className="block mb-2 text-sm font-medium text-[var(--color-text-primary)]">
              Password
            </label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Min 8 characters"
              className="w-full bg-[var(--color-bg-elevated)] border border-[var(--color-border-subtle)] rounded-xl px-4 py-3 text-[var(--color-text-primary)] placeholder:text-[var(--color-text-secondary)] focus:outline-none focus:border-[var(--color-accent)] transition"
            />
          </div>

          {/* اسم الشركة */}
          <div>
            <label className="block mb-2 text-sm font-medium text-[var(--color-text-primary)]">
              Company Name
            </label>
            <input
              type="text"
              value={companyName}
              onChange={(e) => setCompanyName(e.target.value)}
              placeholder="Euphrates Construction Co."
              className="w-full bg-[var(--color-bg-elevated)] border border-[var(--color-border-subtle)] rounded-xl px-4 py-3 text-[var(--color-text-primary)] placeholder:text-[var(--color-text-secondary)] focus:outline-none focus:border-[var(--color-accent)] transition"
            />
          </div>

          {/* معرّف مساحة العمل */}
          <div>
            <label className="block mb-2 text-sm font-medium text-[var(--color-text-primary)]">
              Workspace URL
            </label>
            <input
              type="text"
              value={slug}
              onChange={(e) => setSlug(e.target.value)}
              placeholder="al-rashid-eng"
              className="w-full bg-[var(--color-bg-elevated)] border border-[var(--color-border-subtle)] rounded-xl px-4 py-3 text-[var(--color-text-primary)] placeholder:text-[var(--color-text-secondary)] focus:outline-none focus:border-[var(--color-accent)] transition"
            />
            <p className="text-xs text-[var(--color-text-secondary)] mt-1.5">
              Only lowercase letters, numbers and hyphens
            </p>
          </div>

          {/* رسالة الخطأ */}
          {mutation.isError && (
            <div className="text-red-400 text-sm">
              Registration failed. Please check your details.
            </div>
          )}

          {/* زرّ الإنشاء، بنفسجيّ كالتصميم */}
          <button
            type="submit"
            disabled={mutation.isPending}
            className="w-full bg-[var(--color-purple)] hover:bg-[var(--color-purple-hover)] text-white font-medium py-3 rounded-xl transition disabled:opacity-50"
          >
            {mutation.isPending ? "Creating..." : "Create Workspace"}
          </button>

          {/* رابط الدخول للموجودين */}
          <p className="text-center text-sm text-[var(--color-text-secondary)]">
            Already have an account?{" "}
            <a href="/login" className="text-[var(--color-accent)] hover:underline">
              Sign in
            </a>
          </p>
        </form>
      </div>
    </div>
  );
}