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