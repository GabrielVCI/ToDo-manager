
async function manejarErrorApi(respuesta) {
    let mensajeError = '';

    if (status == 400) { //Error 400 puede ser cualquier error
        mensajeError = await respuesta.text();

    } else if (respuesta.status == 404) {

        mensajeError = 'El recurso solicitado no fue encontrado';
    } else {
        mensajeError = 'Ha ocurrido un error con su solicitud';
    }

    mostrarMensajeError();
}

function mostrarMensajeError(mensaje) {

    Swal.fire({
        icon: 'error',
        title: 'Error...',
        text: mensaje
    });
}