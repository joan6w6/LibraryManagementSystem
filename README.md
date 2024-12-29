# LibraryManagementSystem
圖書管理系統
圖書管理系統 是一個基於 ASP.NET Core MVC 的網頁應用程式，用於管理圖書館的借閱和歸還流程，為讀者提供個性化服務，並為管理員提供高效的管理工具。

1.功能

針對管理員
提供借閱與歸還數據。
通過儀表板圖表展示數據趨勢。
管理圖書館藏
增加、更新或刪除圖書資料。
查看圖書的當前可借狀態及借閱歷史記錄。

針對一般讀者
以書籍封面與簡介的形式水平滾動展示。
顯示即將到期（3天內）或已過期的借閱書籍提醒。
包含書名、到期日期。


2.API 端點

借閱統計
GET /api/dashboard/borrow-stats
返回每日和每月的借閱與歸還數據，用於生成趨勢圖表。

推薦書籍
GET /api/dashboard/recommend-books
基於借閱歷史，水平滾動顯示推薦書籍。

到期提醒
GET /api/dashboard/due-reminders
返回當前用戶即將到期或已過期的借閱書籍信息。
新書資訊

3.技術使用
後端：ASP.NET Core MVC
前端：Razor Views、Bootstrap、JavaScript
資料庫：Microsoft SQL Server
身份驗證：Cookie Authentication
消息隊列：RabbitMQ 用於通知服務
日誌記錄：ILogger 用於伺服器端日誌
