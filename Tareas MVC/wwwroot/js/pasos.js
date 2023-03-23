function ManejarClickAgregarPaso() {

    const indice = tareaEditarViewModel.pasos().findIndex(p => p.esNuevo());

    if (indice !== -1) {
        return;
    }

    tareaEditarViewModel.pasos.push(new pasoViewModel({ ModoEdicion: true, realizado: false }));

    $("[name=txtPasoDescripcion]:visible").focus();

}


function manejarClickCancelarPasos(paso) {

    if (paso.esNuevo()) {
        tareaEditarViewModel.pasos.pop();
    } else {
        paso.ModoEdicion(false);
        paso.descripcion(paso.descripcionAnterior);
    }
}

async function manejarClickSalvarPasos(paso) {

    paso.ModoEdicion(false);

    const esNuevo = paso.esNuevo();

    const idTarea = tareaEditarViewModel.id;

    const data = obtenerCuerpoPeticionPaso(paso);

    const descripcion = paso.descripcion();

    if (!descripcion) {
        paso.descripcion(paso.descripcionAnterior);

        if (paso.esNuevo) {
            tareaEditarViewModel.pasos.pop();
        }

        return;
    }

    if (esNuevo) {

        await insertarPaso(paso, data, idTarea);

    } else {
        actualizarPaso(data, paso.id());
    }
}

async function insertarPaso(paso, data, idTarea) {

    const respuesta = await fetch(`${UrlPasos}/${idTarea}`, {

        body: data,
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (respuesta.ok) {
        const json = await respuesta.json();
        paso.id(json.id)

        const tarea = obtenerTareaEnEdicion();
        tarea.pasosTotal(tarea.pasosTotal() + 1);
        

        if (paso.pasosRealizados()) {
            tarea.pasosRealizados(tarea.pasosRealizados() + 1);
        }

    } else {
        manejarErrorApi(respuesta);
    }
}


function obtenerCuerpoPeticionPaso(paso) {

    return JSON.stringify
        ({
            descripcion: paso.descripcion(),
            realizado: paso.realizado()
        });
}

function manejarClickDescripcionPaso(paso) {
    paso.ModoEdicion(true);
    paso.descripcionAnterior = paso.descripcion();
    $("[name=txtPasoDescripcion]:visible").focus();
}

async function actualizarPaso(data, id)
{
    const respuesta = await fetch(`${UrlPasos}/${id}`, {

        body: data,
        method: "PUT",
        headers: {
            'Content-Type': 'application/json'
        }

    });

    if (!respuesta.ok) {
       
        return manejarErrorApi(respuesta);
    }
}


function manejarClickCheckboxPaso(paso) {

    if (paso.esNuevo()) {
        return true;
    }

    const data = obtenerCuerpoPeticionPaso(paso);

    actualizarPaso(data, paso.id());

    const tarea = obtenerTareaEnEdicion();

    let pasosRealizadosActual = tarea.pasosRealizados();

    if (paso.realizado()) {
        pasosRealizadosActual++;
    } else {
        pasosRealizadosActual--;
    }

    tarea.pasosRealizados(pasosRealizadosActual);

    return true;
}

function manejarClickBorrarPasos(paso) {

    modalEditarTareaBootstrap.hide();

    confirmarAccion({
        callbackAceptar: () => {
            borrarPaso(paso);
            modalEditarTareaBootstrap.show();
        },

        callbackCancelar: () => {
            modalEditarTareaBootstrap.show();
        },
        titulo: `¿Desea borrar este paso?`
    });
}

async function borrarPaso(paso) {

    const respuesta = await fetch(`${UrlPasos}/${paso.id()}`, {

        method: "DELETE"
    });

    if (!respuesta) {
        manejarErrorApi(respuesta);
        return;
    }

    tareaEditarViewModel.pasos.remove(function (item) { return item.id() == paso.id() });

    const tarea = obtenerTareaEnEdicion();
    tarea.pasosTotal(tarea.pasosTotal() - 1);

    if (paso.realizado()) {
        tarea.pasosRealizados(tarea.pasosRealizados() - 1);
    }
}

async function actualizarOrdenPaso() {

    const ids = obtenerIdsPasos();
    await enviarIdsPasosAlBackend(ids);

    const arregloOrganizado = tareaEditarViewModel.pasos.sorted(function (a, b) {
        return ids.indexOf(a.id().toString()) - ids.indexOf(b.id().toString());
    });

    return tareaEditarViewModel.pasos(arregloOrganizado);
}

function obtenerIdsPasos() {
    const ids = $("[name=chbPaso]").map(function () {
        return $(this).attr('data-id');
    }).get();

    return ids;
}

async function enviarIdsPasosAlBackend(ids) {
    var data = JSON.stringify(ids);

    const respuesta = await fetch(`${UrlPasos}/ordenar/${tareaEditarViewModel.id}`, {

        method: "POST",
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
}

$(function () {

    $("#reordenable-pasos").sortable({
        axis: 'y',
        stop: async function () {
            await actualizarOrdenPaso();
        }
    });
    
})