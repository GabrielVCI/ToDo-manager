
let inputArchivoTarea = document.getElementById('archivoATarea');

function manejarClickAgregarArchivoAdjunto() {
    inputArchivoTarea.click();
}

async function manejarSeleccionArchivoTarea(event) {

    const archivos = event.target.files;
    const archivosArreglo = Array.from(archivos);
    const idTarea = tareaEditarViewModel.id;

    const formData = new FormData();

    for (var i = 0; i < archivosArreglo.length; i++) {
        formData.append("archivos", archivosArreglo[i]);
    }

    const respuesta = await fetch(`${UrlArchivos}/${idTarea}`, {
        body: formData,
        method: 'POST'
    });

    if (!respuesta.ok) {
        manejarErrorApi(respuesta);
        return;
    }

    const json = await respuesta.json();
    prepararArchivosAdjuntos(json);

    inputArchivoTarea.value = null;
}

function prepararArchivosAdjuntos(archivosAdjuntos) {

    archivosAdjuntos.forEach(archivoAdjunto => {
        let fechaCreacion = archivoAdjunto.fechaCreacion;
        if (archivoAdjunto.fechaCreacion.indexOf('Z' === -1)) {
            fechaCreacion += 'Z';
        }

        const fechaCreacionDT = new Date(fechaCreacion);
        archivoAdjunto.publicado = fechaCreacionDT.toLocaleString();

        tareaEditarViewModel.archivosAdjuntos.push(new archivoAdjuntoVM({ ...archivoAdjunto, modoEdicion: false }));
    })
}

let tituloArchivoAdjuntoAnterior;
function manejarClickTituloArchivoAdjunto(archivoAdjunto) {

    archivoAdjunto.modoEdicion(true);
    tituloArchivoAdjuntoAnterior = archivoAdjunto.titulo();
    $("[name='txtArchivoAdjuntoTitulo']:visible").focus();

}

async function manejarFocusoutTituloArchivoAdjunto(archivoAdjunto) {
    archivoAdjunto.modoEdicion(false);

    const idTarea = archivoAdjunto.id;

    if (!archivoAdjunto.titulo()) {
        archivoAdjunto.titulo(tituloArchivoAdjuntoAnterior);
    }

    if (archivoAdjunto.titulo() === tituloArchivoAdjuntoAnterior) {
        return;
    }

    const data = JSON.stringify(archivoAdjunto.titulo());

    const respuesta = await fetch(`${UrlArchivos}/${idTarea}`, {
        body: data,
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
            }
    });

    if (!respuesta.ok) { 
        manejarErrorApi(respuesta);
    }

}

function manejarClickBorrarArchivoAdjunto(archivoAdjunto) {

    modalEditarTareaBootstrap.hide();

    confirmarAccion({

        callbackAceptar: () => {
            borrarArchivoAdjunto(archivoAdjunto);
            modalEditarTareaBootstrap.show();
        },

        callbackCancelar: () => {
            modalEditarTareaBootstrap.show();
        },
        titulo: '¿Desea eliminar este archivo?'
    })
}

async function borrarArchivoAdjunto(archivoAdjunto) {

    const respuesta = await fetch(`${UrlArchivos}/${archivoAdjunto.id}`, {
        method: 'DELETE'
    });

    if (!respuesta.ok) {
        manejarErrorApi(respuesta);
        return;
    }

    tareaEditarViewModel.archivosAdjuntos.remove(function (item) {
        return item.id == archivoAdjunto.id
    });
}


function manejarClickDescargarArchivoAdjunto(archivoAdjunto) {

    descargarArchivo(archivoAdjunto.url, archivoAdjunto.titulo());
}