using System;
using System.Threading;
using System.Collections.Generic;
using System.Net.Cache;

public class Paciente {
    public int Id { get; set; }
    public int LlegadaHospital { get; set; }
    public int TiempoConsulta { get; set; }
    public int Estado { get; set; } // 0 = EsperaConsulta, 1 = Consulta, 2 = EsperaDiagnostico, 3 = Finalizado
    public bool RequiereDiagnostico { get; set; }
    public int Prioridad { get; set; } // 1 = Emergencias, 2 = Urgencias, 3 = Consultas generales

    public Paciente(int id, int llegadaHospital, int tiempoConsulta, bool requiereDiagnostico, int prioridad) {
        this.Id = id;
        this.LlegadaHospital = llegadaHospital;
        this.TiempoConsulta = tiempoConsulta;
        this.Estado = 0; // Empieza en EsperaConsulta
        this.RequiereDiagnostico = requiereDiagnostico;
        this.Prioridad = prioridad;
    }
}

class Program {
    static Random random = new Random();
    static SemaphoreSlim semaforoMedicos = new SemaphoreSlim(4, 4); // 4 médicos
    static SemaphoreSlim semaforoMaquinas = new SemaphoreSlim(2, 2); // 2 máquinas
    static List<Paciente> pacientesEnEspera = new List<Paciente>(); // Lista de espera
    static readonly object lockEspera = new object(); // Sincroniza la lista
    delegate void MostrarMensaje(string mensaje); // Delegate para mensajes

    static void Main(string[] args) {
        Console.WriteLine("=== Simulación de 20 pacientes con prioridades empezando ===\n");

        List<Thread> pacientes = new List<Thread>();
        int tiempoActual = 0;

        // Crear 20 pacientes
        for (int i = 1; i <= 20; i++) {
            int id = random.Next(1, 101);
            int tiempoConsulta = random.Next(5, 16);
            bool requiereDiagnostico = random.Next(0, 2) == 1;
            int prioridad = random.Next(1, 4); // Prioridad aleatoria entre 1 y 3
            Paciente paciente = new Paciente(id, tiempoActual, tiempoConsulta, requiereDiagnostico, prioridad);

            MostrarMensaje mostrar = Console.WriteLine;
            mostrar($"Paciente {paciente.Id}. Llegado el {i}. Prioridad: {paciente.Prioridad}. Estado: EsperaConsulta. Duración Espera: 0 segundos.");
            Thread hilo = new Thread(() => AtenderPaciente(paciente, i, mostrar));
            pacientes.Add(hilo);
            hilo.Start();

            tiempoActual += 2;
            Thread.Sleep(2000); // Llega cada 2 segundos
        }

        foreach (Thread hilo in pacientes) {
            hilo.Join(); // Espera a que todos los hilos terminen
        }

        Console.WriteLine("\n=== Todos los pacientes han terminado ===");
    }

    static void AtenderPaciente(Paciente paciente, int numeroLlegada, MostrarMensaje mostrar) {
        // Añadir a la lista de espera
        lock (lockEspera) {
            pacientesEnEspera.Add(paciente);
        }

        // Esperar turno segun prioridad y orden de llegada
        while (true) {
            lock (lockEspera) {
                // Buscar el paciente con menor prioridad y menor tiempo de llegada
                Paciente siguiente = pacientesEnEspera
                    .OrderBy(p => p.Prioridad)
                    .ThenBy(p => p.LlegadaHospital)
                    .FirstOrDefault();

                if (siguiente == paciente && semaforoMedicos.Wait(0)) {
                    pacientesEnEspera.Remove(paciente); // Eliminar de la lista de espera
                    break;
                }
            }
            Thread.Sleep(100); // Espera un poco antes de volver a comprobar
        }

        try {
            // Estado Consulta
            paciente.Estado = 1;
            mostrar($"Paciente {paciente.Id}. Llegado el {numeroLlegada}. Prioridad: {paciente.Prioridad}. Estado: Consulta. Duración Espera: variable.");
            Thread.Sleep(paciente.TiempoConsulta * 1000); // Simula el tiempo de consulta
        } finally {
            semaforoMedicos.Release(); // Libera el médico
        }

        // Si requiere diagnóstico
        if (paciente.RequiereDiagnostico) {
            // Estado EsperaDiagnostico
            paciente.Estado = 2;
            mostrar($"Paciente {paciente.Id}. Llegado el {numeroLlegada}. Prioridad: {paciente.Prioridad}. Estado: EsperaDiagnostico. Duración Consulta: {paciente.TiempoConsulta} segundos.");

            semaforoMaquinas.Wait(); // Espera una máquina
            try {
                Thread.Sleep(15000); // Simula el tiempo de diagnóstico
                // Estado Finalizado
                paciente.Estado = 3;
                mostrar($"Paciente {paciente.Id}. Llegado el {numeroLlegada}. Prioridad: {paciente.Prioridad}. Estado: Finalizado. Duración Diagnóstico: 15 segundos.");
            } finally {
                semaforoMaquinas.Release(); // Libera la máquina
            }
        } else {
            // Estado Finalizado
            paciente.Estado = 3;
            mostrar($"Paciente {paciente.Id}. Llegado el {numeroLlegada}. Prioridad: {paciente.Prioridad}. Estado: Finalizado. Duración Consulta: {paciente.TiempoConsulta} segundos.");
        }
    }
}