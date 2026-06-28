<?php

$host = "db";
$db = "mi_banco_db";
$user = "root";
$pass = "";
$charset = "utf8mb4";

$conn = new mysqli($host, $user, $pass, $db);

if ($conn->connect_error) {
    die("Error de conexión al contenedor de la base de datos: " . $conn->connect_error);
}

$conn->set_charset($charset);
?>