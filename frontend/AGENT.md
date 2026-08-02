# Role: Senior Front-end Developer, UI/UX Architect & React/Tailwind Expert

# Context:
Tôi cần xây dựng một trang chức năng cho phân hệ **Quản trị viên (Admin)** thuộc Hệ thống Quản lý Giáo dục (SIMS).
- **Tính năng cụ thể cần làm:** [Mô tả trang muốn làm - Ví dụ: Trang quản lý danh sách sinh viên / Trang Dashboard thống kê].
- **Tech Stack:** React (JSX - JavaScript thuần, Functional Components), Tailwind CSS.

# Requirements:

## 1. UI Layout & Dashboard Structure (Cấu trúc & Bố cục Admin)
- **Sidebar & Header Grid:** Thiết kế bố cục chuẩn Admin Dashboard gồm Sidebar cố định bên trái (có thể thu gọn) và khu vực nội dung chính (`<main>`) bên phải. Header chứa Breadcrumb, Avatar profile và Notification.
- **Spacing System:** Sử dụng hệ thống khoảng cách nhất quán của Tailwind (`p-4`, `p-6`, `gap-4`, `space-y-4`...) tương đương chuẩn 8px, 16px, 24px để tạo khoảng thở thị giác.
- **Responsive Web Design (RWD):** * Trên Desktop: Hiển thị đầy đủ Sidebar và khu vực nội dung chính.
    * Trên Mobile/Tablet: Sidebar tự động biến thành Drawer ẩn/hiện thông qua nút Hamburger; các bảng dữ liệu phải có cơ chế cuộn ngang (`overflow-x-auto`) không làm vỡ khung.

## 2. Design System & Component Consistency (Hệ thống thiết kế đồng bộ)
- Toàn bộ các UI Components (Button, Input, Card, Table, Modal, Badge/Tag) phải tuân thủ nghiêm ngặt các quy tắc:
    * **Bo góc (Border Radius):** Đồng bộ một chuẩn duy nhất (Ưu tiên `rounded-lg` cho card/modal và `rounded-md` cho input/button).
    * **Shadow & Borders:** Sử dụng `shadow-sm` hoặc `border border-slate-200` đồng nhất, tránh việc trang thì đổ bóng đậm, trang thì dùng viền sắc cạnh.
    * **Typography Hierarchy:** Định nghĩa rõ kích thước qua class Tailwind: Title (`text-2xl font-bold`), Section Header (`text-lg font-semibold`), Body (`text-sm text-slate-600`), Caption/Subtext (`text-xs text-slate-400`).
    * **Alignment:** Chiều cao (Height) của ô Input và Button nằm cùng một hàng phải bằng nhau tuyệt đối (ví dụ cùng dùng `h-10` hoặc `py-2`).

## 3. Data Table & Advanced Filtering (Xử lý dữ liệu & Bộ lọc)
- Nếu trang có hiển thị danh sách, hãy thiết kế một **Bảng dữ liệu (Data Table)** hiện đại:
    * Có checkbox chọn tất cả / chọn nhiều dòng để xử lý hàng loạt (Bulk actions - xóa, khóa tài khoản).
    * Phân trang (Pagination) rõ ràng: Hiển thị số lượng mục trên mỗi trang và nút điều hướng Trước/Sau.
- Tích hợp **Bộ lọc nâng cao (Advanced Filter Row):** Gồm ô tìm kiếm nhanh (Search Input), các Dropdown lọc theo trạng thái, chuyên ngành... và một nút "Clear Filter" để đặt lại bộ lọc.

## 4. UX States & Interaction (Trải nghiệm người dùng & Trạng thái UI)
- Đảm bảo có đầy đủ hiệu ứng vi mô (Micro-interactions): `duration-200 ease-in-out`, hover đổi màu nền nhẹ, focus input có viền màu Primary (`focus:ring-2 focus:ring-purple-500`).
- Triển khai 4 trạng thái bắt buộc cho mọi luồng dữ liệu:
    * **Loading State:** Sử dụng **Skeleton Screen** (Khung xương màu xám nhạt chuyển động nhấp nháy `animate-pulse`) thay vì Spinner thô kệch để tạo cảm giác app tải nhanh hơn.
    * **Empty State:** Khi bộ lọc không có kết quả, hiển thị một minh họa nhẹ kèm dòng chữ "Không tìm thấy dữ liệu phù hợp".
    * **Error State:** Khi API lỗi, hiển thị Alert đỏ kèm nút "Thử lại".
    * **Form Validation State:** Thông báo lỗi real-time hiển thị chữ màu đỏ ngay dưới ô Input bị sai và viền Input đổi sang màu đỏ (`border-red-500`).
- (Nếu có Upload) Hiển thị ảnh xem trước (Preview Thumbnail) kèm nút "Xóa/Thay đổi ảnh".

## 5. Technical Implementation & Performance (Kỹ thuật & Hiệu năng React JS)
- **Component-Driven:** Tách biệt các logic thành sub-components (ví dụ: `FilterBar.jsx`, `StudentTableRow.jsx`, `Pagination.jsx`).
- **State Management:** Sử dụng `useState` cho Form state, bộ lọc, và phân trang. Sử dụng `useMemo` để xử lý lọc/tìm kiếm dữ liệu cục bộ trên mảng dữ liệu (Mock Data) một cách mượt mà, tối ưu hiệu năng.
- **Mock Data:** Tạo sẵn một mảng dữ liệu JavaScript mẫu (Array of Objects) chứa các thông tin thực tế của trang để component có dữ liệu chạy ngay lập tức.

## 6. Breadcrumb & Navigation (Điều hướng phân cấp)
- Component Breadcrumb nằm cố định đầu trang theo dạng: `Trang chủ > Danh mục cha > Trang hiện tại`.
- Trang hiện tại viết chữ đậm, màu tối (`text-slate-800 font-medium`) và không có link. Các trang cấp cha có màu nhạt hơn, hover sẽ gạch chân và đổi màu để biểu thị có thể click quay lại.

## 7. Deliverables (Kết quả mong muốn)
- Mã nguồn chi tiết bằng React (JSX) kết hợp các class Tailwind CSS sạch, tối ưu.
- Giải thích cách tổ chức State và xử lý logic bộ lọc hoặc phân trang.
- Danh sách các lưu ý quan trọng về UX hoặc hiệu năng (Performance) cho tính năng này.