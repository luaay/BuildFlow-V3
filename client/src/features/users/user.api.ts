// دوالّ استدعاء الخادم الخاصّة بالمستخدمين

import { apiClient } from "../../shared/api/client";

// نوع المستخدم، مطابق لاستجابة الخادم
export interface User {
  id: string;
  email: string;
  fullName: string;
  role: string;
  status: string;
}

interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// دالّة جلب المستخدمين
export async function getUsers(): Promise<PagedResult<User>> {
  const response = await apiClient.get<PagedResult<User>>("/api/users");
  return response.data;
}


// ما نرسله لدعوة مستخdم، يطابق أمر الخادم
export interface InviteUserRequest {
  email: string;
  fullName: string;
  initialPassword: string;
  role: number;
}

// دالّة دعوة مستخدم جديد للمستأجر
export async function inviteUser(data: InviteUserRequest): Promise<void> {
  await apiClient.post("/api/users/invite", data);
}

// تفعيل الحساب: يرسل الرمز وكلمة المرور الجديدة
// لا رمز مصادقة، فالمدعوّ لا يملك جلسة بعد
export async function activateUser(
  activationToken: string,
  newPassword: string
): Promise<void> {
  await apiClient.post("/api/users/activate", {
    activationToken,
    newPassword,
  });
}