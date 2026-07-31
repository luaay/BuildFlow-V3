// أنواع المشاريع: تصف شكل ما يرجعه الخادم بالضبط

// ملخّص المشروع الواحد في القائمة
export interface ProjectSummary {
  id: string;
  name: string;
  code: string;
  description: string | null;   // قد يكون فارغاً، لذا نسمح بالقيمة الفارغة
  status: string;
  budgetAmount: number;
  budgetCurrency: string;
  clientName: string;
  location: string | null;
  startDate: string | null;
  endDate: string | null;
  memberCount: number;
  createdAtUtc: string;
}

// ── الاستجابة المرقّمة ──
// الخادم لا يرجع القائمة مباشرةً، بل يلفّها بمعلومات الترقيم
// هذا النوع عامّ، يصلح لأيّ قائمة مرقّمة، لا المشاريع فقط
export interface PagedResult<T> {
  items: T[];              // القائمة نفسها
  totalCount: number;      // العدد الكلّيّ عبر كل الصفحات
  page: number;            // الصفحة الحالية
  pageSize: number;        // حجم الصفحة
  totalPages: number;      // عدد الصفحات
  hasNextPage: boolean;    // هل توجد صفحة تالية
  hasPreviousPage: boolean;
}

// ── عضو المشروع ──
// يظهر في التفصيل فقط، لا في القائمة
export interface ProjectMember {
  userId: string;
  role: string;
  joinedAtUtc: string;
}

// ── تفصيل المشروع الكامل ──
// يضيف على الملخّص: الأعضاء وتاريخ التعديل
export interface ProjectDetail {
  id: string;
  name: string;
  code: string;
  description: string | null;
  status: string;
  budgetAmount: number;
  budgetCurrency: string;
  clientName: string;
  location: string | null;
  startDate: string | null;
  endDate: string | null;
  members: ProjectMember[];       // قائمة الأعضاء
  createdAtUtc: string;
  modifiedAtUtc: string | null;   // قد يكون فارغاً إن لم يُعدَّل
}