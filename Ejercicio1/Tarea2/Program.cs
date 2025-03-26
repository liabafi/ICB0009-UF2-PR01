using System;
using System.Threading;
using System.Collections.Generic;

public class Paciente {
    public int Id { get; set; }
    public int LlegadaHospital { get; set; }
    public int TiempoConsulta { get; set; }
    public int Estado { get; set; } // 0 = Espera, 1 = Consulta, 2 = Finalizado

    public Paciente(int id, int llegadaHospital, int tiempoConsulta) {
        this.Id = id;
        this.LlegadaHospital = llegadaHospital;
        this.TiempoConsulta = tiempoConsulta;
        this.Estado = 0; // Empieza en espera
    }
}

class Program {
    static Random random = new Random(); // Para números aleatorios
    static SemaphoreSlim semaforoMedicos = new SemaphoreSlim(4, 4); // 4 médicos
    delegate void MostrarMensaje(string mensaje); // Delegate para mensajes

    static void Main(string[] args) {
        Console.WriteLine("=== Simulación de atención médica empezando ===\n");

        List<Thread> pacientes = new List<Thread>(); // Lista de hilos
        int tiempoActual = 0;

        // Crear 4 pacientes
        for (int i = 1; i <= 4; i++) {
            int id = random.Next(1, 101); 
            int tiempoConsulta = random.Next(5, 16); // 5 a 15 segundos
            Paciente paciente = new Paciente(id, tiempoActual, tiempoConsulta);

            MostrarMensaje mostrar = Console.WriteLine; // Delegate
            mostrar($"Paciente {paciente.Id} (Orden {i}) ha llegado al hospital en t={paciente.LlegadaHospital}s");
            Thread hilo = new Thread(() => AtenderPaciente(paciente, i, mostrar));
            pacientes.Add(hilo); // Añadir hilo a la lista
            hilo.Start(); // Iniciar hilo

            tiempoActual += 2; 
            Thread.Sleep(2000); // Esperar 2 segundos
        }

        // Esperar a que todos los pacientes terminen
        foreach (Thread hilo in pacientes) {
            hilo.Join();
        }

        Console.WriteLine("\n=== Todos los pacientes han sido atendidos ===");
    }

    static void AtenderPaciente(Paciente paciente, int numeroLlegada, MostrarMensaje mostrar) {
        // Estado Espera
        paciente.Estado = 0;
        mostrar($"Paciente {paciente.Id} (Orden {numeroLlegada}). Estado: Espera");

        semaforoMedicos.Wait(); // Esperar a que haya un médico disponible
        try {
            // Estado Consulta
            paciente.Estado = 1;
            mostrar($"Paciente {paciente.Id} (Orden {numeroLlegada}). Estado: Consulta");
            Thread.Sleep(paciente.TiempoConsulta * 1000); // Tiempo de consulta

            // Estado Finalizado
            paciente.Estado = 2;
            mostrar($"Paciente {paciente.Id} (Orden {numeroLlegada}). Estado: Finalizado tras {paciente.TiempoConsulta}s");
        } finally {
            semaforoMedicos.Release(); // Liberar médico
        }
    }
}