// صفحة المشاريع: تجلب المشاريع من الخادم وتعرضها
// نتعلّم هنا: الاستعلام useQuery، وحالاته الثلاث

import { useQuery } from "@tanstack/react-query";
import { getProjects } from "./project.api";
import { Link } from "react-router-dom";

export function ProjectsPage() {
  // ── الاستعلام: أداة جلب البيانات ──
  // نعطيه مفتاحاً يميّزه، ودالّة الجلب
  const { data, isLoading, isError } = useQuery({
    // مفتاح الاستعلام: اسم فريd يخزّن به TanStack Query البيانات
    // لو طلبت شاشة أخرى المشاريع بالمفتاح نفسه، تأخذها من الذاكرة بلا طلب جديد
    queryKey: ["projects"],

    // دالّة الجلب التي بنيناها
    queryFn: getProjects,
  });

  // ── الحالة الأولى: التحميل ──
  // أثناء انتظار الخادم، نعرض رسالة تحميل
  if (isLoading) {
    return (
      <div className="p-8">
        <p className="text-gray-600">Loading projects...</p>
      </div>
    );
  }

  // ── الحالة الثانية: الخطأ ──
  // إن فشل الطلب، نعرض رسالة خطأ
  if (isError) {
    return (
      <div className="p-8">
        <p className="text-red-600">Failed to load projects.</p>
      </div>
    );
  }

  // ── الحالة الثالثة: النجاح ──
  // وصلت البيانات، نعرض القائمة
  return (
    <div className="p-8">
      <h1 className="text-2xl font-bold mb-6">Projects</h1>

      {/* نمرّ على كل مشروع في القائمة ونعرضه */}
      <div className="space-y-3">
        {data?.items.map((project) => (
          // كل عنصر يحتاج مفتاحاً فريداً، نستعمل معرّفه
          <Link
            key={project.id}
            to={`/projects/${project.id}`}
            className="block border rounded-lg p-4 bg-white shadow-sm hover:shadow-md transition"
          >
            <div className="flex justify-between items-start">
              <div>
                <h2 className="font-semibold text-lg">{project.name}</h2>
                <p className="text-sm text-gray-500">{project.code}</p>
              </div>
              <span className="text-xs px-2 py-1 rounded bg-gray-100">
                {project.status}
              </span>
            </div>
            <p className="text-sm text-gray-600 mt-2">
              {project.clientName}
            </p>
          </Link>
        ))}
      </div>

      {/* إن كانت القائمة فارغة */}
      {data?.items.length === 0 && (
        <p className="text-gray-500">No projects yet.</p>
      )}
    </div>
  );
}