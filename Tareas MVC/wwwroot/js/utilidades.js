
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

function confirmarAccion({ callbackAceptar, callBackCancelar, titulo }) {

    Swal.fire({

        title: titulo || 'Esta accion no se puede retroceder',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si',
        focusConfirm: true
    }).then((resultado => {
        if (resultado.isConfirmed) {
            callbackAceptar();
        } else if (callBackCancelar) {
            callBackCancelar();
        }
    }))
}
