// حارس المسار: يلفّ أيّ صفحة محميّة
// إن كان المستخدم داخلاً، يعرض الصفحة. وإلا يحوّله إلى الدخول

import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";
import { useAuth } from "./useAuth";

// النوع: المكوّن يستقبل ما يلفّه في children
interface ProtectedRouteProps {
  children: ReactNode;
}

export function ProtectedRoute({ children }: ProtectedRouteProps) {
  const { isAuthenticated } = useAuth();

  // إن لم يكن داخلاً، حوّله إلى الدخول
  // replace يمنع الرجوع بالزرّ إلى الصفحة المحميّة
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  // إن كان داخلاً، اعرض المحتوى الملفوف
  return <>{children}</>;
}