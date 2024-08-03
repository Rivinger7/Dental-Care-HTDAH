Demo Project: https://dentalcareplatform.azurewebsites.net/

Deployed By Tú and Hòa
 
 Hướng dẫn sử dụng trước khi dùng Database (Update Version 16) 🚀
===
### 1. Xóa Database 🎯
- Phía trên nhấn Tools -> NuGet Package Manager -> Package Manager Console (PMC)
- Trong PMC, gõ : `Drop-Database`
- Khi hiện dòng option [Y] Yes  [A] Yes to All  [N] No  [L] No to All  [S] Suspend  [?] Help (default is "Y"): `Nhấn phím A`
  
### 2. Thêm lại Database để reset ID tự tăng 
- Sau khi Drop Database thành công rồi thì gõ `Update-Database`

### 3. Thêm Dữ Liệu Vào Database
- Script để insert dữ liệu vào Database lấy [ở đây nè!!!](https://github.com/Hoapooh/Dental-Care-HTDAH/blob/master/Clinic_InsertDB_Script_V20_Hoa.sql)
- Mở File .sql lên và chỉ cần ấn Execute (hoặc F5) 🔥🔥🔥. Không cần chọn tên DB luôn 😎 Quá đãããã...

### 4. Các lệnh cần lưu ý:
 > - Add-Migration tên_Migration <br>
 > - Update-Database <br>
 > - Remove-Migration (Cái này sẽ xóa Migration gần nhất, ko xóa cái Migration đang áp dụng cho Database)
 > - Drop-Database<br>
 
<code>**Mỗi lần có thay đổi liên quan đến File DentalClinicDbContext thì add Migration mới rồi Update-Database**</code>

---
©️Pham Duy Hoang 2024 | Thanks for reading!!! ❤️

