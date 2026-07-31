// عميل الاستعلامات: عقل TanStack Query
// يدير التخزين المؤقّت لكل بيانات الخادم في مكان واحد

import { QueryClient } from "@tanstack/react-query";

// ننشئ العميل، ونضبط سلوكه الافتراضيّ
export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      // كم مرّة يعيد المحاولة عند فشل الطلب
      retry: 1,

      // لا يعيد الجلب تلقائياً حين يعود التركيز إلى النافذة
      // نطفئه لأنه قد يزعج أثناء التطوير
      refetchOnWindowFocus: false,

      // مدّة اعتبار البيانات طازجة، بالأجزاء من الألف
      // خلالها لا يعيد الجلب، فتقلّ الطلبات
      staleTime: 30_000,
    },
  },
});