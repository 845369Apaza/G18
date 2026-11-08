# E-Commerce con microservicios (.NET 10) - Grupo 18

Cinco REST APIs independientes. Cada una tiene su propia base SQLite.

## Cómo ejecutar

Requisito: SDK de .NET 10.

1. Clonar el repositorio.
2. Hacer doble clic en `iniciar-todo.bat`.
3. Se abre Swagger de cada servicio.

También se puede abrir `ECommerce.sln`, elegir "Configurar proyectos de inicio",
marcar "Varios proyectos de inicio" y poner los 5 en "Iniciar".

## Servicios y puertos

| Servicio          | Puerto | Swagger                       | Descripción                  |
|-------------------|--------|-------------------------------|------------------------------|
| Products.API      | 5001   | http://localhost:5001/swagger | Catálogo y stock             |
| Users.API         | 5002   | http://localhost:5002/swagger | Registro, login y bloqueo    |
| Cart.API          | 5003   | http://localhost:5003/swagger | Carrito por usuario          |
| Orders.API        | 5004   | http://localhost:5004/swagger | Órdenes y estados            |
| Notifications.API | 5005   | http://localhost:5005/swagger | Notificaciones simuladas     |

Cada servicio expone `/health`, `/health/ready` y `/health/live`.

## Arquitectura

    Swagger ---> Products <--- Cart
       |            ^  |
       |            |  v
       |          Orders ---> Users <--- Notifications

    Comunicación por HTTP con IHttpClientFactory.
    El header X-Correlation-Id viaja en cada llamada entre servicios.

## Códigos de error

| Código  | HTTP | Mensaje                                                 |
|---------|------|---------------------------------------------------------|
| PRD-001 | 404  | Producto no encontrado.                                 |
| PRD-002 | 400  | Los datos del producto son inválidos.                   |
| PRD-003 | 409  | Ya existe un producto con ese nombre en la categoría.   |
| PRD-004 | 409  | El producto tiene órdenes activas y no puede eliminarse.|
| PRD-005 | 500  | Error interno al procesar el producto.                  |
| USR-001 | 409  | El email ya está registrado.                            |
| USR-002 | 400  | Los datos del usuario son inválidos.                    |
| USR-003 | 401  | Credenciales incorrectas.                               |
| USR-004 | 403  | Usuario bloqueado por demasiados intentos fallidos.     |
| USR-005 | 403  | Usuario bloqueado por detección de fraude.              |
| USR-006 | 500  | Error interno al procesar el usuario.                   |
| ORD-001 | 404  | Orden no encontrada.                                    |
| ORD-002 | 400  | Los datos de la orden son inválidos.                    |
| ORD-003 | 404  | Usuario no encontrado al crear la orden.                |
| ORD-004 | 404  | Producto no encontrado al crear la orden.               |
| ORD-005 | 422  | Stock insuficiente para uno o más productos.            |
| ORD-006 | 409  | El estado de la orden no puede ser modificado.          |
| ORD-007 | 500  | Error interno al procesar la orden.                     |
| CRT-001 | 404  | Carrito no encontrado.                                  |
| CRT-002 | 404  | Producto no encontrado.                                 |
| CRT-003 | 422  | Stock insuficiente para agregar al carrito.             |
| CRT-004 | 400  | Cantidad inválida.                                      |
| CRT-005 | 500  | Error interno al procesar el carrito.                   |
| NTF-001 | 404  | Usuario no encontrado.                                  |
| NTF-002 | 400  | Los datos de la notificación son inválidos.             |
| NTF-003 | 404  | No se encontraron notificaciones para el usuario.       |
| NTF-004 | 500  | Error interno al procesar la notificación.              |

## Integrantes

| Integrante | Usuario de GitHub | Tareas |
|---|---|---|
| Nicole Ortiz | 901051Ortiz | C4, C6, C11, C13, C17, C18, C21 |
| Gabriel Apaza | 845369Apaza | C1, C3, C7, C9, C14, C15, C19 |
| Paloma Steinberg | palomasteinberg | C2, C5, C8, C10, C12, C16, C20 |