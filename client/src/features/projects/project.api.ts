// دوالّ استدعاء الخادم الخاصّة بالمشاريع
// نفصلها عن المكوّن، فيبقى المكوّن للعرض والدالّة للاتصال

import { apiClient } from "../../shared/api/client";
import type { ProjectSummary, ProjectDetail, PagedResult } from "./project.types";

// دالّة جلب المشاريع: تطلب القائمة المرقّمة من الخادم
export async function getProjects(): Promise<PagedResult<ProjectSummary>> {
  // نستعمل العميل المركزيّ، فيحقن العنوان والرمز تلقائياً
  const response = await apiClient.get<PagedResult<ProjectSummary>>("/api/projects");

  // axios يضع الجسم المُرجَع في حقل data
  return response.data;
}

// دالّة جلب مشروع واحd بمعرّفه
// تستقبل المعرّف، وتضعه في العنوان، وترجع التفصيل
export async function getProject(id: string): Promise<ProjectDetail> {
  // لاحظ المعرّف في العنوان، جزء متغيّر
  const response = await apiClient.get<ProjectDetail>(`/api/projects/${id}`);

  return response.data;
}

// ما نرسله لإنشاء مشروع، يطابق أمر الخادم
export interface CreateProjectRequest {
  name: string;
  code: string;
  description: string;
  budget: number;
  currency: string;
  clientName: string;
  location: string;
  startDate: string | null;
  endDate: string | null;
}

// دالّة إنشاء المشروع
export async function createProject(
  data: CreateProjectRequest
): Promise<void> {
  await apiClient.post("/api/projects", data);
}

// دالّة تغيير حالة المشروع
// الجسم يحمل الحالة المستهدفة رقماً، كما يتوقّع الخادم
export async function changeProjectStatus(
  id: string,
  targetStatus: number
): Promise<void> {
  await apiClient.patch(`/api/projects/${id}/status`, { targetStatus });
}


// دالّة إضافة عضt للمشروع
// الجسم يحمل معرّف المستخدم والدور رقماً
export async function addProjectMember(
  projectId: string,
  userId: string,
  role: number
): Promise<void> {
  await apiClient.post(`/api/projects/${projectId}/members`, {
    userId,
    role,
  });
}