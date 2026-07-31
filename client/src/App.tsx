// المكوّن الجذر: يوزّع المسارات
// كل مسار يربط عنواناً بصفحة

import { Routes, Route, Navigate } from "react-router-dom";
import { LoginPage } from "./features/auth/LoginPage";
import { ProtectedRoute } from "./features/auth/ProtectedRoute";
import { ProjectsPage } from "./features/projects/ProjectsPage";
import { ProjectDetailPage } from "./features/projects/ProjectDetailPage";
import { DashboardPage } from "./features/dashboard/DashboardPage";
import { RegisterPage } from "./features/auth/RegisterPage";

function App() {
  return (
    <Routes>

      {/* مسار التسجيل: مفتوح */}
      <Route path="/register" element={<RegisterPage />} />
      
      {/* مسار الدخول: مفتوح، بلا حماية */}
      <Route path="/login" element={<LoginPage />} />

      {/* لوحة المعلومات: محميّة */}
      <Route
        path="/dashboard"
        element={
          <ProtectedRoute>
            <DashboardPage />
          </ProtectedRoute>
        }
      />

      {/* مسار المشاريع: محميّ، يُلفّ بالحارس */}
      <Route
        path="/projects"
        element={
          <ProtectedRoute>
            <ProjectsPage />
          </ProtectedRoute>
        }
      />

      {/* مسار تفصيل المشروع: محميّ، بمعامل متغيّر id */}
      <Route
        path="/projects/:id"
        element={
          <ProtectedRoute>
            <ProjectDetailPage />
          </ProtectedRoute>
        }
      />

      {/* المسار الجذر: حوّل إلى المشاريع */}
      <Route path="/" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}

export default App;