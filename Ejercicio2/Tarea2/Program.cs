using System;
using System.Threading;
using System.Collections.Generic;

public class Paciente {
    public int Id { get; set; }
    public int LlegadaHospital { get; set; }
    public int TiempoConsulta { get; set; }
    public int Estado { get; set; } // 0 = EsperaConsulta, 1 = Consulta, 2 = EsperaDiagnostico, 3 = Finalizado
    public bool RequiereDiagnostico { get; set; }

    public Paciente(int id, int llegadaHospital, int tiempoConsulta, bool requiereDiagnostico) {
        this.Id = id;
        this.LlegadaHospital = llegadaHospital;
        this.TiempoConsulta = tiempoConsulta;
        this.Estado = 0; // Empieza en EsperaConsulta
        this.RequiereDiagnostico = requiereDiagnostico;
    }
}

class Program {
    static Random random = new Random();
    static SemaphoreSlim semaforoMedicos = new SemaphoreSlim(4, 4); // 4 médicos 
    static SemaphoreSlim semaforoMaquinas = new SemaphoreSlim(2, 2); // 2 máquinas de diagnóstico
    static int turnoDiagnostico = 1; // Controla el turno de diagnóstico
    static readonly object lockDiagnostico = new object(); // Sincroniza el turno
    delegate void MostrarMensaje(string mensaje); // Delegate para mensaje

    static void Main(string[] args) {
        Console.WriteLine("=== Simulación de pacientes con diagnóstico empezando ===\n");

        List<Thread> pacientes = new List<Thread>();
        int tiempoActual = 0;

        // Crear 4 pacientes
        for (int i = 1; i <= 4; i++) {
            int id = random.Next(1, 101);
            int tiempoConsulta = random.Next(5, 16); // Tiempo de consulta entre 5 y 15 segundos
            bool requiereDiagnostico = random.Next(0, 2) == 1; // 50% de probabilidad
            Paciente paciente = new Paciente(id, tiempoActual, tiempoConsulta, requiereDiagnostico);

            MostrarMensaje mostrar = Console.WriteLine;
            mostrar($"Paciente {paciente.Id}. Llegado el {i}. Estado: EsperaConsulta. Duración Espera: 0 segundos.");
            Thread hilo = new Thread(() => AtenderPaciente(paciente, i, mostrar));
            pacientes.Add(hilo);
            hilo.Start();

            tiempoActual += 2;
            Thread.Sleep(2000); 
        }

        // Esperar a que todos terminen
        foreach (Thread hilo in pacientes) {
            hilo.Join(); 
        }

        Console.WriteLine("\n=== Todos los pacientes han terminado ===");
    }

    static void AtenderPaciente(Paciente paciente, int numeroLlegada, MostrarMensaje mostrar) {
        semaforoMedicos.Wait(); // Esperar médico
        try {
            // Estado Consulta
            paciente.Estado = 1;
            mostrar($"Paciente {paciente.Id}. Llegado el {numeroLlegada}. Estado: Consulta. Duración Espera: 0 segundos.");
            Thread.Sleep(paciente.TiempoConsulta * 1000); // Simular tiempo de consulta
        } finally {
            semaforoMedicos.Release(); // Liberar médico
        }

        // Si requiere diagnóstico
        if (paciente.RequiereDiagnostico) {
            // Estado EsperaDiagnostico
            paciente.Estado = 2;
            mostrar($"Paciente {paciente.Id}. Llegado el {numeroLlegada}. Estado: EsperaDiagnostico. Duración Consulta: {paciente.TiempoConsulta} segundos.");

            // Esperar su turno
            while (true) {
                lock (lockDiagnostico) {
                    if (turnoDiagnostico == numeroLlegada) {
                        semaforoMaquinas.Wait(); // Esperar máquina
                        try {
                            Thread.Sleep(15000); // Simular tiempo de diagnóstico
                            paciente.Estado = 3; // Finalizado
                            mostrar($"Paciente {paciente.Id}. Llegado el {numeroLlegada}. Estado: Finalizado. Duración Diagnóstico: 15 segundos.");
                            turnoDiagnostico++; // Pasar al siguiente turno
                        } finally {
                            semaforoMaquinas.Release(); // Liberar máquina
                        }
                        break; // Salir del bucle
                    }
                }
                Thread.Sleep(100); // Esperar un poco antes de volver a comprobar el turno
            }
        } else {
            // Estado Finalizado
            paciente.Estado = 3;
            mostrar($"Paciente {paciente.Id}. Llegado el {numeroLlegada}. Estado: Finalizado. Duración Consulta: {paciente.TiempoConsulta} segundos.");
        }
    }
}