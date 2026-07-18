# المرحلة الأولى: البناء
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# انسخ ملفّ الحلّ وملفّات المشاريع أولاً، لاستغلال طبقات التخزين المؤقّت
COPY ["BuildFlow-V3.sln", "./"]
COPY ["src/", "src/"]

# استرجع الحزم ثم انشر التطبيق
RUN dotnet restore "src/BuildFlow.Api/BuildFlow.Api.csproj"
RUN dotnet publish "src/BuildFlow.Api/BuildFlow.Api.csproj" \
    -c Release -o /app/publish --no-restore

# المرحلة الثانية: التشغيل، صورة خفيفة بلا أدوات بناء
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# المنفذ الذي يستمع عليه التطبيق داخل الحاوية
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "BuildFlow.Api.dll"]