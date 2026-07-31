// خطّاف المصادقة: يجيب سؤالاً واحداً — هل المستخدم مسجّل الدخول؟
// يعتمد على وجود الرمز في التخزين المحلّيّ

export function useAuth() {
  // اقرأ الرمز من التخزين المحلّيّ
  const token = localStorage.getItem("accessToken");

  // المستخدم داخل إن وُجد رمز
  const isAuthenticated = !!token;

  // دالّة الخروج: تحذف الرمز
  const logout = () => {
    localStorage.removeItem("accessToken");
    // أعِد التوجيه إلى الدخول
    window.location.href = "/login";
  };

  return { isAuthenticated, logout };
}