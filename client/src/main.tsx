// نقطة الدخول: تزرع التطبيق في صفحة HTML
// وتلفّه بالموفّرين اللذين يحتاجهما كل التطبيق

import { StrictMode } from "react";
import { createRoot } from "react-dom/client";

// موفّر التنقّل: يتيح المسارات في كل مكان
import { BrowserRouter } from "react-router-dom";

// موفّر الاستعلامات: يتيح جلب البيانات في كل مكان
import { QueryClientProvider } from "@tanstack/react-query";

import { queryClient } from "./app/queryClient";
import App from "./App";
import "./index.css";

// جد عنصر الجذر في صفحة HTML، وارسم فيه التطبيق
createRoot(document.getElementById("root")!).render(
  <StrictMode>
    {/* موفّر الاستعلامات يلفّ كل شيء، فيتاح الجلب في أيّ مكوّن */}
    <QueryClientProvider client={queryClient}>
      {/* موفّر التنقّل يتيح المسارات والروابط */}
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </QueryClientProvider>
  </StrictMode>
);