# NRUA Guest Manager
## Gestor de Huéspedes para Depósito de Arrendamientos de Corta Duración

Una aplicación de Windows para gestionar fácilmente los datos de huéspedes para el depósito de arrendamientos de corta duración requerido por el Real Decreto 1312/2024 en España.

---

## 📋 Características

- ✅ **Interfaz visual fácil de usar** - No necesita conocimientos técnicos
- ✅ **Importar desde CSV** - Cargue sus datos existentes
- ✅ **Exportar formato N2** - Genera archivos compatibles con la aplicación N2 oficial
- ✅ **Validación automática** - Detecta errores antes de exportar
- ✅ **Edición manual** - Añada o modifique registros individualmente
- ✅ **Formatos de fecha flexibles** - Acepta dd/MM/yyyy, dd-MM-yyyy, dd.MM.yyyy, yyyy-MM-dd

---

## 🚀 Instalación

### Opción 1: Ejecutar directamente (Requiere .NET)

1. **Instalar .NET 6.0 Runtime** (si no lo tiene):
   - Descargue desde: https://dotnet.microsoft.com/download/dotnet/6.0
   - Seleccione ".NET Desktop Runtime 6.0" para Windows

2. **Compilar la aplicación**:
   ```cmd
   cd NRUAGuestManager
   dotnet build -c Release
   ```

3. **Ejecutar**:
   ```cmd
   dotnet run
   ```

### Opción 2: Crear ejecutable independiente

Para crear un archivo .exe que no requiera .NET instalado:

```cmd
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

El ejecutable estará en: `bin\Release\net6.0-windows\win-x64\publish\NRUAGuestManager.exe`

---

## 📖 Cómo Usar

### 1️⃣ Crear un Nuevo Registro Manualmente

1. Haga clic en **"➕ Añadir Registro"**
2. Complete los campos:
   - **NRUA**: El número completo de 56 caracteres (ej: `ESHFTU00004501300027277450000000000000000000000000008`)
   - **Check-in**: Fecha de entrada
   - **Check-out**: Fecha de salida (opcional)
   - **Nº Huéspedes**: Cantidad de personas
   - **Finalidad**: Seleccione de la lista:
     - Vacacional/Turístico
     - Laboral
     - Estudios
     - Causas médicas
     - Otros

### 2️⃣ Importar desde CSV

Si tiene datos en Excel u otro programa:

1. **Exporte a CSV** desde su programa con este formato:
   ```csv
   NRUA;checkin;checkout;huespedes;codigo_finalidad
   ESHFTU00004501300027277450000000000000000000000000008;15/02/2025;20/02/2025;2;1
   ESHFTU00004501300027277450000000000000000000000000008;25/03/2025;30/03/2025;4;1
   ```

2. En la aplicación: **Archivo → Abrir CSV...**
3. Seleccione su archivo

**Formatos de fecha aceptados**:
- `dd/MM/yyyy` → 15/02/2025
- `dd-MM-yyyy` → 15-02-2025
- `dd.MM.yyyy` → 15.02.2025
- `yyyy-MM-dd` → 2025-02-15

**Códigos de Finalidad**:
- `1` = Vacacional/Turístico
- `2` = Laboral
- `3` = Estudios
- `4` = Causas médicas
- `5` = Otros

### 3️⃣ Validar Datos

Antes de exportar, valide sus datos:

1. Haga clic en **"✓ Validar Datos"**
2. La aplicación verificará:
   - ✅ NRUA tiene 56 caracteres
   - ✅ Fechas son válidas
   - ✅ Check-out es posterior a check-in
   - ✅ Número de huéspedes es razonable

### 4️⃣ Exportar para N2

Cuando sus datos estén listos:

1. Haga clic en **"📤 Exportar N2"** o **Archivo → Exportar para N2...**
2. Guarde el archivo (ej: `N2_export_20250209.csv`)
3. Este archivo está listo para importar en la **aplicación N2 oficial**

### 5️⃣ Importar en la Aplicación N2

1. Abra la **aplicación N2**
2. Vaya a **Formulario → Importar datos...**
3. Seleccione el archivo CSV que exportó
4. Marque "El archivo contiene una fila de cabecera con los nombres de las columnas"
5. Haga clic en **Siguiente** y revise los datos
6. Confirme la importación

---

## 📝 Formato del Archivo CSV

El archivo de exportación tiene este formato (compatible con N2):

```csv
NRUA;checkin;checkout;huespedes;codigo_finalidad
ESHFTU00004501300027277450000000000000000000000000008;01/02/2025;15/02/2025;3;1
ESHFTU00004501300027277450000000000000000000000000008;18/01/2025;;6;1
```

**Campos**:
- **NRUA** (obligatorio): Número de 56 caracteres
- **checkin** (obligatorio): Fecha formato dd/MM/yyyy
- **checkout** (opcional): Fecha formato dd/MM/yyyy (vacío si sigue activo)
- **huespedes** (obligatorio): Número entero mayor que 0
- **codigo_finalidad** (opcional): 1-5 (por defecto 1)

---

## ⚠️ Información Importante

### Sobre el Depósito de Arrendamientos

- **¿Cuándo?**: Debe presentarse en **febrero de cada año**
- **Plazo 2026**: Del 1 de febrero al 2 de marzo
- **¿Qué reportar?**: Todos los arrendamientos iniciados durante el año **2025**
- **¿Quién?**: El titular registral o quien gestione el arrendamiento

### Validaciones que Realiza N2

La aplicación N2 rechazará filas si:
- ❌ NRUA vacío o no válido
- ❌ NRUA no corresponde al CRU del formulario
- ❌ Fecha de entrada vacía o no válida
- ❌ Fecha de entrada no corresponde al ejercicio (2025)
- ❌ Fecha de salida anterior a fecha de entrada
- ❌ Número de huéspedes vacío o no válido
- ❌ Datos duplicados (mismo NRUA + fechas + huéspedes)

---

## 🔧 Solución de Problemas

### Error: "No se puede abrir el archivo CSV"
- Verifique que el archivo no esté abierto en Excel u otro programa
- Asegúrese de que el archivo esté en formato UTF-8

### Error al importar en N2: "NRUA no válido"
- Verifique que todos los NRUA tengan exactamente 56 caracteres
- Copie y pegue los NRUA directamente de los documentos oficiales

### Error: "Fecha de entrada no corresponde al ejercicio"
- Para el depósito 2026, las fechas deben ser del año 2025
- Si tiene reservas de 2024, no deben incluirse en este depósito

### Filas duplicadas en N2
- N2 considera duplicado si NRUA + fecha entrada + fecha salida + huéspedes + finalidad son idénticos
- Esto es normal si la misma propiedad se alquiló varias veces

---

## 📞 Soporte

Para dudas sobre el proceso oficial:
- **Sede Electrónica Registradores**: https://sede.registradores.org
- **Manuales oficiales**: Disponibles en la Sede Electrónica
- **Teléfono**: +34 91 270 16 99 / 902 181 442

---

## 📄 Licencia

Este software es de código abierto y se proporciona "tal cual" sin garantías.
No está afiliado oficialmente con el Colegio de Registradores de España.

---

## 🎯 Ejemplo Práctico

### Caso de Uso: Propietario con 10 reservas en 2025

1. **Prepare sus datos en Excel**:
   ```
   NRUA                                                   | Check-in   | Check-out  | Huéspedes | Finalidad
   ESHFTU00004501300027277450000000000000000000000000008 | 15/01/2025 | 22/01/2025 | 2         | 1
   ESHFTU00004501300027277450000000000000000000000000008 | 01/02/2025 | 08/02/2025 | 4         | 1
   ...
   ```

2. **Guarde como CSV** (Archivo → Guardar como → CSV UTF-8)

3. **Importe en esta aplicación**:
   - Archivo → Abrir CSV...
   - Seleccione su archivo
   - Revise que se hayan importado correctamente

4. **Valide**:
   - Clic en "✓ Validar Datos"
   - Corrija cualquier error detectado

5. **Exporte**:
   - Clic en "📤 Exportar N2"
   - Guarde como `mi_deposito_2025.csv`

6. **Use en N2**:
   - Abra aplicación N2
   - Crear nuevo formulario para 2025
   - Formulario → Importar datos...
   - Seleccione `mi_deposito_2025.csv`
   - Revise y confirme
   - Generar huella digital
   - Presentar en el registro

**¡Listo!** Su depósito está completo.

---

## 💡 Consejos

- 📅 **Haga el depósito en enero** - No espere al último día de febrero
- 💾 **Guarde copias** - Mantenga backups de sus archivos CSV
- ✅ **Valide siempre** - Use el botón de validación antes de exportar
- 📝 **Documente bien** - Anote el ID de trámite que le da N2 tras enviar
- 🔄 **Mantenga actualizado** - Añada reservas según lleguen durante el año

---

## 📚 Referencias

- **Real Decreto 1312/2024**: Normativa oficial
- **Aplicación N2**: Software oficial del Colegio de Registradores
- **Manual N2**: Instrucciones detalladas de uso de N2
- **Manual Presentación**: Guía de presentación telemática

Todos disponibles en: https://sede.registradores.org

---

**Versión**: 1.0.0  
**Fecha**: Febrero 2026  
**Compatibilidad**: Windows 10/11, .NET 6.0+
