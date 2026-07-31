// دوالّ استدعاء الخادم الخاصّة بالمستندات

import { apiClient } from "../../shared/api/client";

// نوع ملخّص المستند، مطابق لاستجابة الخادم
export interface DocumentSummary {
  id: string;
  projectId: string;
  title: string;
  type: string;
  status: string;
  currentVersionNumber: number;
  createdAtUtc: string;
}

// نوع القائمة المرقّمة، عامّ
interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// دالّة جلب المستندات
export async function getDocuments(): Promise<PagedResult<DocumentSummary>> {
  const response = await apiClient.get<PagedResult<DocumentSummary>>(
    "/api/documents"
  );
  return response.data;
}