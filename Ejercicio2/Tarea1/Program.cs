using System;
using System.Threading;
using System.Collections.Generic;

public class Paciente {
    public int Id { get; set; }
    public int LlegadaHospital { get; set; }
    public int TiempoConsulta { get; set; }
    public int Estado { get; set; } // 0 = Espera, 1 = Consulta, 2 = Diagnostico, 3 = Finalizado
    public bool RequiereDiagnostico { get; set; } // True si necesita diagnóstico

    public Paciente(int id, int llegadaHospital, int tiempoConsulta, bool requiereDiagnostico) {
        this.Id = id;
        this.LlegadaHospital = llegadaHospital;
        this.TiempoConsulta = tiempoConsulta;
        this.Estado = 0; 
        this.RequiereDiagnostico = requiereDiagnostico;
    }
}

class Program {
    static Random random = new Random(); // Generador de números aleatorios
    static bool[] medicosOcupados = new bool[4]; // 4 médicos
    static bool[] maquinasOcupadas = new bool[2]; // 2 máquinas de diagnóstico

    static void Main(string[] args) {
        Console.WriteLine("=== Simulación de pacientes con diagnóstico empezando ===");
        
        List<Thread> pacientes = new List<Thread>();
        int tiempoActual = 0;

        // Crear 4 pacientes
        for (int i = 1; i <= 4; i++) {
            int id = random.Next(1, 101); // ID aleatorio
            int tiempoConsulta = random.Next(5, 16); // Entre 5 y 15 segundos
            bool requiereDiagnostico = random.Next(0, 2) == 1; // 50% de probabilidad 
            Paciente paciente = new Paciente(id, tiempoActual, tiempoConsulta, requiereDiagnostico);

            Thread hiloPaciente = new Thread(() => AtenderPaciente(paciente, i));
            pacientes.Add(hiloPaciente);
            hiloPaciente.Start();

            tiempoActual += 2;
            Thread.Sleep(2000); // Esperar 2 segundos
        }

        // Esperar a todos que terminen
        foreach (Thread p in pacientes) {
            p.Join();
        }

        Console.WriteLine("=== Todos los pacientes han terminando ===");
    }

    static void AtenderPaciente(Paciente paciente, int numeroLlegada) {
        // Estado Espera
        string estadoText = paciente.Estado == 0 ? "Espera Consulta" : "Consulta";
        Console.WriteLine($"Paciente {paciente.Id}. Llegado el  {numeroLlegada}. Estado: {estadoText}. Duración Espera: 0 segundos.");
        Console.WriteLine();

        // Buscar médico disponible
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
            // Estado Consulta 
            paciente.Estado = 1;
            estadoText = paciente.Estado == 1 ? "Consulta" : "Espera Consulta";
            Console.WriteLine($"Paciente {paciente.Id}. Llegado el  {numeroLlegada}. Estado: {estadoText}. Duración Espera: 0 segundos.");
            Console.WriteLine();

            Thread.Sleep(paciente.TiempoConsulta * 1000); // Tiempo en consulta

            // Liberar médico
            lock (medicosOcupados) {
                medicosOcupados[medicoAsignado - 1] = false;
            }

            // Si requiere diagnóstico
            if (paciente.RequiereDiagnostico) {
                // Estado EsperaDiagnóstico
                paciente.Estado = 2;
                estadoText = paciente.Estado == 2 ? "Espera Diagnóstico" : "Consulta";
                Console.WriteLine($"Paciente {paciente.Id}. Llegado el  {numeroLlegada}. Estado: {estadoText}. Duración Consulta: {paciente.TiempoConsulta} segundos.");
                Console.WriteLine();

                // Buscar máquina disponible
                int maquinaAsignada = -1;
                lock (maquinasOcupadas) {
                    for (int i = 0; i < 2; i++) {
                        if (!maquinasOcupadas[i]) {
                            maquinasOcupadas[i] = true;
                            maquinaAsignada = i + 1;
                            break;
                        }
                    }
                }

                if (maquinaAsignada != -1) {
                    Thread.Sleep(15000); // 15 segundos para diagnóstico
                    // Estado Finalizado
                    paciente.Estado = 3;
                    estadoText = paciente.Estado == 3 ? "Finalizado" : "Espera Diagnóstico";
                    // Liberar máquina
                    lock (maquinasOcupadas) {
                        maquinasOcupadas[maquinaAsignada - 1] = false;
                    }
                }
            }
        }
    }
}