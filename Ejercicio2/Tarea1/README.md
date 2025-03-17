# Ejercicio 2 - Tarea 1: Unidades de Diagnóstico

## Propósito del Código

Este programa simula 4 pacientes que llegan al hospital, algunos necesitan diagnóstico con máquinas (2 disponibles). Uso .NET 8. Los pacientes van primero a consulta (5-15 segundos) y luego, si hace falta, a diagnóstico (15 segundos). Muestro los estados y tiempos en consola.

## Explicación Técnica

- **Clase Paciente:** Tiene `Id` (aleatorio 1-100), `LlegadaHospital` (0, 2, 4, 6 segundos), `TiempoConsulta` (aleatorio 5-15 segundos), `Estado` (0 = EsperaConsulta, 1 = Consulta, 2 = EsperaDiagnostico, 3 = Finalizado) y `RequiereDiagnostico` (aleatorio true/false).
- **Main:** Creo 4 pacientes con hilos, llegan cada 2 segundos.
- **AtenderPaciente:**
  - Empieza en "EsperaConsulta".
  - Pasa a "Consulta", espera el tiempo de consulta y libera el médico.
  - Si `RequiereDiagnostico` es true, pasa a "EsperaDiagnostico", busca una máquina libre, espera 15 segundos y pasa a "Finalizado".
  - Si no necesita diagnóstico, pasa directo a "Finalizado".

## Respuesta a la Pregunta

### ¿Los pacientes que deben esperar para hacerse las pruebas diagnóstico entran luego a hacerse las pruebas por orden de llegada? Explica qué tipo de pruebas has realizado para comprobar este comportamiento

Sí, los pacientes entran a las pruebas de diagnóstico por orden de llegada porque cada uno termina su consulta en orden (llegan cada 2 segundos y hay 4 médicos), y luego buscan una máquina libre. Como solo hay 2 máquinas, los primeros 2 que necesitan diagnóstico las ocupan, y los siguientes esperan hasta que una se libere.  
**Pruebas:** Corrí el programa varias veces y miré la salida. Por ejemplo, si el Paciente 1 y 2 necesitan diagnóstico, ocupan las máquinas primero. Si el Paciente 3 también lo necesita, espera hasta que el Paciente 1 o 2 termine (después de 15 segundos). El orden de llegada (1, 2, 3, 4) se respeta porque los hilos se ejecutan en secuencia y el lock asegura que las máquinas se asignen en orden.

## Captura de Pantalla

Aquí está la captura de la salida. La hice con "Imp Pant" y la pegué abajo.

---
