# Sistema de Gestión de Productos 

Este repositorio contiene una solución completa de **Gestión de Productos** implementada con una arquitectura moderna y robusta, diseñada para ser escalable y segura.



##  Tecnologías Utilizadas

El proyecto está dividido en dos partes principales:

*   **Backend**: Desarrollado en **.NET 8 (C#)** usando **ASP.NET Core Web API**.
    *   Arquitectura Limpia (Controlador -> Servicio -> Repositorio).
    *   Entity Framework Core (SQL Server) con **Code First**.
    *   Autenticación y Autorización mediante Cookies Seguras y BCrypt.
    *   Protección contra ataques CSRF/XSRF.
*   **Frontend**: Desarrollado en **Angular 17+**.
    *   Componentes Standalone y Arquitectura Modular.
    *   Diseño responsivo con **Bootstrap 5** y **Bootstrap Icons**.
    *   Manejo de rutas protegido con **AuthGuard**.

## Requisitos Previos

Para ejecutar este proyecto localmente, asegúrate de tener instalado:

1.  **SDK de .NET 8.0**: [Descargar aquí](https://dotnet.microsoft.com/download/dotnet/8.0).
2.  **Node.js (LTS)**: [Descargar aquí](https://nodejs.org/).
3.  **SQL Server** (LocalDB o Express).
4.  **CLI de Angular** (Opcional, se puede usar npx): `npm install -g @angular/cli`.

## Instrucciones de Instalación

Sigue estos pasos para levantar el entorno de desarrollo:

### 1. Configuración de la Base de Datos

El proyecto utiliza Entity Framework Core. Ejecuta las migraciones para crear la base de datos y sembrar el usuario administrador por defecto.


### 2. Iniciar el Backend (.NET API)

Ejecuta el servicio de API. Por defecto correrá en el puerto 5038 (http).

```powershell
dotnet run --project Backend/Backend.csproj
```

### 3. Iniciar el Frontend (Angular)

En una nueva terminal, instala las dependencias y arranca el servidor de desarrollo.

```powershell
cd Frontend
npm install
npm start
```

La aplicación estará disponible en: **http://localhost:4200**

&copy; 2026 
