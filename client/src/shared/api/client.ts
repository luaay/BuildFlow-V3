// عميل الاتصال المركزيّ: كل طلب إلى الخادم يمرّ عبره
// فائدته: مكان واحد يحقن الرمز ويعالج الأخطاء، بلا تكرار في كل شاشة

import axios from "axios";

// عنوان الخادم. نقرؤه من متغيّر بيئة، مع قيمة افتراضية محلّية
const BASE_URL =
  import.meta.env.VITE_API_URL ?? "https://localhost:7124";

// ننشئ نسخة axios مهيّأة، نستعملها في كل التطبيق بدل axios المجرّد
export const apiClient = axios.create({
  baseURL: BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// ── اعتراض الطلبات: يُنفَّذ قبل إرسال أيّ طلب ──
// وظيفته: إن وُجد رمز محفوظ، أرفقه في ترويسة التصريح
apiClient.interceptors.request.use((config) => {
  // اقرأ الرمز من التخزين المحلّيّ
  const token = localStorage.getItem("accessToken");

  // إن وُجد، أضِفه بصيغة الحامل Bearer التي يتوقّعها الخادم
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

// ── اعتراض الاستجابات: يُنفَّذ عند وصول أيّ ردّ ──
// وظيفته: معالجة موحّدة للأخطاء، خاصّةً انتهاء الرمز
apiClient.interceptors.response.use(
  // الحالة الأولى: ردّ ناجح، مرّره كما هو
  (response) => response,

  // الحالة الثانية: ردّ بخطأ
  (error) => {
    // إن كان الخطأ عدم تصريح، فالرمز منتهٍ أو غير صالح
    if (error.response?.status === 401) {
      // احذف الرمز المنتهي
      localStorage.removeItem("accessToken");

      // أعِد المستخدم إلى صفحة الدخول
      // نستعمل window مؤقّتاً، وسنحسّنه لاحقاً مع التنقّل
      window.location.href = "/login";
    }

    // مرّر الخطأ ليعالجه المستدعي أيضاً
    return Promise.reject(error);
  }
);