# TJK Delta - Industrial Rebar Machinery Website & CRM

Industrial machinery catalog, customer inquiry CRM, and content management clone of [tjkserecon.com](https://www.tjkserecon.com) built for **TJK Delta Bulgaria**.

---

## 🛠 Tech Stack

* **Framework**: .NET 8 (ASP.NET Core MVC)
* **ORM & Database**: Entity Framework Core 8
  * Default: SQLite (zero-config, single file)
  * Production ready: MySQL / MariaDB (switchable via config)
* **Frontend UI**: Bootstrap 5 with custom strict industrial dark theme (black & white, sharp rectangular edges, high-contrast typography)
* **Icons**: Bootstrap Icons (CDN)
* **Containerization**: Multi-stage `Dockerfile` (port `8080`)

---

## 📂 Project Structure

```
├── Dockerfile                      # Production multi-stage Docker build
├── .dockerignore                   # Ignores bin, obj, git, local databases
├── .gitignore                      # Standard Visual Studio & DB ignores
├── README.md                       # This documentation
├── tjk/
│   ├── Controllers/
│   │   ├── HomeController.cs       # Public catalog, detail pages, contact form, sitemap
│   │   └── AdminController.cs      # Auth, Inquiries CRM, Machine management CRUD
│   ├── Data/
│   │   ├── AppDbContext.cs         # EF Core context (Inquiries, Categories, Products)
│   │   └── DbInitializer.cs        # Automatic schema seeding (6 categories, 10 machines)
│   ├── Models/
│   │   ├── CatalogModels.cs        # CategoryItem, ProductItem, ViewModels
│   │   └── Inquiry.cs              # Customer lead entity & internal CRM notes
│   ├── Services/
│   │   └── ProductService.cs       # Product catalog queries and CRUD operations
│   ├── Views/
│   │   ├── Home/                   # Public views (Index, Products, Detail, Contact, etc.)
│   │   ├── Admin/                  # Admin views (Login, Inquiries CRM, Machines, MachineForm)
│   │   └── Shared/                 # _Layout.cshtml (Header, footer, dynamic admin links)
│   ├── wwwroot/
│   │   ├── css/tjk-theme.css       # Custom industrial styling
│   │   ├── images/                 # Original SVG/PNG white and dark logos
│   │   │   └── machines/           # Upload directory for new machine photos
│   │   └── robots.txt              # Search engine crawler directives
│   ├── appsettings.json            # Base configuration
│   └── appsettings.Production.json # Production override configuration
```

---

## 🚀 Running Locally

### Prerequisites
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Start the Application
Run the following in the repository root:
```bash
dotnet run --project tjk
```
Open your browser to:
* Public site: `http://localhost:5000` (or the port displayed in terminal)
* Admin panel: `http://localhost:5000/admin`

The SQLite database (`tjk_inquiries.db`) and all seed data (6 categories, 10 machines) will be created automatically on the first run.

---

## 🔐 Admin Panel & CRM Operations

### 1. Access & Authentication
* **URL**: `/admin/login` (unauthenticated visitors to `/admin` are automatically redirected here)
* **Default Username**: `admin`
* **Default Password**: `SlavaNaXi1953?`

> ⚠️ **Important for Production**: Change the password before deploying by setting the environment variable `AdminSettings__Password` in Railway/Render, or updating `AdminSettings:Password` in `appsettings.Production.json`.

### 2. Inquiries CRM (`/admin/inquiries`)
* **Live Lead Capture**: Whenever a visitor submits the form on `/contact` (or asks for a quote on a machine detail page), it arrives instantly in the CRM.
* **Lead Status Workflow**: Each inquiry has a status:
  * `Ново` (New)
  * `В процес` (In progress)
  * `Прегледано` (Reviewed)
  * `Приключено` (Completed / Closed)
* **Filtering**: Filter tabs at the top allow viewing leads by status.
* **Internal CRM Notes**: Below each inquiry message, you can write and save internal team notes (e.g. *"Оферта изпратена на 20.10. Очаква се обаждане"*).
* **Delete**: Delete spam or obsolete inquiries with confirmation.

### 3. Machine Catalog Management (`/admin/machines`)
* **List & Filter**: View all active machinery in the catalog and filter by category.
* **Add New Machine (`/admin/machines/create`)**:
  * Set machine name, slug (auto-generated if empty), category, and description.
  * **Image Options**: Either paste an external image URL **or** upload a JPG/PNG file directly from your computer (saved to `wwwroot/images/machines/`).
  * **Badges**: One-click tags such as `ОКАЗИОН`, `НА СКЛАД`, `ТОП МОДЕЛ`, `НОВА ОФЕРТА`.
  * **Featured Flag**: Check *"Избран модел"* to display the machine prominently on the homepage hero grid.
  * **Specifications & Features**: Add key features and technical specs in `Key: Value` format (e.g., `Диаметър: Ø5 - 12 mm`).
* **Edit & Delete**: Update existing machines or remove discontinued models at any time.

---

## ☁️ Deployment Guide (Railway & Render)

Both Railway and Render natively support the included `Dockerfile` with zero manual build configuration.

### Option A: Deploying on Railway (Recommended)

Railway is typically the fastest setup for .NET + SQLite/MySQL:

1. **Sign Up / Log In**: Go to [railway.app](https://railway.app) and sign in with GitHub.
2. **Create New Project**:
   * Click **"New Project"** -> Select **"Deploy from GitHub repo"**.
   * Choose your repository: `Pengustus/tjk`.
3. **Automatic Docker Detection**:
   * Railway detects the root `Dockerfile` and starts building automatically.
4. **Configure Environment Variables**:
   * Click on the service -> Go to **"Variables"** -> Add:
     * `PORT`: `8080`
     * `ASPNETCORE_ENVIRONMENT`: `Production`
     * `AdminSettings__Password`: `YourSecureAdminPassword123!`
5. **Persistent Data Storage (For SQLite & Uploads)**:
   * To keep inquiries and uploaded images safe across code deployments:
   * Click on your service -> **"Settings"** -> scroll to **"Volumes"** -> **"Add Volume"**.
   * Mount path: `/app/data`
   * *(Alternative)*: You can click "+ New" in Railway and add a 1-click **MySQL** database (sets up automatically without volumes).
6. **Generate Domain**:
   * In **"Settings"** -> **"Networking"** -> Click **"Generate Domain"** to get a free `xxx.up.railway.app` URL for testing.

---

### Option B: Deploying on Render

1. **Sign Up / Log In**: Go to [render.com](https://render.com) and log in with GitHub.
2. **Create Web Service**:
   * Click **"New +"** -> Select **"Web Service"**.
   * Connect your GitHub repository `Pengustus/tjk`.
3. **Settings**:
   * **Name**: `tjk-delta` (or your choice)
   * **Language**: `Docker`
   * **Branch**: `master`
   * **Region**: Frankfurt (EU Central) - best latency for Bulgaria
   * **Instance Type**: Free or Starter
4. **Environment Variables**:
   * Add the following:
     * `PORT`: `8080`
     * `ASPNETCORE_ENVIRONMENT`: `Production`
     * `AdminSettings__Password`: `YourSecureAdminPassword123!`
5. **Health Check Path**:
   * Set **Health Check Path** to `/health`.
6. **Deploy**:
   * Click **"Create Web Service"**. Render will build the Docker container and deploy it to a free `onrender.com` URL.

---

## 🌐 Custom Domain & DNS Setup

Once your site is running on Railway or Render, connect your custom domain (e.g. `tjkdelta.com` or `www.tjkdelta.com`):

### 1. In Railway / Render Dashboard
* Go to your service **Settings** -> **Custom Domains**.
* Add two domains:
  1. `yourdomain.com` (Apex domain)
  2. `www.yourdomain.com` (Subdomain)
* The dashboard will provide the exact DNS records to enter:
  * For `www`: A **CNAME** pointing to `your-app.up.railway.app` or `your-app.onrender.com`.
  * For root `@`: An **ALIAS / ANAME** or **A** record pointing to the IP address provided by the host.

### 2. In Your Domain Registrar (Cloudflare, Namecheap, SuperHosting, GoDaddy)
* Log in to your domain registrar's DNS management panel.
* Add the records provided:
  | Type | Name / Host | Target / Value | TTL |
  | :--- | :--- | :--- | :--- |
  | `CNAME` | `www` | `your-app.up.railway.app` (or Render target) | Auto / 300 |
  | `ALIAS` or `A` | `@` | (IP or alias supplied by Railway/Render) | Auto / 300 |
* **SSL Certificate**: Both Railway and Render automatically issue and renew a free Let's Encrypt SSL/TLS certificate within 5–15 minutes of DNS propagation.

### 3. Update Company Settings
In `appsettings.Production.json` or via environment variables, verify your contact details:
* `CompanySettings__Domain`: `tjkdelta.com`
* `CompanySettings__Email`: `office@tjkdelta.com`
* `CompanySettings__Phone`: `+359 2 ...`

---

## 🔍 SEO, Robots & Health Checks

* **Robots.txt**: Located at `/robots.txt`. Allows search engines to index public catalog pages while barring `/admin/`.
* **Dynamic Sitemap**: Accessible at `/sitemap.xml`. Automatically includes all machine model detail URLs and category pages generated straight from the database.
* **Health Check**: Accessible at `/health`. Used by Docker, Render, and Kubernetes to monitor container status.
