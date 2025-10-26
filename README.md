# DepartmentStore_DSM
Hệ thống quản lý sản phẩm, tồn kho, đơn hàng, khách hàng, nhân viên.

## 📘 Overview
**DepartmentStore** là một hệ thống quản lý cửa hàng bách hóa (Department Store Management System) được xây dựng trong khuôn khổ môn học **PRN232 - .NET Application Development** tại Đại học FPT.

Dự án mô phỏng quy trình vận hành và quản lý của một cửa hàng bách hóa bao gồm quản lý **sản phẩm, nhân viên, khách hàng, hóa đơn, nhập hàng**, và **báo cáo doanh thu**, được phát triển theo mô hình **3-tier architecture** (Controller – Service – DataAccess) sử dụng **Code First** với **Entity Framework Core**.

## 🧩 System Architecture
DSM (solution)
 ├── DSM.API               → Web API project (Server)
 ├── DSM.DataAccess        → Data + EFCore + Entities
 ├── DSM.Services          → Business logic
 ├── DSM.Client.Razor      → Razor Pages (Client)
 └── DSM.Utilities         → Helpers, DTOs, Constants

