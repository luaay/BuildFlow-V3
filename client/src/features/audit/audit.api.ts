// دوالّ استدعاء الخادم الخاصّة بسجلّ التدقيق

import { apiClient } from "../../shared/api/client";

// شكل سجلّ التدقيق الواحد، يطابق استجابة الخادم
export interface AuditEntry {
  id: string;
  userId: string | null;
  entityName: string;       // اسم الكيان، مثل Project
  entityId: string;
  action: string;           // العملية: Created / Updated / Deleted
  changedColumns: string | null;
  oldValues: string | null; // JSON نصّاq
  newValues: string | null; // JSON نصّاq
  occurredAt: string;
  ipAddress: string | null;
}

// القائمة المرقّمة، النوع العامّ نفسه
interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// دالّة جلب سجلّ التدقيق
export async function getAuditLog(): Promise<PagedResult<AuditEntry>> {
  const response = await apiClient.get<PagedResult<AuditEntry>>("/api/audit");
  return response.data;
}