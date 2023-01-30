# Novit Software - Academia

## Requerimientos funcionales

Un cliente del mercado inmobiliario necesita una aplicación para gestionar las ventas realizadas sobre sus productos. Para lo cual nos pide que la aplicación cumpla con los siguientes requerimientos:

- La aplicación debe contar un inicio de sesión para todos los usuarios, existen dos tipos de usuarios, con rol vendedor y con rol comercial.
- En el inicio de sesión el usuario debe ingresar el nombre de usuario y contraseña.
- Los usuarios con rol vendedor deben poder realizar operaciones CRUD sobre productos y las reservas sobre dichos productos.
- De cada producto se desea informar un código (alfanumérico), un barrio, el precio y un enlace a una imagen ilustrativa.
- Los posibles estados de un producto son disponible, reservado y vendido.
- Cuando se realiza una reserva esta tiene un estado de ingresada, mientras que el estado del producto será reservado
- Si una reserva se encuentra en estado de ingresada tiene 2 opciones:
  - Que la reserva pase aprobada, lo cual indica que el producto pasa de de estado reservado a vendido
  - Que la reserva pase cancelada, lo cual indica que el producto pasa de estado reservado a disponible
- Cada reserva debe tener asociado el nombre del cliente y el producto. Al momento de crear una reserva se debe tener la opción de solicitar la aprobación (por parte de comercial) o cancelar la reserva.
- Los posibles estados de una reserva son ingresada, cancelada, aprobada y rechazada.
- Los usuarios con rol comercial deben poder aprobar y/o rechazar las reservas.
- Los usuarios con rol comercial deben poder visualizar un reporte (mediante un gráfico a definir) en donde se debe indicar las ventas por cada vendedor.
- En los siguientes casos las reservas no requieren aprobación alguna (se auto aprueban)
  - Pertenece al Barrio X y su precio es menor a 100.000
  - Es el último producto disponible del Barrio X
- En el caso de que el vendedor tiene hasta 3 reservas ingresadas no se debe permitir realizar una nueva reserva


## Restricciones tecnológicas

- La aplicación debe tener un frontend realizado en Angular y un backend en .NET. 
- En el frontend se debe aplicar componentes Angular Material UI, etc. 
- En el backend se compone de dos partes una base de datos en SQL Server y una WebAPI realizada en .NET la cual usará un ORM como Entity Framework Core para la comunicación con la base de datos.
- El código fuente del frontend y backend debe tener un repositorio de github propio. El repositorio debe tener visibilidad privada y se debe agregar como colaborador a los profesores y tutores.
- Para el desarrollo de código fuente se recomienda usar Visual Studio y/o Visual Studio Code

## Diagrama de Producto

![Screenshot 2023-01-19 113522](https://user-images.githubusercontent.com/7192115/213470036-13b53cfa-e24c-4e94-a3f2-084723415bec.png)

## Entregas

### Hito 1

**Angular - Fecha entrega: 20-01-2023**

Realizar las pantallas para permitir operaciones CRUD de los productos y las operaciones de venta. Los resultados de las operaciones se deben reflejar en la consola del navegador con datos simulados.

**.NET - Fecha entrega: 27-01-2023**

Realizar los controladores con los endpoints necesarios para permitir operaciones CRUD de los productos y las operaciones de venta. Los resultados de las operaciones se deben probar desde el swagger. (opcional: verificar que los datos se persisten correctamente en la base de datos)

### Hito 2 

**Angular - Fecha entrega: 03-02-2023**

Conectar el frontend con el backend de forma local. El objetivo es consumir los endpoints desde los servicios de cada pantalla. Para lo cual se debe tener en ejecución ambas aplicaciones en paralelo y se debe configurar la URL de conexión hacia la WebAPI.

**.NET - Fecha entrega: 03-02-2023**

Refactorizar la funcionalidad realizada en los controladores/endpoints en repositorios y servicios correspondientes. Para ello se debe estructurar correctamente el proyecto e inyectar las dependencias necesarias. (Nota: eso no debe afectar de ninguna forma la comunicación con el frontend)

### Hito 3

**Angular - Fecha entrega: 10-02-2023**

Mejorar el aspecto visual de las pantallas mediante la aplicación de componentes de Angular Material UI.

**.NET - Fecha entrega: 10-02-2023**

Agregar y configurar un middleware que permita loguear los accesos hacia la aplicación de cualquier usuario. Los logueos se deben visualizar por consola.

### Final

**Fecha entrega: 17-02-2023**

La aplicación debe funcionar correctamente, esto implica que la conexión entre el frontend y backend debe ser transparente para el usuario. El backend debe tener configurado correctamente la base de datos.

