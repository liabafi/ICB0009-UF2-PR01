using System;
using System.Threading;
using System.Collections.Generic;

class Program {
    static Random random = new Random(); // Para asignar médicos aleatorios
    static bool medicosOcupados = new bool[4]; // Para saber si un médico está ocupado

    static void Main(string[] args) {
        Console.WriteLine("Inicio de la simulación");

        // Lista para los pacientes
        List<Thread> pacientes = new List<Thread>();

        // Crear 4 pacientes con hilos
        for (int i = 1; i <= 4; i++) {
            int numPaciente = i;
            Thread paciente = new Thread(() => AtenderPaciente(numPaciente));
            pacientes.Add(paciente);
            paciente.Start();
            Thread.Sleep(2000); // Esperar 2 segundo 
        }

        // Esperar a todos los pacientes
        foreach (Thread p in pacientes) {
            p.Join();
        }

        Console.WriteLine("Todo los pacientes han sido atendidos");
    }

    static void AtenderPaciente(int numPaciente) {
        Console.WriteLine("Paciente " + numPaciente + " ha llegado al hospital.");

        // Buscar un médico disponible
        int medicoAsignado = -1;
        lock (medicosOcupados) {
            for (int i = 0; i < 4; i++) {
                if (!medicosOcupados[i]) {
                    medicosOcupados[i] = true;
                    medicoAsignado = i + 1;
                    break;
                }
            }
        }

        if (medicoAsignado != -1) {
            Console.WriteLine("Paciente " + numPaciente + " entra consulta con el médico " + medicoAsignado + ".");
            Thread.Sleep(10000); // Esperar 10 segundos
            Console.WriteLine("Paciente " + numPaciente + " sale consulta con el médico " + medicoAsignado + ".");

            // Liberar al médico
            lock (medicosOcupados) {
                medicosOcupados[medicoAsignado - 1] = false;
            }
        } else {
            Console.WriteLine("Paciente " + numPaciente + " no pudo ser atendido, todos los médicos están ocupados.");
        }
    }
}