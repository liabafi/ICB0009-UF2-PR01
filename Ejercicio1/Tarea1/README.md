# Ejercicio 1 - Tarea 1: Consulta Médica

## Propósito del Código

Este programa simula 4 pacientes que llegan al hospital cada 2 segundos y son atendidos por 4 médicos durante 10 segundos cada uno.

## Explicación Técnica

- **Main:**
  - Creo una `List<Thread>` para guardar los hilos de los pacientes.
  - Uso un `for` para lanzar 4 pacientes, cada uno con su hilo (`Thread`). Los añado a la lista y espero 2 segundos con `Thread.Sleep(2000)` entre llegadas.
  - Al final, uso `foreach` con `Join()` para esperar a que todos los hilos terminen.
- **AtenderPaciente:**
  - Uso `SemaphoreSlim` con 4 lugares para los médicos. `Wait()` espera a que haya uno libre y `Release()` lo libera.
  - Un delegate `MostrarMensaje` muestra los mensajes con `Console.WriteLine`.
  - Asigno un médico al azar (1 a 4) y espero 10 segundos con `Thread.Sleep(10000)`.

  ## Respuestas a las Preguntas

### ¿Cuántos hilos se están ejecutando en este programa? Explica tu respuesta

Hay 5 hilos: 1 del `Main` y 4 de los pacientes (uno por cada hilo en `List<Thread>`).

### ¿Cuál de los pacientes entra primero en consulta? Explica tu respuesta

El **Paciente 1** entra primero porque el `for` empieza con él y hay 4 médicos libres al inicio.

### ¿Cuál de los pacientes sale primero de consulta? Explica tu respuesta

El **Paciente 1** sale primero porque entra primero y todos tardan 10 segundos.

## Captura de Pantalla

Aquí está la captura de la salida.

![Captura de salida](<Captura de salida.png>)
