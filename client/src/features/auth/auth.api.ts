// دوالّ استدعاء الخادم الخاصّة بالمصادقة
// نفصلها عن المكوّن، فيبقى المكوّن للعرض والدالّة للاتصال

import { apiClient } from "../../shared/api/client";
import type { LoginRequest, LoginResponse, RegisterRequest } from "./auth.types";


// دالّة الدخول: ترسل البيانات، وترجع الرمز عند النجاح
export async function login(
  data: LoginRequest
): Promise<LoginResponse> {
  // نستعمل العميل المركزيّ، فيحقن العنوان تلقائياً
  const response = await apiClient.post<LoginResponse>(
    "/api/auth/login",
    data
  );

  // axios يضع الجسم المُرجَع في حقل data
  return response.data;
}

// دالّة التسجيل: تنشئ مستأجراً جديداً
export async function register(data: RegisterRequest): Promise<void> {
  // نقطة تسجيل المستأجر في الخادم
  await apiClient.post("/api/tenants/register", data);
}