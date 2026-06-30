<?php

session_start();
require_once 'conexion.php';

if (!isset($_SESSION['usuario_logueado']) || $_SESSION['usuario_logueado'] !== true) {
    header('Location: ingreso.html');
    exit();
}

$dni = $_SESSION['dni'];

$query_tarjeta = "
    SELECT t.num_cuenta, t.numero_tarjeta, t.banco_emisor, t.saldo, t.estado,
           l.periodo, l.fecha_vencimiento, l.total_a_pagar, l.pago_minimo
    FROM tarjetas t
    LEFT JOIN liquidaciones l ON t.num_cuenta = l.num_cuenta
    WHERE t.dni_titular = ?
    ORDER BY l.periodo DESC
    LIMIT 1";

$stmt = $conn->prepare($query_tarjeta);
$stmt->bind_param("s", $dni);
$stmt->execute();
$res_tarjeta = $stmt->get_result()->fetch_assoc();

if (!$res_tarjeta) {
    die("Error: No se encontró ninguna tarjeta asociada a su cuenta. Contacte a soporte.");
}

$num_cuenta = $res_tarjeta['num_cuenta'];

$query_historial = "
    SELECT periodo, fecha_vencimiento, total_a_pagar, pago_minimo 
    FROM liquidaciones 
    WHERE num_cuenta = ? AND periodo < ? 
    ORDER BY periodo DESC";

$stmt_hist = $conn->prepare($query_historial);
$ultimo_periodo = $res_tarjeta['periodo'] ?? '';
$stmt_hist->bind_param("is", $num_cuenta, $ultimo_periodo);
$stmt_hist->execute();
$res_historial = $stmt_hist->get_result();
?>

<!DOCTYPE html>
<html lang="es">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Mis Tarjetas - Mi Resumen</title>
    <script src="https://cdn.tailwindcss.com"></script>
</head>

<body class="bg-gray-100 font-sans min-h-screen flex flex-col justify-between">

    <header class="bg-[#004691] text-white flex justify-between items-center px-6 py-4 shadow-md">
        <h1 class="text-xl font-semibold">Mis <span class="font-bold">Tarjetas</span></h1>
        <div class="flex items-center space-x-4">
            <span
                class="text-sm bg-blue-800 px-3 py-1 rounded-full"><?php echo htmlspecialchars($_SESSION['nombre_completo']); ?></span>
            <a href="logout.php" class="text-xs text-gray-200 hover:text-white underline">Cerrar Sesión</a>
        </div>
    </header>

    <main class="flex-grow max-w-4xl w-full mx-auto p-6 space-y-6">

        <div
            class="bg-white p-6 rounded-lg shadow-md border-l-4 border-[#004691] flex flex-col sm:flex-row justify-between items-start sm:items-center">
            <div>
                <p class="text-xs text-gray-400 font-semibold uppercase">Tarjeta de Crédito</p>
                <h2 class="text-xl font-bold text-gray-800">
                    <?php echo htmlspecialchars($res_tarjeta['banco_emisor']); ?> Progra3card
                </h2>
                <p class="text-sm text-gray-600 mt-1">Nro. Tarjeta: **** **** ****
                    <?php echo substr($res_tarjeta['numero_tarjeta'], -4); ?>
                </p>
                <p class="text-xs text-gray-500">Nro. Cuenta: <?php echo $res_tarjeta['num_cuenta']; ?></p>
            </div>
            <div class="mt-4 sm:mt-0 text-right">
                <span
                    class="px-2 py-1 text-xs font-bold rounded <?php echo $res_tarjeta['estado'] == 'Activa' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'; ?>">
                    <?php echo $res_tarjeta['estado']; ?>
                </span>
                <p class="text-2xl font-bold text-gray-800 mt-2">
                    $<?php echo number_format($res_tarjeta['saldo'], 2, ',', '.'); ?></p>
                <p class="text-xs text-gray-400 font-semibold uppercase">Saldo Actual</p>
            </div>
        </div>

        <div class="bg-white rounded-lg shadow-md overflow-hidden">
            <div class="bg-gradient-to-r from-gray-800 to-gray-700 text-white p-4">
                <h3 class="font-semibold text-md">📌 Resumen del Período Actual</h3>
            </div>
            <div class="p-6">
                <?php if ($res_tarjeta['periodo']): ?>
                    <div class="grid grid-cols-2 md:grid-cols-4 gap-4 text-center">
                        <div class="border-r border-gray-100 last:border-0">
                            <p class="text-xs font-semibold text-gray-400 uppercase">Período</p>
                            <p class="text-lg font-bold text-gray-700 mt-1">
                                <?php echo htmlspecialchars($res_tarjeta['periodo']); ?>
                            </p>
                        </div>
                        <div class="border-r border-gray-100 last:border-0">
                            <p class="text-xs font-semibold text-gray-400 uppercase">Vencimiento</p>
                            <p class="text-lg font-bold text-red-600 mt-1">
                                <?php echo date("d/m/Y", strtotime($res_tarjeta['fecha_vencimiento'])); ?>
                            </p>
                        </div>
                        <div class="border-r border-gray-100 last:border-0">
                            <p class="text-xs font-semibold text-gray-400 uppercase">Total a Pagar</p>
                            <p class="text-xl font-extrabold text-gray-800 mt-1">
                                $<?php echo number_format($res_tarjeta['total_a_pagar'], 2, ',', '.'); ?></p>
                        </div>
                        <div>
                            <p class="text-xs font-semibold text-gray-400 uppercase">Pago Mínimo</p>
                            <p class="text-lg font-bold text-gray-700 mt-1">
                                $<?php echo number_format($res_tarjeta['pago_minimo'], 2, ',', '.'); ?></p>
                        </div>
                    </div>
                <?php else: ?>
                    <p class="text-center text-sm text-gray-500 py-4">No se han emitido liquidaciones para esta tarjeta
                        todavía.</p>
                <?php endif; ?>
            </div>
        </div>

        <div class="bg-white rounded-lg shadow-md overflow-hidden">
            <div class="bg-gray-50 border-b border-gray-200 p-4">
                <h3 class="font-semibold text-gray-700 text-md">🕒 Historial de Resúmenes</h3>
            </div>
            <div class="overflow-x-auto">
                <table class="w-full text-left border-collapse">
                    <thead>
                        <tr
                            class="bg-gray-100 text-gray-400 uppercase text-[10px] font-semibold border-b border-gray-200">
                            <th class="p-4">Período</th>
                            <th class="p-4">Vencimiento</th>
                            <th class="p-4">Total a Pagar</th>
                            <th class="p-4">Pago Mínimo</th>
                        </tr>
                    </thead>
                    <tbody class="divide-y divide-gray-100 text-sm text-gray-600">
                        <?php if ($res_historial->num_rows > 0): ?>
                            <?php while ($hist = $res_historial->fetch_assoc()): ?>
                                <tr class="hover:bg-gray-50 transition">
                                    <td class="p-4 font-semibold text-gray-700">
                                        <?php echo htmlspecialchars($hist['periodo']); ?>
                                    </td>
                                    <td class="p-4"><?php echo date("d/m/Y", strtotime($hist['fecha_vencimiento'])); ?></td>
                                    <td class="p-4 font-bold text-gray-800">
                                        $<?php echo number_format($hist['total_a_pagar'], 2, ',', '.'); ?></td>
                                    <td class="p-4">$<?php echo number_format($hist['pago_minimo'], 2, ',', '.'); ?></td>
                                </tr>
                            <?php endwhile; ?>
                        <?php else: ?>
                            <tr>
                                <td colspan="4" class="p-6 text-center text-gray-400 text-xs">No hay resúmenes anteriores
                                    registrados en el historial.</td>
                            </tr>
                        <?php endif; ?>
                    </tbody>
                </table>
            </div>
        </div>

    </main>

    <footer class="bg-gray-50 text-[10px] text-gray-500 text-center p-4 border-t border-gray-200 mt-12">
        Portal Oficial de Consultas de Liquidaciones Progra3card.
    </footer>

</body>

</html>
<?php
$stmt->close();
$stmt_hist->close();
$conn->close();
?>