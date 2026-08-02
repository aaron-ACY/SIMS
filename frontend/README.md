# SIMS - Student Information Management System (UI/UX Focus)

## Giới thiệu dự án
Hệ thống Quản lý Thông tin Sinh viên (SIMS) là một nền tảng Web chuyên biệt tập trung vào việc tối ưu hóa trải nghiệm quản lý giáo dục. Dự án hướng tới việc xây dựng một giao diện hiện đại, tinh giản, trực quan và mượt mà trên mọi nền tảng thiết bị từ máy tính để bàn (Desktop) cho đến các thiết bị di động (Mobile). Với triết lý lấy người dùng làm trung tâm (User-Centered Design), hệ thống giúp đơn giản hóa các tác vụ quản trị phức tạp thành những luồng tương tác tự nhiên và dễ tiếp cận nhất.

### Bảng công nghệ sử dụng (Tech Stack)

| Thành phần | Công nghệ | Phiên bản | Vai trò trong hệ thống |
| :--- | :--- | :--- | :--- |
| **Frontend Core** | React | 19.x | Thư viện xây dựng giao diện dựa trên Component, tối ưu render hiệu năng cao |
| **Styling** | Tailwind CSS | 4.x | Framework CSS tiện ích (Utility-first) giúp xây dựng Design System linh hoạt |
| **Animations** | Framer Motion | 12.x | Hiện thực hóa các chuyển động vi mô (Micro-interactions) và chuyển trang mượt mà |
| **Icons** | Lucide React | 1.x / Latest | Bộ icon vector tối giản, đồng bộ độ dày nét và khoảng cách |
| **Routing** | React Router DOM | 7.x | Điều hướng Single Page Application (SPA) mượt mà không tải lại trang |

---

## Hệ thống thiết kế đồng bộ (Design System Guidelines)

Để đảm bảo tính nhất quán thẩm mỹ (Visual Consistency) trên toàn bộ ứng dụng, SIMS áp dụng các quy chuẩn thiết kế nghiêm ngặt sau:

*   **Bảng màu (Color Palette):**
    *   *Primary (Màu chủ đạo):* Tím Hiện đại (`#6366f1` / `indigo-500` hoặc `#7c3aed` / `violet-600`) - đại diện cho tính công nghệ và học thuật sáng tạo.
    *   *Success (Thành công):* Xanh lục dịu (`emerald-500`) - dùng cho các trạng thái hoàn thành, đã chấm điểm hoặc hợp lệ.
    *   *Danger (Nguy hiểm/Lỗi):* Đỏ san hô (`rose-500`) - cảnh báo các hạn chót, lỗi nhập liệu hoặc hành động xóa dữ liệu.
    *   *Neutral Grays (Trung tính):* Thang xám từ đá phiến (`slate-50` đến `slate-900`) làm nền và chữ, giúp giảm mỏi mắt và tăng độ tương phản đọc hiểu.
*   **Độ bo góc (Border Radius):**
    *   Áp dụng `rounded-lg` (8px - 12px) cho các cấu trúc lớn như Cards (Thẻ thông tin) và Modal (Hộp thoại nổi) để tạo cảm giác mềm mại, hiện đại.
    *   Áp dụng `rounded-md` (6px) cho các thành phần tương tác nhỏ bên trong như Form Inputs, Action Buttons và Badges nhằm giữ sự sắc nét, chuẩn xác.
*   **Hệ thống chữ (Typography):** Sử dụng font chữ không chân hiện đại (Sans-serif) với tỷ lệ phân cấp rõ rệt:
    *   Tiêu đề trang chính (Page Header): `text-2xl font-bold tracking-tight text-slate-900`.
    *   Tiêu đề thẻ thông tin (Card Title): `text-lg font-semibold text-slate-800`.
    *   Nội dung chi tiết (Body Text): `text-sm text-slate-600` với chiều cao dòng thoáng (`leading-relaxed`).
*   **Đồng bộ kích thước:**
    *   Tất cả các phần tử Form Input và Button nằm trên cùng một hàng ngang luôn được ép cứng chiều cao bằng nhau (`h-10` hoặc `py-2`) để loại bỏ sự lệch trục thị giác.

---

## Chi tiết các trang & Tối ưu hóa UX (Key Pages & UI/UX Optimizations)

Mỗi trang trong hệ thống được thiết kế để giải quyết triệt để các hạn chế trải nghiệm thường gặp trên các phần mềm quản lý giáo dục truyền thống:

### 1. Trang Đăng nhập (Login Page)
*   **Bố cục Desktop:** Sử dụng chia đôi màn hình (Split-screen) tỷ lệ 50/50. Một bên là hình ảnh thương hiệu tối tối giản, sang trọng; một bên là Form đăng nhập nền trắng nổi bật trên nền tối.
*   **Trải nghiệm tương tác:**
    *   Tích hợp Icon mắt ẩn/hiện mật khẩu ngay trong Input để tránh việc nhập sai nhiều lần.
    *   Liên kết "Quên mật khẩu" được căn phải tự nhiên cuối Input mật khẩu.
    *   Nút hành động "Back to Home" (Quay lại trang chủ) được định vị cố định (fixed) góc trên bên trái giúp người dùng dễ dàng thoát trang.
*   **Tối ưu Mobile:** Khi mở bàn phím ảo trên điện thoại, Form tự động thu gọn chiều rộng (90% width) và đẩy khoảng cách lề thích ứng, ngăn chặn hoàn toàn việc vỡ layout hoặc che khuất nút Submit.

### 2. Bảng điều khiển tổng quan (Overview Dashboard)
*   **Responsive Grid:** Lưới thẻ chỉ số (Stats Cards) tự động cấu hình lại cột tùy theo kích thước màn hình: 1 cột trên Mobile, 2 cột trên Tablet và 4 cột trên Desktop.
*   **Dòng chảy thông tin:** Các biểu đồ phân tích và danh sách "Top sinh viên" tự động xếp dọc (Vertical Stack) một cách mượt mà trên thiết bị di động, đảm bảo biểu đồ không bị bóp méo hay tràn viền.

### 3. Trang Quản lý lớp học chi tiết (View Class)
*   **Hệ thống Tabs tương tác:** Thay vì nhồi nhét thông tin danh sách sinh viên, điểm số và môn học lên cùng một trang gây quá tải thông tin (Information Overload), SIMS sử dụng Tab switcher. Người dùng có thể chuyển đổi mượt mà giữa "Danh sách sinh viên" và "Danh sách môn học" (Subject List) chỉ với một cú click chuột tại chỗ.

### 4. Trang Nộp bài tập (Assignment Submission)
*   **Bố cục luồng dọc (Vertical Flow):** Thiết kế dạng dòng chảy một cột từ trên xuống giúp học sinh dễ dàng nắm bắt: Đề bài -> Hướng dẫn -> Trạng thái nộp.
*   **Hệ thống Badges trực quan:** Trạng thái bài tập được chuẩn hóa màu sắc (Đỏ cho Overdue/Chưa nộp, Vàng cho Chờ chấm điểm, Xanh lục cho Đã hoàn thành).
*   **Khu vực kéo thả file (Upload Zone):** Thiết kế dạng khung đứt nét nét mảnh (dashed border) kéo rộng toàn màn hình (full-width), có hiệu ứng đổi màu khi rê file vào, tạo chỉ dẫn hành động cực kỳ rõ ràng.

### 5. Modal Thêm tài liệu (Upload Resource Modal)
*   **Tinh giản thông tin:** Lược bỏ các trường nhập liệu dư thừa như Subject ID và File Type. Hệ thống tự động nhận diện dựa trên ngữ cảnh môn học hiện tại và định dạng file tải lên.
*   **Layout cân đối:** Chia tỷ lệ các input hàng đầu theo tỷ lệ 50/50 cân xứng, nút lưu chính đặt ở góc phải dưới cùng theo đúng quy luật quét mắt mắt của người dùng.

---

## Tính năng tương tác nâng cao (Advanced Interaction Features)

### 1. Clickable Table Row (Hàng bảng có thể click)
*   Áp dụng trên các bảng danh sách như Department List hay Student List. Toàn bộ hàng có thuộc tính `cursor-pointer` và hiệu ứng `hover:bg-slate-50` báo hiệu có thể click được.
*   *Xử lý kỹ thuật:* Để tránh việc click vào các nút hành động CRUD (Sửa, Xóa) kích hoạt luôn sự kiện click của cả hàng (xem chi tiết), các hàm xử lý nút bấm được chèn mã chặn nổi bọt sự kiện:
    ```javascript
    const handleDelete = (e, id) => {
      e.stopPropagation(); // Ngăn chặn sự kiện lan truyền lên thẻ <tr>
      // Thực hiện logic xóa ở đây
    };
    ```

### 2. Thiết kế đáp ứng di động (Mobile Responsive Layouts)
*   **Hamburger Navigation:** Trên thiết bị di động, thanh Sidebar cố định bên trái sẽ thu gọn hoàn toàn và chỉ xuất hiện dưới dạng Drawer Menu (Menu ngăn kéo) khi nhấn vào nút Hamburger ở góc màn hình.
*   **Local Scrolling:** Tất cả bảng dữ liệu lớn đều được bao bọc trong thẻ div cấu hình `overflow-x-auto`. Điều này cho phép cuộn ngang nội dung bảng trên di động mà không gây vỡ cấu trúc layout chung của ứng dụng.

### 3. Biểu diễn các trạng thái giao diện (UI States Representation)
*   **Loading State (Skeleton Screen):** Sử dụng các khung xương màu xám nhạt chuyển động nhấp nháy mượt mà (chạy animation mạch xung) thay cho vòng xoay loading cổ điển, giúp giảm cảm giác chờ đợi của người dùng.
*   **Empty State (Trạng thái rỗng):** Khi bảng không có dữ liệu, hệ thống hiển thị một hình minh họa tối giản kèm thông điệp hướng dẫn cụ thể thay vì để màn hình trắng trơn.
*   **Validation State:** Các Form nhập liệu kiểm tra lỗi thời gian thực. Khi có lỗi phát sinh, viền input lập tức đổi sang màu đỏ (`border-rose-500`) kèm dòng tin nhắn cảnh báo nhỏ ngay phía dưới.

---

## Cấu trúc thư mục Front-end (Project Structure)

Dưới đây là cấu trúc tổ chức mã nguồn sạch, chuẩn hóa mô-đun của dự án Front-end React:

```text
frontend/
├── public/                 # Các tài nguyên tĩnh (Logo, Ảnh nền, Favicon)
├── src/
│   ├── api/                # Cấu hình gọi API (Axios client, endpoints)
│   ├── assets/             # CSS toàn cục và các tài nguyên hình ảnh dùng chung
│   ├── components/         # Các thành phần giao diện tái sử dụng
│   │   ├── Layout/         # Khung giao diện chính hệ thống
│   │   │   ├── AdminLayout.jsx      # Bố cục cho Admin
│   │   │   ├── LecturerLayout.jsx   # Bố cục cho Giảng viên
│   │   │   ├── StudentLayout.jsx    # Bố cục cho Sinh viên
│   │   │   ├── Sidebar.jsx          # Thanh điều hướng bên
│   │   │   └── Navbar.jsx           # Thanh công cụ phía trên
│   │   └── Shared/         # Component dùng chung (Buttons, Modals, Tables)
│   ├── context/            # Quản lý trạng thái toàn cục (AuthContext, ThemeContext)
│   ├── hooks/              # Các Custom Hooks xử lý logic độc lập
│   ├── pages/              # Phân hệ giao diện theo vai trò người dùng
│   │   ├── Admin/          # Các trang dành cho Quản trị viên (User, Dept...)
│   │   ├── Auth/           # Các trang đăng nhập, phân quyền truy cập
│   │   ├── Home/           # Trang chủ giới thiệu
│   │   ├── Instructor/     # Giao diện của Giảng viên (Grade, Assignments...)
│   │   └── Students/       # Giao diện của Sinh viên (Dashboard, Submission...)
│   ├── routes/             # Cấu hình định tuyến và phân quyền Route bảo vệ
│   ├── App.css             # CSS tùy chỉnh mức Component
│   ├── index.css           # Cài đặt Tailwind CSS tokens và phong cách nền tảng
│   └── main.jsx            # Điểm khởi chạy ứng dụng React
├── eslint.config.js        # Cấu hình chuẩn hóa code (Linter)
├── package.json            # Quản lý thư viện phụ thuộc và kịch bản chạy lệnh
└── vite.config.js          # Cấu hình đóng gói và tối ưu hóa Vite
```

---

## Hướng dẫn cài đặt & Chạy ứng dụng

Hãy chắc chắn rằng máy tính của bạn đã được cài đặt sẵn **Node.js** (tối thiểu phiên bản v18) và trình quản lý gói **npm**.

### 1. Tải dự án và di chuyển vào thư mục Frontend
```bash
git clone <link-kho-luu-tru>
cd frontend
```

### 2. Cài đặt các thư viện phụ thuộc
Chạy lệnh sau để tải toàn bộ các gói giao diện thiết yếu:
```bash
npm install
```

### 3. Khởi chạy môi trường Phát triển (Local Development)
Chạy dự án ở chế độ HMR (Hot Module Replacement) để xem thay đổi giao diện lập tức:
```bash
npm run dev
```
Sau đó truy cập đường dẫn cục bộ trên trình duyệt: `http://localhost:5173`.

### 4. Đóng gói sản phẩm (Build Production)
Biên dịch và tối ưu hóa mã nguồn sẵn sàng cho việc đưa lên máy chủ sản xuất:
```bash
npm run build
```
Mã nguồn đã tối ưu sẽ nằm trong thư mục `/dist`.
