// دوالّ استدعاء الخادم الخاصّة بالمصادقة
// نفصلها عن المكوّن، فيبقى المكوّن للعرض والدالّة للاتصال

import { apiClient } from "../../shared/api/client";
import type { LoginRequest, LoginResponse } from "./auth.types";

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