# TalentoPlus - Employee Management System

Complete system for employee management at TalentoPlus S.A.S., composed of a REST API and an MVC web application.

## 🔗 Repository

[Repository Link](https://github.com/JuanDavidBarr/TalentoPlus)

---

## 📊 Entity-Relationship Diagram

```mermaid
erDiagram
    DEPARTMENT ||--o{ EMPLOYEE : "has"
    POSITION ||--o{ EMPLOYEE : "has"
    
    DEPARTMENT {
        int Id PK
        string Name
        string Description
    }
    
    POSITION {
        int Id PK
        string Name
        string Description
    }
    
    EMPLOYEE {
        int Id PK
        string FirstName
        string LastName
        string DocumentNumber
        string Email
        string Phone
        date BirthDate
        string Address
        date HireDate
        string Status
        decimal Salary
        string EducationLevel
        string ProfessionalProfile
        int PositionId FK
        int DepartmentId FK
    }
```

---

## 👤 Use Case Diagram

```mermaid
flowchart TB
    subgraph Actors
        U[👤 User/Employee]
        A[👨‍💼 Administrator]
    end
    
    subgraph "Use Cases - Authentication"
        CU1[Register]
        CU2[Login]
        CU3[View My Information]
    end
    
    subgraph "Use Cases - Employee Management"
        CU4[List Employees]
        CU5[View Employee Details]
        CU6[Create Employee]
        CU7[Edit Employee]
        CU8[Delete Employee]
    end
    
    subgraph "Use Cases - Features"
        CU9[Import from Excel]
        CU10[Export to Excel]
        CU11[Generate Resume PDF]
        CU12[Send Resume by Email]
    end
    
    U --> CU1
    U --> CU2
    U --> CU3
    U --> CU11
    
    A --> CU4
    A --> CU5
    A --> CU6
    A --> CU7
    A --> CU8
    A --> CU9
    A --> CU10
    A --> CU11
    A --> CU12
```

---

## 🔄 Flow Diagram - Main Application Flow

```mermaid
flowchart TD
    A[Start] --> B{User authenticated?}
    B -->|No| C[Show Login/Register]
    C --> D{Action}
    D -->|Register| E[Registration Form]
    E --> F[Validate Data]
    F -->|Valid| G[Create Employee]
    G --> H[Send Welcome Email]
    H --> I[Redirect to Login]
    F -->|Invalid| E
    
    D -->|Login| J[Login Form]
    J --> K[Validate Credentials]
    K -->|Valid| L[Generate JWT Token]
    L --> M[System Access]
    K -->|Invalid| J
    
    B -->|Yes| M
    M --> N[Employee Dashboard]
    
    N --> O{User Action}
    O -->|View List| P[List Employees]
    O -->|Create| Q[New Employee Form]
    O -->|Edit| R[Edit Employee Form]
    O -->|Delete| S[Confirm Deletion]
    O -->|Import Excel| T[Upload Excel File]
    O -->|Export Excel| U[Download Excel]
    O -->|Generate PDF| V[Generate Resume]
    
    Q --> W[Save to DB]
    R --> W
    S --> X[Delete from DB]
    T --> Y[Process Excel]
    Y --> W
    
    W --> N
    X --> N
    U --> N
    V --> Z[Download PDF]
    Z --> N
```

---

## 🔄 Flow Diagram - REST API

```mermaid
flowchart LR
    subgraph Client
        WEB[MVC Web App]
    end
    
    subgraph API[REST API - Port 5001]
        AUTH[AuthController]
        EMP[EmployeesController]
        EXCEL[ExcelImportController]
        RESUME[ResumeController]
    end
    
    subgraph Services
        AS[AuthService]
        ES[EmployeeService]
        EXS[ExcelImportService]
        RS[ResumeService]
        EMS[EmailService]
        JS[JwtService]
    end
    
    subgraph Data
        REPO[EmployeeRepository]
        DB[(PostgreSQL)]
    end
    
    WEB -->|HTTP| AUTH
    WEB -->|HTTP| EMP
    WEB -->|HTTP| EXCEL
    WEB -->|HTTP| RESUME
    
    AUTH --> AS
    AUTH --> RS
    EMP --> ES
    EXCEL --> EXS
    RESUME --> RS
    
    AS --> JS
    AS --> EMS
    AS --> REPO
    ES --> REPO
    EXS --> REPO
    RS --> REPO
    
    REPO --> DB
```

---

## 🚀 Steps to Run the Solution

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/) installed
- [Docker Compose](https://docs.docker.com/compose/install/) installed

### Execution

1. **Clone the repository:**
   ```bash
   git clone https://github.com/JuanDavidBarr/TalentoPlus.git
   cd TalentoPlus
   ```

2. **Create the environment variables file:**
   ```bash
   cp .env.example .env
   ```
   > Edit the `.env` file with the provided credentials (see configuration section).

3. **Start the containers:**
   ```bash
   docker-compose up --build -d
   ```

4. **Verify that containers are running:**
   ```bash
   docker-compose ps
   ```

5. **Access the application:**
   - **Web App:** http://localhost:5000
   - **API (Swagger):** http://localhost:5001/swagger

### Useful Commands

```bash
# View logs in real time
docker-compose logs -f

# Stop the containers
docker-compose down

# Restart the containers
docker-compose restart
```

---

## ⚙️ Environment Variables Configuration

Create a `.env` file in the project root with the following variables:

```env
# Database
DB_HOST=<database_host>
DB_PORT=5432
DB_NAME=<database_name>
DB_USER=<username>
DB_PASSWORD=<password>

# JWT
JWT_SECRET=<jwt_secret_key>
JWT_ISSUER=TalentoPlusAPI
JWT_AUDIENCE=TalentoPlusClients
JWT_EXPIRATION_HOURS=24

# SMTP (for sending emails)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=<email>
SMTP_PASSWORD=<app_password>
SMTP_FROM_NAME=Web app - TalentoPlus
SMTP_FROM_EMAIL=<email>
```

---

## 🔑 Access Credentials

### Database (PostgreSQL)

| Field    | Value                     |
|----------|---------------------------|
| Host     | 157.90.251.124            |
| Port     | 5432                      |
| Database | JuanDavid_Prueba          |
| Username | riwi_user                 |
| Password | J9YoXTAy77bVPxwMtArRHfXDC |

### Web Application

| Field | Value                 |
|-------|-----------------------|
| URL   | http://localhost:5000 |

### API

| Field   | Value                         |
|---------|-------------------------------|
| URL     | http://localhost:5001         |
| Swagger | http://localhost:5001/swagger |

---

## 🏗️ Architecture

```
TalentoPlus/
├── docker-compose.yml          # Container orchestration
├── .env                        # Environment variables (not versioned)
├── .env.example                # Variables template
├── TalentoPlusAPI/             # REST API (.NET 8)
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── EmployeesController.cs
│   │   ├── ExcelImportController.cs
│   │   └── ResumeController.cs
│   ├── Services/
│   ├── Repositories/
│   ├── Models/
│   ├── DTOs/
│   └── Dockerfile
└── TalentoPlusWeb/             # MVC Web Application (.NET 8)
    ├── Controllers/
    ├── Views/
    ├── Services/
    └── Dockerfile
```

---

## 📋 API Endpoints

| Method | Endpoint                  | Description                |
|--------|---------------------------|----------------------------|
| GET    | /api/employees            | List all employees         |
| GET    | /api/employees/{id}       | Get employee by ID         |
| POST   | /api/employees            | Create new employee        |
| PUT    | /api/employees/{id}       | Update employee            |
| DELETE | /api/employees/{id}       | Delete employee            |
| POST   | /api/auth/register        | Employee self-registration |
| POST   | /api/auth/login           | Login                      |
| GET    | /api/auth/departments     | List departments           |
| POST   | /api/excelimport/upload   | Import employees from Excel|
| GET    | /api/resume/employee/{id} | Generate resume PDF        |

---

## 👤 Author

- **Name:** Juan David
- **Project:** TalentoPlus Technical Test
