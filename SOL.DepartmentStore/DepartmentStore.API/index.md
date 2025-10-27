
### install ban đầu
dotnet tool install --global dotnet-ef

### Cách migrate database
C1: Các bước migrate
1. cd ManagementLab.DataAccess
2. dotnet ef migrations add migrate_name
3. dotnet ef database update

C2. recommend
# Đứng ở thư mục ManagementLab
cd ManagementLab

# Tạo migration ( nếu tạo mới hoặc cập nhật . Bỏ qua bước này nếu không update)
dotnet ef migrations add InitialCreate -p DepartmentStore.DataAccess/DepartmentStore.DataAccess.csproj -s DepartmentStore.API/DepartmentStore.API.csproj --context AppDbContext

# Cập nhật DB
***
dotnet ef database update -p DepartmentStore.DataAccess/DepartmentStore.DataAccess.csproj -s DepartmentStore.API/DepartmentStore.API.csproj



### Other

Xóa migrate cuối cùng
dotnet ef migrations remove