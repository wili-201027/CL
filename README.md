# Terminal (CL)

Un clon de terminal interactivo desarrollado en .NET 8 y Windows Forms. 

---

⚠️ **DESCARGO DE RESPONSABILIDAD / DISCLAIMER**
Este proyecto ha sido desarrollado estrictamente con fines **educativos y de entretenimiento por fanáticos (Fan Project)**. 
* El autor **no posee** los derechos de propiedad intelectual, marcas comerciales ni personajes asociados con el concepto original en el que se basa esta aplicación.
* Todos los derechos pertenecen a sus respectivos dueños legales. 
* No se persigue ningún fin de lucro ni uso comercial con este repositorio.

---

## 🚀 Características
* Interfaz basada en **Windows Forms** y ejecutada sobre **.NET 8.0-windows**.
* Sistema de reloj (`Clock.cs`) integrado con elementos interactivos de terminal.
* Estructura modular y adaptable.

## 🛠️ Requisitos e Instalación
Para clonar, compilar y ejecutar este proyecto de forma local:

1. Asegúrate de tener instalado el SDK de [.NET 8.0](https://microsoft.com).
2. Abre tu terminal o Git Bash y clona el repositorio:
   ```bash
   git clone https://github.com
   ```
3. Entra al directorio del proyecto:
   ```bash
   cd CL
   ```
4. Restaura las dependencias y ejecuta la aplicación:
   ```bash
   dotnet run --project Terminal.csproj
   ```

## 📁 Estructura del Repositorio
* `VentanaBase.cs`: Ventana principalizada que sirve de plantilla para la UI.
* `Form1.cs`: Ventana de ejecución de la terminal.
