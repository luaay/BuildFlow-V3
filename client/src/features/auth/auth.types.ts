// أنواع المصادقة: تصف شكل ما نرسله ونستقبله
// TypeScript يستعملها ليتحقّق أنّنا لا نخطئ في الحقول

// ما نرسله عند الدخول، يطابق ما يتوقّعه الخادم
export interface LoginRequest {
  slug: string;      // المعرّف النصّيّ للمستأجر
  email: string;
  password: string;
}

// ما نستقبله عند نجاح الدخول، يطابق ما يرجعه الخادم
export interface LoginResponse {
  accessToken: string;
  userId: string;
  fullName: string;
  role: string;
  tenantId: string;
  tenantSlug: string;
}

// ما نرسله عند التسجيل، يطابق ما يتوقّعه الخادم
export interface RegisterRequest {
  tenantName: string;      // اسم الشركة
  slug: string;            // معرّف مساحة العمل
  plan: number;            // الخطّة، نثبّتها مؤقّتاً
  ownerEmail: string;
  ownerPassword: string;
  ownerFullName: string;   // الاسم الأوّل والأخير مدموجين
}