// خطّاف المصادقة: يجيب هل المستخدم داخل، ويعطي بياناته ودالّة الخروج

// شكل بيانات المستخدم المحفوظة
interface StoredUser {
  fullName: string;
  role: string;
  tenantSlug: string;
}

export function useAuth() {
  const token = localStorage.getItem("accessToken");
  const isAuthenticated = !!token;

  // اقرأ بيانات المستخدم، وحوّلها من نصّ إلى كائن
  const userJson = localStorage.getItem("user");
  const user: StoredUser | null = userJson ? JSON.parse(userJson) : null;

  // دالّة الخروج: تحذف الرمز والبيانات، وتعيد للدخول
  const logout = () => {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("user");
    window.location.href = "/login";
  };

  return { isAuthenticated, user, logout };
}