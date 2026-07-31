// صفحة تفصيل المشروع: تقرأ المعرّف من العنوان، وتجلب به المشروع
// نتعلّم هنا: قراءة معامل المسار useParams

import { useParams, Link } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { getProject } from "./project.api";

export function ProjectDetailPage() {
  // ── قراءة المعرّف من العنوان ──
  // useParams يعطي المعاملات المتغيّرة في المسار
  // اسم "id" يطابق ما سنكتبه في تعريف المسار لاحقاً
  const { id } = useParams<{ id: string }>();

  // ── الاستعلام، معتمِداً على المعرّف ──
  const { data, isLoading, isError } = useQuery({
    // المفتاح يحوي المعرّف، فكل مشروع يُخزَّن منفصلاً
    // لو فتحت مشروعاً آخر، يُخزَّن تحت مفتاح مختلف
    queryKey: ["project", id],

    // دالّة الجلب، نمرّر لها المعرّف
    // علامة التعجّب تؤكّد لـ TypeScript أن المعرّف موجود
    queryFn: () => getProject(id!),
  });

  // ── حالة التحميل ──
  if (isLoading) {
    return (
      <div className="p-8">
        <p className="text-gray-600">Loading project...</p>
      </div>
    );
  }

  // ── حالة الخطأ ──
  if (isError || !data) {
    return (
      <div className="p-8">
        <p className="text-red-600">Failed to load project.</p>
        {/* رابط العودة إلى القائمة */}
        <Link to="/projects" className="text-blue-600 underline mt-2 inline-block">
          Back to projects
        </Link>
      </div>
    );
  }

  // ── حالة النجاح ──
  return (
    <div className="p-8 max-w-3xl">
      {/* راب1 العودة */}
      <Link to="/projects" className="text-blue-600 underline mb-4 inline-block">
        ← Back to projects
      </Link>

      {/* العنوان والحالة */}
      <div className="flex justify-between items-start mb-6">
        <div>
          <h1 className="text-2xl font-bold">{data.name}</h1>
          <p className="text-gray-500">{data.code}</p>
        </div>
        <span className="text-sm px-3 py-1 rounded bg-gray-100">
          {data.status}
        </span>
      </div>

      {/* الوصف */}
      {data.description && (
        <p className="text-gray-700 mb-6">{data.description}</p>
      )}

      {/* شبكة التفاصيل */}
      <div className="grid grid-cols-2 gap-4 mb-6">
        <Detail label="Client" value={data.clientName} />
        <Detail label="Location" value={data.location ?? "—"} />
        <Detail
          label="Budget"
          value={`${data.budgetAmount} ${data.budgetCurrency}`}
        />
        <Detail label="Members" value={String(data.members.length)} />
      </div>

      {/* قائمة الأعضاء */}
      <h2 className="text-lg font-semibold mb-2">Members</h2>
      <div className="space-y-2">
        {data.members.map((member) => (
          <div
            key={member.userId}
            className="border rounded p-3 flex justify-between text-sm"
          >
            <span className="text-gray-600">{member.userId}</span>
            <span className="font-medium">{member.role}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

// ── مكوّن صغير مساعِd لعرض حقل تفصيل ──
// نعرّفه هنا لأنّه خاصّ بهذه الصفحة
function Detail({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <p className="text-xs text-gray-500">{label}</p>
      <p className="font-medium">{value}</p>
    </div>
  );
}