<?php

session_start();
require_once 'conexion.php';

if (isset($_POST['ingresar'])) {
    $tipo_doc = $_POST['tipo_doc'];
    $documento = $_POST['documento'];
    $usuario = $_POST['usuario'];
    $password = $_POST['password'];

    $stmt = $conn->prepare("SELECT documento, nombre, apellido FROM usuarios WHERE tipo_doc = ? AND documento = ? AND usuario = ? AND password = ?");
    $stmt->bind_param("ssss", $tipo_doc, $documento, $usuario, $password);
    $stmt->execute();
    $result = $stmt->get_result();

    if ($result->num_rows === 1) {
        $user_data = $result->fetch_assoc();

        $_SESSION['usuario_logueado'] = true;
        $_SESSION['dni'] = $user_data['documento'];
        $_SESSION['nombre_completo'] = $user_data['nombre'] . " " . $user_data['apellido'];

        header("Location: resumen.php");
        exit();
    } else {
        die("Credenciales incorrectas o usuario no activado.");
    }
}
$conn->close();
?>