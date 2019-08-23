// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API

window.JsFunctions =
    {
        ToDataTable: function (Id) {
            $(document).ready(function () {
                $('#' + Id).DataTable({
                    retrieve: true, //https://datatables.net/manual/tech-notes/3
                    stateSave: true,
                    colReorder: true,
                    //autoFill: true,
                    dom: 'B<"clear">lfrtip',
                    buttons: true,
                    //fixedColumns: true,
                    //fixedHeader: true,
                    //keys: true,
                    //responsive: true,
                    //rowGroup: { dataSrc: 'group' },
                    rowReorder: true,
                    //ajax: '/api/data',
                    scrollY: 600,
                    //scrollCollapse: true,
                    deferRender: true,
                    scroller: true,
                    select: true
                });
            });
            return true;
        }
    };
