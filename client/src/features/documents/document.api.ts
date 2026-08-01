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

// ── دوالّ سير المراجعة ──

// تقديم المستند للمراجعة، بتعيين مراجع
export async function submitForReview(
  documentId: string,
  reviewerId: string
): Promise<void> {
  await apiClient.post(`/api/documents/${documentId}/submit-for-review`, {
    reviewerId,
  });
}

// اعتماد المستند
export async function approveDocument(
  documentId: string,
  notes: string | null
): Promise<void> {
  await apiClient.post(`/api/documents/${documentId}/approve`, { notes });
}

// رفض المستند
export async function rejectDocument(
  documentId: string,
  notes: string | null
): Promise<void> {
  await apiClient.post(`/api/documents/${documentId}/reject`, { notes });
}

// ما نرسله لإنشاء مستند
export interface CreateDocumentRequest {
  projectId: string;
  title: string;
  description: string;
  type: number;
  fileName: string;
  filePath: string;
  fileSizeBytes: number;
  contentType: string;
}

// دالّة إنشاء المستند
export async function createDocument(
  data: CreateDocumentRequest
): Promise<void> {
  await apiClient.post("/api/documents", data);
}