using System;
using MySql.Data.MySqlClient; 

namespace Progra3Card.Administrativo
{
    class Program
    {
        private static string connectionString = "Server=localhost;Database=mi_banco_db;Uid=root;Pwd=;";

        static void Main(string[] args)
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("    SISTEMA ADMINISTRATIVO PROGRA3CARD   ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Emitir Nueva Tarjeta (Alta de Cliente)");
                Console.WriteLine("2. Listar Tarjetas");
                Console.WriteLine("3. Ver Detalle de una Tarjeta / Cliente");
                Console.WriteLine("4. Eliminar Tarjeta (Baja de Sistema)");
                Console.WriteLine("5. Emitir Nueva Liquidación Mensual");
                Console.WriteLine("6. Salir");
                Console.WriteLine("========================================");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine())
                {
                    case "1": MenuEmitirTarjeta(); break;
                    case "2": MenuListarTarjetas(); break;
                    case "3": MenuVerDetalleTarjeta(); break;
                    case "4": MenuEliminarTarjeta(); break;
                    case "5": MenuEmitirLiquidacion(); break;
                    case "6": salir = true; break;
                    default:
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // Funciones a completar:

        static void MenuListarTarjetas()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO GENERAL DE TARJETAS ---");
            Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", "Nro Cuenta", "Nro Tarjeta", "Banco Emisor", "DNI Titular");
            Console.WriteLine("----------------------------------------------------------------------");

            // === A realizar ===
            // Aquí deben implementar un SELECT sobre la tabla 'tarjetas'
            // para recorrer las filas e imprimirlas en la consola.
            
            ObtenerYMostrarTarjetas();

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuVerDetalleTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- DETALLE DE TARJETA Y CLIENTE ---");
            Console.Write("Ingrese el Número de Cuenta a consultar: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            // === A realizar ===
            // Aquí deben realizar un SELECT con un JOIN entre 'tarjetas' y 'usuarios' 
            // filtrando por el numCuenta para traer todos los campos (Nombre, Apellido, Email, Saldo, etc.)
            
            MostrarDetalleCompleto(numCuenta);

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEliminarTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR TARJETA DEL SISTEMA ---");
            Console.Write("Ingrese el Número de Cuenta de la tarjeta a dar de baja: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n⚠️ ADVERTENCIA: Se eliminará la tarjeta, sus liquidaciones y los datos de acceso web vinculados.");
            Console.ResetColor();
            Console.Write("¿Está seguro de continuar? (S/N): ");
            
            if (Console.ReadLine().ToUpper() == "S")
            {
                // === A realizar ===
                // Aquí deben ejecutar un DELETE sobre la tabla 'tarjetas' donde num_cuenta = numCuenta.
                // Como definimos ON DELETE CASCADE en la base de datos, las liquidaciones se borrarán solas.
                // Opcional: Evaluar si también eliminan al usuario de la tabla 'usuarios' o si lo mantienen.
                
                bool exito = DarDeBajaTarjeta(numCuenta);

                if (exito)
                    Console.WriteLine("\nTarjeta eliminada correctamente del sistema.");
                else
                    Console.WriteLine("\nError al intentar eliminar la tarjeta. Verifique el número de cuenta.");
            }
            else
            {
                Console.WriteLine("\nOperación cancelada.");
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }


        static void MenuEmitirTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- EMITIR NUEVA TARJETA (ALTA DE CLIENTE) ---");
            
            Console.Write("Ingrese Documento del Cliente: ");
            string documento = Console.ReadLine();
            
            Console.Write("Ingrese Tipo de Documento (DNI/PASAPORTE): ");
            string tipoDoc = Console.ReadLine().ToUpper();
            if (tipoDoc != "DNI" && tipoDoc != "PASAPORTE")
            {
                Console.WriteLine("Tipo de documento no válido (debe ser DNI o PASAPORTE).");
                Console.WriteLine("\nPresione una tecla para volver al menú...");
                Console.ReadKey();
                return;
            }

            Console.Write("Ingrese Nombre: ");
            string nombre = Console.ReadLine();

            Console.Write("Ingrese Apellido: ");
            string apellido = Console.ReadLine();

            Console.Write("Ingrese Fecha de Nacimiento (YYYY-MM-DD): ");
            string fechaNac = Console.ReadLine();

            Console.Write("Ingrese Email: ");
            string email = Console.ReadLine();

            Console.Write("Ingrese Número de Tarjeta (16 dígitos): ");
            string numeroTarjeta = Console.ReadLine();

            Console.WriteLine("Seleccione el Banco Emisor:");
            Console.WriteLine("1. Banco Nación");
            Console.WriteLine("2. Banco Provincia");
            Console.WriteLine("3. Banco Galicia");
            Console.WriteLine("4. Banco Santander");
            Console.WriteLine("5. Banco BBVA");
            Console.WriteLine("6. Banco Macro");
            Console.Write("Selección (1-6): ");
            string bancoOpcion = Console.ReadLine();
            string bancoEmisor = "";
            switch (bancoOpcion)
            {
                case "1": bancoEmisor = "Banco Nación"; break;
                case "2": bancoEmisor = "Banco Provincia"; break;
                case "3": bancoEmisor = "Banco Galicia"; break;
                case "4": bancoEmisor = "Banco Santander"; break;
                case "5": bancoEmisor = "Banco BBVA"; break;
                case "6": bancoEmisor = "Banco Macro"; break;
                default:
                    Console.WriteLine("Opción de banco no válida.");
                    Console.WriteLine("\nPresione una tecla para volver al menú...");
                    Console.ReadKey();
                    return;
            }

            Console.Write("Ingrese Saldo Inicial (ej: 0,00 o presione Enter para 0): ");
            string saldoInput = Console.ReadLine();
            decimal saldo = 0;
            if (!string.IsNullOrEmpty(saldoInput))
            {
                decimal.TryParse(saldoInput, out saldo);
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string queryUsuario = "INSERT INTO usuarios (documento, tipo_doc, nombre, apellido, fecha_nacimiento, email, usuario, password) VALUES (@documento, @tipoDoc, @nombre, @apellido, @fechaNac, @email, NULL, NULL)";
                            using (MySqlCommand cmdUsuario = new MySqlCommand(queryUsuario, conn, transaction))
                            {
                                cmdUsuario.Parameters.AddWithValue("@documento", documento);
                                cmdUsuario.Parameters.AddWithValue("@tipoDoc", tipoDoc);
                                cmdUsuario.Parameters.AddWithValue("@nombre", nombre);
                                cmdUsuario.Parameters.AddWithValue("@apellido", apellido);
                                cmdUsuario.Parameters.AddWithValue("@fechaNac", fechaNac);
                                cmdUsuario.Parameters.AddWithValue("@email", email);
                                cmdUsuario.ExecuteNonQuery();
                            }

                            string queryTarjeta = "INSERT INTO tarjetas (numero_tarjeta, banco_emisor, estado, saldo, dni_titular) VALUES (@numeroTarjeta, @bancoEmisor, 'Activa', @saldo, @documento)";
                            using (MySqlCommand cmdTarjeta = new MySqlCommand(queryTarjeta, conn, transaction))
                            {
                                cmdTarjeta.Parameters.AddWithValue("@numeroTarjeta", numeroTarjeta);
                                cmdTarjeta.Parameters.AddWithValue("@bancoEmisor", bancoEmisor);
                                cmdTarjeta.Parameters.AddWithValue("@saldo", saldo);
                                cmdTarjeta.Parameters.AddWithValue("@documento", documento);
                                cmdTarjeta.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\n¡Tarjeta emitida y cliente registrado con éxito!");
                            Console.ResetColor();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\nError durante la transacción: " + ex.Message);
                            Console.ResetColor();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nError de conexión a la base de datos: " + ex.Message);
                Console.ResetColor();
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEmitirLiquidacion()
        {
            Console.Clear();
            Console.WriteLine("--- EMITIR NUEVA LIQUIDACIÓN MENSUAL ---");
            
            Console.Write("Ingrese el Número de Cuenta: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            Console.Write("Ingrese Período (formato YYYY-MM, ej: 2026-06): ");
            string periodo = Console.ReadLine();

            Console.Write("Ingrese Fecha de Vencimiento (YYYY-MM-DD): ");
            string fechaVencimiento = Console.ReadLine();

            Console.Write("Ingrese Total a Pagar (ej: 12500,50): ");
            decimal totalAPagar = Convert.ToDecimal(Console.ReadLine());

            Console.Write("Ingrese Pago Mínimo (ej: 2500): ");
            decimal pagoMinimo = Convert.ToDecimal(Console.ReadLine());

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO liquidaciones (num_cuenta, periodo, fecha_vencimiento, total_a_pagar, pago_minimo) VALUES (@numCuenta, @periodo, @fechaVencimiento, @totalAPagar, @pagoMinimo)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@numCuenta", numCuenta);
                        cmd.Parameters.AddWithValue("@periodo", periodo);
                        cmd.Parameters.AddWithValue("@fechaVencimiento", fechaVencimiento);
                        cmd.Parameters.AddWithValue("@totalAPagar", totalAPagar);
                        cmd.Parameters.AddWithValue("@pagoMinimo", pagoMinimo);
                        
                        cmd.ExecuteNonQuery();
                    }
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n¡Liquidación emitida con éxito!");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nError al emitir la liquidación: " + ex.Message);
                Console.ResetColor();
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }


        // =========================================================================
        // MÉTODOS BASE QUE DEBEN COMPLETAR CON LA LÓGICA 
        // =========================================================================

        static void ObtenerYMostrarTarjetas()
        {
            // Completar 
            // Ejemplo de impresión dentro del bucle: 
            // Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", reader["num_cuenta"], reader["numero_tarjeta"], ...);
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT num_cuenta, numero_tarjeta, banco_emisor, dni_titular FROM tarjetas";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", reader["num_cuenta"], reader["numero_tarjeta"], reader["banco_emisor"], reader["dni_titular"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error al listar tarjetas: " + ex.Message);
                Console.ResetColor();
            }
        }

        static void MostrarDetalleCompleto(int cuenta)
        {
            // Completar haciendo un SELECT con JOIN de usuarios y tarjetas WHERE num_cuenta = @cuenta
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT u.documento, u.tipo_doc, u.nombre, u.apellido, u.fecha_nacimiento, u.email, u.usuario,
                               t.num_cuenta, t.numero_tarjeta, t.banco_emisor, t.saldo, t.estado
                        FROM tarjetas t
                        INNER JOIN usuarios u ON t.dni_titular = u.documento
                        WHERE t.num_cuenta = @cuenta";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@cuenta", cuenta);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine("\n--- DATOS DE LA TARJETA ---");
                                Console.WriteLine("Número de Cuenta : {0}", reader["num_cuenta"]);
                                Console.WriteLine("Número de Tarjeta: {0}", reader["numero_tarjeta"]);
                                Console.WriteLine("Banco Emisor     : {0}", reader["banco_emisor"]);
                                Console.WriteLine("Estado           : {0}", reader["estado"]);
                                Console.WriteLine("Saldo Actual     : ${0:N2}", Convert.ToDecimal(reader["saldo"]));
                                
                                Console.WriteLine("\n--- DATOS DEL CLIENTE ---");
                                Console.WriteLine("Nombre Completo  : {0} {1}", reader["nombre"], reader["apellido"]);
                                Console.WriteLine("Tipo y Nro Doc   : {0} {1}", reader["tipo_doc"], reader["documento"]);
                                Console.WriteLine("Fecha Nacimiento : {0:dd/MM/yyyy}", Convert.ToDateTime(reader["fecha_nacimiento"]));
                                Console.WriteLine("Email            : {0}", reader["email"]);
                                Console.WriteLine("Usuario Web      : {0}", reader["usuario"] != DBNull.Value ? reader["usuario"] : "(Pendiente de activación)");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("\nNo se encontró ninguna tarjeta con el número de cuenta ingresado.");
                                Console.ResetColor();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nError al consultar los datos: " + ex.Message);
                Console.ResetColor();
            }
        }

        static bool DarDeBajaTarjeta(int cuenta)
        {
            // Completar usando un DELETE FROM tarjetas WHERE num_cuenta = @cuenta
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    
                    // 1. Obtener el DNI del titular antes de borrar la tarjeta
                    string queryDni = "SELECT dni_titular FROM tarjetas WHERE num_cuenta = @cuenta";
                    string dni = null;
                    using (MySqlCommand cmdDni = new MySqlCommand(queryDni, conn))
                    {
                        cmdDni.Parameters.AddWithValue("@cuenta", cuenta);
                        dni = cmdDni.ExecuteScalar()?.ToString();
                    }

                    if (dni == null)
                    {
                        return false;
                    }

                    // 2. Ejecutar el DELETE sobre la tabla 'tarjetas' (esto borra liquidaciones en cascada)
                    string queryDeleteTarjeta = "DELETE FROM tarjetas WHERE num_cuenta = @cuenta";
                    int rowsDeleted = 0;
                    using (MySqlCommand cmdDelete = new MySqlCommand(queryDeleteTarjeta, conn))
                    {
                        cmdDelete.Parameters.AddWithValue("@cuenta", cuenta);
                        rowsDeleted = cmdDelete.ExecuteNonQuery();
                    }

                    // 3. Opcional: Eliminar al usuario de la tabla 'usuarios'
                    if (rowsDeleted > 0 && !string.IsNullOrEmpty(dni))
                    {
                        string queryDeleteUsuario = "DELETE FROM usuarios WHERE documento = @dni";
                        using (MySqlCommand cmdDeleteUser = new MySqlCommand(queryDeleteUsuario, conn))
                        {
                            cmdDeleteUser.Parameters.AddWithValue("@dni", dni);
                            cmdDeleteUser.ExecuteNonQuery();
                        }
                    }

                    return rowsDeleted > 0;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nError al dar de baja la tarjeta: " + ex.Message);
                Console.ResetColor();
                return false;
            }
        }
    }
}