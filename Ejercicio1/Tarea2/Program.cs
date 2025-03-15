using System;
using System.Threading;
using System.Collections.Generic;

// Proyecto para .NET 8, simulamos pacientes con datos en un hospital
public class Paciente {
    public int Id { get; set; }
    public int LlegadaHospital { get; set; }
    public int TiempoConsulta { get; set; }
    public int Estado { get; set; } 

    public Paciente(int id, int llegadaHospital, int tiempoConsulta) {
        Id = id;
        LlegadaHospital = llegadaHospital;
        TiempoConsulta = tiempoConsulta;
        this.Estado = 0; // 0: Esperando, 1: Atendiendo, 2: Finalizado
    }
}

class Program {
    static Random random = new Random(); // Para asignar mnumeros aleatorios
    static bool[] medicosOcupados = new bool[4]; // Para saber si un médico está ocupado

    static void Main(string[] args) {
        Console.WriteLine("Inicio de la simulación");

        List<Thread> pacientes = new List<Thread>();
        int tiempoActual = 0; // Tiempo en segundos 

        // Crear 4 pacientes 
        for (int i = 1; i <= 4; i++) {
            int id = random.Next(1, 101); // Id del paciente
            int tiempoConsulta = random.Next(5, 16); // Tiempo de consulta
            Paciente paciente = new Paciente(id, tiempoActual, tiempoConsulta);

            Thread hiloPaciente = new Thread(() => AtenderPaciente(paciente, i));
            pacientes.Add(hiloPaciente);
            hiloPaciente.Start();

            tiempoActual += 2; // Cada paciente llega cada 2 segundos
            Thread.Sleep(2000); // Esperar 2 segundos
        }

        // Esperar que todos terminen
        foreach (Thread p in pacientes) {
            p.Join();
        }

        Console.WriteLine("Todos los pacientes han sido atendidos");
    }

    static void AtenderPaciente(Paciente paciente, int numeroLlegada) {
        // Mostrar llegada
        Console.WriteLine($"Paciente {numeroLlegada} (ID: {paciente.Id}) ha llegado al hospital en {paciente.LlegadaHospital}");

        // Buscar médico disponible
        int medicoAsignado = -1;
        lock (medicosOcupados) {
            for (int i = 0; i < 4; i++) {
                if (!medicosOcupados[i]) {
                    medicosOcupados[i] = true;
                    medicoAsignado = i + 1; // Medicos del 1 al 4
                    break;
                }
            }
        }

        if (medicoAsignado != -1) {
            paciente.Estado = 1; // Atendiendo
            Console.WriteLine($"Paciente {numeroLlegada} (ID: {paciente.Id}) está siendo atendido por el médico {medicoAsignado}");
            Thread.Sleep(paciente.TiempoConsulta * 1000); // Simular tiempo de consulta

            paciente.Estado = 2; // Finalizado
            Console.WriteLine($"Paciente {numeroLlegada} (ID: {paciente.Id}) sale de la consulta tras {paciente.TiempoConsulta}s");

            // Liberar médico
            lock (medicosOcupados) {
                medicosOcupados[medicoAsignado - 1] = false;
            }
        } else {
            Console.WriteLine($"Paciente {numeroLlegada} (ID: {paciente.Id}) no pudo ser atendido, no hay médicos disponibles");
        }
    }
}