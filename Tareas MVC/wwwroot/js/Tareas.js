
//Funcion para agregar una nueva tarea al listado de tareas
function agregarNuevaTareaAlListado() {
    ListadoTareasViewModel.tarea.push(new tareasElemtosListadoViewModel({ id: 0, titulo: '' }));

    $("[name=titulo-tarea]").last().focus();
}


//Funcion para enfocar el input con el cursor al agregar una nueva tarea
async function manejarFocusoutTituloTarea(tarea) {
    const titulo = tarea.titulo();
    if (!titulo) {
        ListadoTareasViewModel.tarea.pop();

        return;
    }

    const data = JSON.stringify(titulo);

    const respuesta = await fetch(UrlTareas, {

        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    //Por si hay un error en el WebAPI
    if (respuesta.ok) {
        const json = await respuesta.json();
        tarea.id(json.id);
    }
    else {
        manejarErrorApi();
    }
}

//Funcion para obtener las tareas por BackEnd
async function ObtenerTareas() {

    ListadoTareasViewModel.cargando(true);

    const respuesta = await fetch(UrlTareas, {

        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!respuesta.ok) {
        manejarErrorApi(respuesta);
        return;
    }

    const json = await respuesta.json();
    ListadoTareasViewModel.tarea([]);

    json.forEach(valor => {
        ListadoTareasViewModel.tarea.push(new tareasElemtosListadoViewModel(valor));
    });

    ListadoTareasViewModel.cargando(false);
}


//Actualizando el orden de las tareas por Front-End
async function actualizarOrdenTareas() {
    const ids = obtenerIdTareas();
    await enviarIdsTareasAlBackend(ids);

    const arregloOrdenado = ListadoTareasViewModel.tarea.sorted(function (a, b) {
        return ids.indexOf(a.id().toString()) - ids.indexOf(b.id().toString());
    });

    ListadoTareasViewModel.tarea([]);
    ListadoTareasViewModel.tarea(arregloOrdenado);

    console.log("actualizar orden", arregloOrdenado, ids)
}

//Funcion para obtener el id de las tarea
function obtenerIdTareas() {
    const ids = $("[name=titulo-tarea]").map(function () {
        return $(this).attr("data-id");
    }).get();

    console.log("Obtener ids de las tareas ", ids)
    return ids;
}


//Funcion para enviar los ids de las tareas al Back-End
async function enviarIdsTareasAlBackend(ids) {
    var data = JSON.stringify(ids);

    await fetch(`${UrlTareas}/ordenar`, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
}

$(function () {
    $("#reordenable").sortable({
        axis: 'y',
        stop: async function () {
            await actualizarOrdenTareas();
        }
    });
})