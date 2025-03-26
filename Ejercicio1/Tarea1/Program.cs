using System;
using System.Threading;
using System.Collections.Generic;

class Program {
    static Random random = new Random();
    static SemaphoreSlim semaforoMedicos = new SemaphoreSlim(4, 4); // 4 médicos
    delegate void MostrarMensaje(string mensaje); // Para mostrar mensajes en consola

    static void Main(string[] args) {
        Console.WriteLine("=== Simulación de atención médica empezando ===\n");

        List<Thread> pacientes = new List<Thread>(); // Lista de pacientes

        // Crear y ejecutar 4 pacientes
        for (int i = 1; i <= 4; i++) {
            int numeroPaciente = i;
            MostrarMensaje mostrar = Console.WriteLine; // Delegate 
            mostrar($"Paciente {numeroPaciente} ha llegado al hospital.");
            Thread paciente = new Thread(() => AtenderPaciente(numeroPaciente, mostrar));
            pacientes.Add(paciente); // Añadir hilo a la lista
            paciente.Start(); // Iniciar hilo
            Thread.Sleep(2000); // Esperar 2 segundos
        }

        // Esperar a que todos los pacientes terminen
        foreach (Thread paciente in pacientes) {
            paciente.Join();
        }

        Console.WriteLine("\n=== Todos los pacientes han sido atendidos ===");
    }

    static void AtenderPaciente(int numeroPaciente, MostrarMensaje mostrar) {
        semaforoMedicos.Wait(); // Esperar a que haya un médico disponible
        try {
            int medicoAsignado = random.Next(1, 5); // Médico aleatorio
            mostrar($"Paciente {numeroPaciente} está siendo atendido por el médico {medicoAsignado}.");
            Thread.Sleep(10000); // Simular tiempo de atención
            mostrar($"Paciente {numeroPaciente} ha sido atendido por el médico {medicoAsignado}.");
        } finally {
            semaforoMedicos.Release(); // Liberar médico
        }
    }
}