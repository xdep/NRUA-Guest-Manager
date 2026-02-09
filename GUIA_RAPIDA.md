# 🚀 GUÍA RÁPIDA - NRUA Guest Manager

## ⚡ Inicio Rápido (5 minutos)

### Paso 1: Instalar .NET (solo la primera vez)

1. Vaya a: https://dotnet.microsoft.com/download/dotnet/6.0
2. Descargue ".NET Desktop Runtime 6.0" para Windows
3. Instale (siguiente, siguiente, finalizar)

### Paso 2: Ejecutar la Aplicación

**Opción A - Modo desarrollo:**
1. Doble clic en `Build.bat` (espere a que compile)
2. Doble clic en `Run.bat`

**Opción B - Crear ejecutable:**
1. Doble clic en `Build-Standalone.bat`
2. Espere (tarda 2-3 minutos)
3. El .exe estará en la carpeta que se abre automáticamente
4. Copie ese .exe donde quiera y ejecútelo

---

## 📝 Uso Básico

### Si tiene datos en Excel:

1. **En Excel**, sus columnas deben ser:
   ```
   NRUA | Check-in | Check-out | Huéspedes | Finalidad
   ```

2. **Guarde como CSV**:
   - Archivo → Guardar como
   - Tipo: CSV UTF-8 (*.csv)
   - Importante: Use punto y coma (;) como separador

3. **En NRUA Guest Manager**:
   - Archivo → Abrir CSV...
   - Seleccione su archivo
   - ¡Listo! Sus datos están importados

4. **Valide**:
   - Botón "✓ Validar Datos"
   - Corrija errores si los hay

5. **Exporte para N2**:
   - Botón "📤 Exportar N2"
   - Guarde el archivo

6. **En la aplicación N2 oficial**:
   - Formulario → Importar datos...
   - Seleccione el archivo que exportó
   - Siguiente → Importar

---

## 🎯 Ejemplo de CSV Válido

```csv
NRUA;checkin;checkout;huespedes;codigo_finalidad
ESHFTU00004501300027277450000000000000000000000000008;15/01/2025;22/01/2025;2;1
ESHFTU00004501300027277450000000000000000000000000008;01/02/2025;08/02/2025;4;1
```

**Códigos de Finalidad:**
- 1 = Vacacional/Turístico
- 2 = Laboral
- 3 = Estudios
- 4 = Causas médicas
- 5 = Otros

---

## ❓ Problemas Comunes

### "No se encuentra dotnet"
→ Instale .NET 6.0 Desktop Runtime

### "Error al abrir CSV"
→ Cierre Excel primero
→ Asegúrese de que sea UTF-8

### "NRUA no válido" en N2
→ Debe tener exactamente 56 caracteres
→ Copie y pegue desde el documento oficial

### Fechas incorrectas
→ Use formato: 15/01/2025 (dd/MM/yyyy)
→ O use: 2025-01-15 (yyyy-MM-dd)

---

## 📞 Más Información

- **README completo**: Vea README.md
- **Manuales oficiales**: https://sede.registradores.org
- **Aplicación N2**: Descargue desde la Sede Electrónica
- **Soporte oficial**: +34 91 270 16 99

---

## ✅ Checklist para el Depósito 2026

- [ ] Tengo todos mis NRUAs del 2025
- [ ] Tengo las fechas de check-in/check-out
- [ ] Tengo el número de huéspedes por reserva
- [ ] He descargado e instalado la aplicación N2
- [ ] He importado mis datos en NRUA Guest Manager
- [ ] He validado todos los datos
- [ ] He exportado el CSV
- [ ] He importado en N2
- [ ] He generado la huella digital en N2
- [ ] He presentado en el registro antes del 2 de marzo

---

**¡Importante!** El plazo es del **1 de febrero al 2 de marzo de 2026**.

No espere al último día. Haga su depósito con tiempo.
