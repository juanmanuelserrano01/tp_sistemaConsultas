<?php

require_once 'conexion.php';

if (isset($_POST['registrar'])) {
    $tipo_doc = $_POST['tipo_doc'];
    $documento = $_POST['documento'];
    $nombre = $_POST['nombre'];
    $apellido = $_POST['apellido'];
    $email = $_POST['email'];
    $usuario = $_POST['usuario'];
    $passwordA = $_POST['passwordA'];
    $passwordB = $_POST['passwordB'];

    if ($passwordA !== $passwordB) {
        die('Error, las contraseñas ingresadas no coinciden');
    }

    if ($tipo_doc !== 'DNI' && $tipo_doc !== 'PASAPORTE') {
        die('Error, el tipo de documento ingresado no es correcto');
    }

    $stmt = $conn->prepare("SELECT usuario FROM usuarios WHERE documento = ? AND tipo_doc = ?");
    $stmt->bind_param("ss", $documento, $tipo_doc);
    $stmt->execute();
    $result = $stmt->get_result();

    if ($result->num_rows === 0) {
        die("Error: No se encontró ningún cliente registrado con ese documento en el sistema. Contacte a la administración.");
    }

    $row = $result->fetch_assoc();

    if (!is_null($row['usuario'])) {
        die("Error: Esta cuenta web ya se encuentra activa.");
    }

    $update_stmt = $conn->prepare("UPDATE usuarios SET nombre = ?, apellido = ?, email = ?, usuario = ?, password = ? WHERE documento = ?");
    $update_stmt->bind_param("ssssss", $nombre, $apellido, $email, $usuario, $passwordA, $documento);

    if ($update_stmt->execute()) {
        echo "¡Cuenta activada con éxito! Ya podés ingresar al portal. <a href='ingreso.html'>Ir al Login</a>";
    } else {
        echo "Error al activar la cuenta: " . $conn->error;
    }

    $stmt->close();
    $update_stmt->close();
}
$conn->close();
?>