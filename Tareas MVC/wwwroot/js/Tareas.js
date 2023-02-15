
function agregarNuevaTareaAlListado() {
    ListadoTareasViewModel.tarea.push(new tareasElemtosListadoViewModel({ id: 0, titulo: '' }));

    $("[name =titulo-tarea]").last().focus();
}

function manejarFocusoutTituloTarea(tarea) {
    const titulo = tarea.titulo();
    if (!titulo) {
        ListadoTareasViewModel.tarea.pop();

        return;
    }

    tarea.id(1);
}