// صفحة المستندات: قائمة بأزرار سير المراجعة
// نبدأ بالعرض والتصفية، ثم أزرار السير، ثم الإنشاء

import { useState } from "react";

import { AppLayout } from "../../shared/components/AppLayout";

import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

import { Modal } from "../../shared/components/Modal";
import { getDocuments, submitForReview, approveDocument, rejectDocument, createDocument } from "./document.api";
import { getProjects } from "../projects/project.api";

// حالات المستند للتبويبات، خمس حالات كخلفيتنا
const statusTabs = ["All", "Draft", "UnderReview", "Approved", "Rejected", "Archived"];

export function DocumentsPage() {
  const [activeTab, setActiveTab] = useState("All");
  const [search, setSearch] = useState("");

  const [isModalOpen, setIsModalOpen] = useState(false);

  const { data, isLoading, isError } = useQuery({
    queryKey: ["documents"],
    queryFn: getDocuments,
  });

  const allDocs = data?.items ?? [];

  // التصفية بالحالة ثم البحث
  const documents = allDocs
    .filter((d) => activeTab === "All" || d.status === activeTab)
    .filter((d) => {
      if (!search.trim()) return true;
      return d.title.toLowerCase().includes(search.toLowerCase());
    });

    const queryClient = useQueryClient();
  

  // ── عمليات سير المراجعة ──
  // نعرّف الثلاث معاً، كلٌّ يبطِل القائمة بعd نجاحه

  // معرّف المستخدم الحاليّ، من الرمز
  // نحتاجه مراجعاً عند التقديم، للتبسيط نعيّن المستخدم نفسه
  const currentUserId = getUserIdFromToken();

  const submitMutation = useMutation({
    mutationFn: (docId: string) => submitForReview(docId, currentUserId),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ["documents"] }),
  });

  const approveMutation = useMutation({
    mutationFn: (docId: string) => approveDocument(docId, null),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ["documents"] }),
  });

  const rejectMutation = useMutation({
    mutationFn: (docId: string) => rejectDocument(docId, null),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ["documents"] }),
  });

  return (
    <AppLayout title="Documents">
      {/* رأس الصفحة */}
      <div className="flex items-center justify-between mb-6">
        <div>
          <h2 className="text-2xl font-bold text-[var(--color-text-primary)]">
            Documents
          </h2>
          <p className="text-sm text-[var(--color-text-secondary)] mt-1">
            {data?.totalCount ?? 0} total documents
          </p>
        </div>
        <button
          onClick={() => setIsModalOpen(true)}
          className="bg-[var(--color-accent)] hover:bg-[var(--color-accent-hover)] text-white text-sm font-medium px-4 py-2.5 rounded-xl transition"
        >
          + New Document
        </button>
      </div>

      {/* البحث والتبويبات */}
      <div className="flex flex-col md:flex-row md:items-center gap-4 mb-6">
        <div className="relative flex-1 max-w-md">
          <input
            type="text"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Search documents..."
            className="w-full bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-xl pl-10 pr-4 py-2.5 text-sm text-[var(--color-text-primary)] placeholder:text-[var(--color-text-secondary)] focus:outline-none focus:border-[var(--color-accent)] transition"
          />
          <svg className="absolute left-3 top-1/2 -translate-y-1/2 text-[var(--color-text-secondary)]" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <circle cx="11" cy="11" r="8" />
            <path d="m21 21-4.3-4.3" />
          </svg>
        </div>

        <div className="flex items-center gap-1 flex-wrap">
          {statusTabs.map((tab) => (
            <button
              key={tab}
              onClick={() => setActiveTab(tab)}
              className={`text-sm px-3 py-1.5 rounded-lg transition ${
                activeTab === tab
                  ? "bg-[var(--color-accent)] text-white font-medium"
                  : "text-[var(--color-text-secondary)] hover:bg-[var(--color-bg-elevated)]"
              }`}
            >
              {tab}
            </button>
          ))}
        </div>
      </div>

      {/* الحالات */}
      {isLoading && (
        <p className="text-[var(--color-text-secondary)]">Loading documents...</p>
      )}
      {isError && <p className="text-red-400">Failed to load documents.</p>}

      {/* القائمة، صفوف لا بطاقات، كالتصميم */}
      {!isLoading && !isError && (
        <div className="space-y-3">
          {documents.map((doc) => (
            <div
              key={doc.id}
              className="bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-2xl p-4 flex items-center justify-between"
            >
              {/* يسار: أيقونة ومعلومات */}
              <div className="flex items-center gap-3">
                <div className="w-11 h-11 rounded-xl bg-[var(--color-purple)] flex items-center justify-center text-white font-bold text-sm">
                  {doc.title.charAt(0).toUpperCase()}
                </div>
                <div>
                  <p className="font-medium text-[var(--color-text-primary)]">
                    {doc.title}
                  </p>
                  <p className="text-xs text-[var(--color-text-secondary)]">
                    DOC-{doc.currentVersionNumber} · {doc.type}
                  </p>
                </div>
              </div>

              {/* يمين: الحالة وأزرار سير المراجعة */}
              <div className="flex items-center gap-2">
                <StatusBadge status={doc.status} />
                <WorkflowActions
                  status={doc.status}
                  onSubmit={() => submitMutation.mutate(doc.id)}
                  onApprove={() => approveMutation.mutate(doc.id)}
                  onReject={() => rejectMutation.mutate(doc.id)}
                  isPending={
                    submitMutation.isPending ||
                    approveMutation.isPending ||
                    rejectMutation.isPending
                  }
                />
              </div>
            </div>
          ))}
        </div>
      )}

      {/* رسالة الفراغ */}
      {!isLoading && !isError && documents.length === 0 && (
        <div className="text-center py-16">
          <p className="text-[var(--color-text-secondary)]">No documents found</p>
        </div>
      )}
      {/* نافذة إنشM المستند */}
      <CreateDocumentModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
      />
    </AppLayout>
  );
}

// شارة حالة المستند الملوّنة
function StatusBadge({ status }: { status: string }) {
  const colorMap: Record<string, string> = {
    Draft: "bg-gray-500/20 text-gray-300",
    UnderReview: "bg-orange-500/20 text-orange-400",
    Approved: "bg-green-500/20 text-green-400",
    Rejected: "bg-red-500/20 text-red-400",
    Archived: "bg-blue-500/20 text-blue-400",
  };
  const color = colorMap[status] ?? "bg-gray-500/20 text-gray-300";
  return (
    <span className={`text-xs px-2.5 py-1 rounded-lg font-medium ${color}`}>
      {status}
    </span>
  );
}

// ── أزرار سير المراجعة، تتغيّر حسب الحالة ──
function WorkflowActions({
  status,
  onSubmit,
  onApprove,
  onReject,
  isPending,
}: {
  status: string;
  onSubmit: () => void;
  onApprove: () => void;
  onReject: () => void;
  isPending: boolean;
}) {
  const btnBase =
    "text-sm font-medium px-3 py-1.5 rounded-lg transition disabled:opacity-50";

  // المسوّدة: تُقدَّم للمراجعة
  if (status === "Draft") {
    return (
      <button
        onClick={onSubmit}
        disabled={isPending}
        className={`${btnBase} bg-[var(--color-accent)] hover:bg-[var(--color-accent-hover)] text-white`}
      >
        Submit
      </button>
    );
  }

  // قيd المراجعة: تُعتمَد أو تُرفَض
  if (status === "UnderReview") {
    return (
      <>
        <button
          onClick={onApprove}
          disabled={isPending}
          className={`${btnBase} bg-green-600 hover:bg-green-700 text-white`}
        >
          Approve
        </button>
        <button
          onClick={onReject}
          disabled={isPending}
          className={`${btnBase} bg-red-600 hover:bg-red-700 text-white`}
        >
          Reject
        </button>
      </>
    );
  }

  // بقيّة الحالات: لا أزرار سير
  return null;
}

// ── استخراج معرّف المستخدم من الرمز ──
// الرمز JWT، جزؤه الأوسg يحوي المعرّف في حقل sub
function getUserIdFromToken(): string {
  const token = localStorage.getItem("accessToken");
  if (!token) return "";
  try {
    // الرمz ثلاثة أجزاء بنقطتين، الأوسط هو الحمولة
    const payload = JSON.parse(atob(token.split(".")[1]));
    return payload.sub ?? "";
  } catch {
    return "";
  }
}

// ── نافذة إنشM المستند ──
function CreateDocumentModal({
  isOpen,
  onClose,
}: {
  isOpen: boolean;
  onClose: () => void;
}) {
  const queryClient = useQueryClient();

  // الحقول الظاهرة في التصميم
  const [title, setTitle] = useState("");
  const [type, setType] = useState("1"); // Drawing افتراضياً
  const [projectId, setProjectId] = useState("");
  const [description, setDescription] = useState("");

  // جلب المشاريع، لربط المستند بمشروع
  const projectsQuery = useQuery({
    queryKey: ["projects"],
    queryFn: getProjects,
  });
  const projects = projectsQuery.data?.items ?? [];

  const mutation = useMutation({
    mutationFn: createDocument,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["documents"] });
      onClose();
      resetForm();
    },
  });

  const resetForm = () => {
    setTitle("");
    setType("1");
    setProjectId("");
    setDescription("");
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!projectId) return; // المستند يحتاج مشروعاً

    mutation.mutate({
      projectId,
      title,
      description,
      type: Number(type),
      // بيانات الملفّ، قيم افتراضية، فالتصميم لا يعرضها
      fileName: `${title || "document"}.pdf`,
      filePath: `/storage/${title || "document"}.pdf`,
      fileSizeBytes: 100000,
      contentType: "application/pdf",
    });
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="New Document">
      <form onSubmit={handleSubmit} className="space-y-4">
        {/* العنوان */}
        <Field label="Title" required>
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Site plans"
            className={inputClass}
          />
        </Field>

        {/* النوع */}
        <Field label="Type" required>
          <select
            value={type}
            onChange={(e) => setType(e.target.value)}
            className={inputClass}
          >
            <option value="1">Drawing</option>
            <option value="2">Specification</option>
            <option value="3">Report</option>
            <option value="4">Contract</option>
            <option value="5">Permit</option>
            <option value="6">Other</option>
          </select>
        </Field>

        {/* المشروع */}
        <Field label="Project" required>
          <select
            value={projectId}
            onChange={(e) => setProjectId(e.target.value)}
            className={inputClass}
          >
            <option value="">Select a project...</option>
            {projects.map((p) => (
              <option key={p.id} value={p.id}>
                {p.name}
              </option>
            ))}
          </select>
        </Field>

        {/* الوصف */}
        <Field label="Description">
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Brief description..."
            rows={3}
            className={inputClass}
          />
        </Field>

        {mutation.isError && (
          <p className="text-red-400 text-sm">Failed to create document.</p>
        )}

        {/* الأزرار */}
        <div className="flex items-center justify-end gap-3 pt-2">
          <button
            type="button"
            onClick={onClose}
            className="text-sm text-[var(--color-text-secondary)] hover:text-[var(--color-text-primary)] px-4 py-2 transition"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={mutation.isPending || !projectId}
            className="bg-[var(--color-accent)] hover:bg-[var(--color-accent-hover)] text-white text-sm font-medium px-5 py-2 rounded-xl transition disabled:opacity-50"
          >
            {mutation.isPending ? "Creating..." : "Create Document"}
          </button>
        </div>
      </form>
    </Modal>
  );
}

// ── مكوّن حقل، وصنف الحقول ──
function Field({
  label,
  required,
  children,
}: {
  label: string;
  required?: boolean;
  children: React.ReactNode;
}) {
  return (
    <div>
      <label className="block mb-1.5 text-sm font-medium text-[var(--color-text-primary)]">
        {label} {required && <span className="text-red-400">*</span>}
      </label>
      {children}
    </div>
  );
}

const inputClass =
  "w-full bg-[var(--color-bg-surface)] border border-[var(--color-border-subtle)] rounded-xl px-4 py-2.5 text-sm text-[var(--color-text-primary)] placeholder:text-[var(--color-text-secondary)] focus:outline-none focus:border-[var(--color-accent)] transition";