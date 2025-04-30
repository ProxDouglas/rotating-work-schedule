# rotating-work-schedule

## Requisitos

Banco de dados: PostgresSql
.Net 8

### Dependencias

dotnet tool install --global dotnet-ef

#### Migration

```
dotnet ef migrations add NomeDaNovaMigration

dotnet ef database update
```

## Database Model

```mermaid
erDiagram

    company {
        integer id PK
        string name
    }

    tenant {
        integer id PK
        integer company_id FK
        string name
    }

    branch {
        integer id PK
        string name
        string country
        string state
        string city
    }

    job_position {
        integer id PK
        string name
        integer workload
        integer maximum_consecutive_days
    }

    <!-- operating_schedule_job {
        integer job_position_id PK
        integer operating_schedule_id PK
        integer branch_id FK
    } -->

    employee {
        integer id PK
        integer branch_id FK
        integer job_position_id FK
        string name
        string email
    }

    unavailability {
        integer id PK
        integer employee_id FK
        datetime start
        datetime end
        strign reason
        date effective_date
        date validity
    }

    operating_schedule {
        integer id PK
        datetime start
        datetime end
        weektime day_week
        datetime canceled
    }

    work_schedule {
        integer id PK
        integer employee_id FK
        datetime start
        datetime end
        datetime canceled
    }

```
