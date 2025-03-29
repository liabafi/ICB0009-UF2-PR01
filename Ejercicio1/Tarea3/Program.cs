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
    static Random random = new Random();
    static SemaphoreSlim semaforoMedicos = new SemaphoreSlim(4, 4); // 4 médicos
    delegate void MostrarMensaje(string mensaje); // Delegate para mensajes

    static void Main(string[] args) {
        Console.WriteLine("=== Simulación de pacientes empezando ===\n");

        List<Thread> paciente = new List<Thread>(); // Lista de hilos
        int tiempoActual = 0;

        // Crear 4 pacientes
        for (int i = 1; i<= 4; i++) {
            int id = random.Next(1, 101); // Numero aleatorio entre 1 y 100
            int tiempoConsulta = random.Next(5, 16); // 5 a 15 segundos
            Paciente pacientes = new Paciente(id, tiempoActual, tiempoConsulta);

            MostrarMensaje mostrar = Console.WriteLine; // Delegate
            mostrar($"Paciente {pacientes.Id}. Llegado el {i}. Estado: Espera. Duración Espera: 0 segundos");
            Thread hilo = new Thread(() => AtenderPaciente(pacientes, i, mostrar));
            paciente.Add(hilo); // Agregar hilo a la lista
            hilo.Start(); // Iniciar hilo

            tiempoActual += 2;
            Thread.Sleep(2000); // Esperar 2 segundos entre pacientes
        }

        // Esperar a que todos los hilos terminen
        foreach (Thread hilo in paciente) {
            hilo.Join();
        }

        Console.WriteLine("\n=== Simulación de pacientes finalizada ===");
    }

    static void AtenderPaciente(Paciente paciente, int numeroLlegada, MostrarMensaje mostrar) {
        semaforoMedicos.Wait(); // Esperar a que un médico esté disponible
        try {
            paciente.Estado = 1; // Cambiar estado a consulta
            mostrar($"Paciente {paciente.Id}. Llegado el {numeroLlegada}. Estado: Consulta. Duración Espera: {paciente.LlegadaHospital} segundos");
            Thread.Sleep(paciente.TiempoConsulta * 1000); // Simular tiempo de consulta

            // Estado finalizado
            paciente.Estado = 2; 
            mostrar($"Paciente {paciente.Id}. Llegado el {numeroLlegada}. Estado: Finalizado. Duración Consulta: {paciente.TiempoConsulta} segundos");
        } finally {
            semaforoMedicos.Release(); // Liberar el médico
        }
    }
}