using System;
using System.Threading;
using System.Collections.Generic;

// Proyecto para .NET 8, simulamos pacientes y mostramos sus estados
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
    static bool[] medicosOcupados = new bool[4]; // 4 médicos, true si están ocupados

    static void Main(string[] args) {
        Console.WriteLine("=== Simulación de pacientes empezando ===\n");

        List<Thread> pacientes = new List<Thread>();
        int tiempoActual = 0; // Tiempo desde que empieza

        // Crear 4 pacientes
        for (int i = 1; i <= 4; i++) {
            int id = random.Next(1, 101); // ID aleatorio de 1 a 100
            int tiempoConsulta = random.Next(5, 16); // Entre 5 y 15 segundos
            Paciente paciente = new Paciente(id, tiempoActual, tiempoConsulta);

            Thread hiloPaciente = new Thread(() => AtenderPaciente(paciente, i));
            pacientes.Add(hiloPaciente);
            hiloPaciente.Start();

            tiempoActual += 2; // Cada paciente llega 2 segundos después
            Thread.Sleep(2000); // Esperar 2 segundos entre llegadas
        }

        // Esperar a que todos terminen
        foreach (Thread p in pacientes) {
            p.Join();
        }

        Console.WriteLine("=== Todos los pacientes han terminado === ");
    }

    static void AtenderPaciente(Paciente paciente, int numeroLlegada) {
        // Estado Espera (al llegar)
        string estadoText = paciente.Estado == 0 ? "Espera" : "Consulta";
        Console.WriteLine($"Paciente {paciente.Id}. Llegado el {numeroLlegada}. Estado: {estadoText}. Duración Espera: 0 segundos");
        Console.WriteLine();

        // Buscar médico libre
        int medicoAsignado = -1;
        lock (medicosOcupados) {
            for (int i = 0; i < 4; i++) {
                if (!medicosOcupados[i]) {
                    medicosOcupados[i] = true;
                    medicoAsignado = i + 1; // Médico 1 a 4
                    break;
                }
            }
        }

        if (medicoAsignado != -1) {
            // Estado Consulta (al entrar)
            paciente.Estado = 1;
            estadoText = paciente.Estado == 1 ? "Consulta" : "Espera";
            Console.WriteLine($"Paciente {paciente.Id}. Llegado el {numeroLlegada}. Estado: {estadoText}. Duración Espera: 0 segundos");

            Thread.Sleep(paciente.TiempoConsulta * 1000); // Tiempo en consulta

            // Estado Finalizado (al salir)
            paciente.Estado = 2;
            estadoText = paciente.Estado == 2 ? "Finalizado" : "Consulta";
            Console.WriteLine($"Paciente {paciente.Id}. Llegado el {numeroLlegada}. Estado: {estadoText}. Duración Consulta: {paciente.TiempoConsulta} segundos");
            Console.WriteLine();
            
            // Liberar médico
            lock (medicosOcupados) {
                medicosOcupados[medicoAsignado - 1] = false;
            }
        } else {
            Console.WriteLine($"Paciente {paciente.Id} no encuentra médico (no debería pasar).");
        }
    }
}