# Servidor Central

En este repositorio encontrarán el servidor central desde dónde se gestionan todas las solicitudes desde la App móvil como también el intercambio de datos con los dispositivos de cada cliente y su posterior almacenamiento en bases de datos.
Para el almacenamiento de notificaciones push se utiliza MongoDB y para los datos de los clientes, dispositivos etc.. se utiliza SQL Server que se conecta a través de un ORM (Dapper)
