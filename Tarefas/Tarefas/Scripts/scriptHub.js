/// <reference path="_references.js" />

function connectHub(usuario, entrou) {
    var hub = $.connection.notificationHub;

    hub.client.entrou = function (nome) {
        showNotify(`${nome} acabou de entrar!`);
    }

    hub.client.indicadoPraUmaTarefa = function (responsavel, tarefa, criador) {
        if (responsavel === usuario) {
            var titulo = `olá ${responsavel}!`;
            var texto = `você foi indicado por <strong>${criador}</strong> para executar a tarefa <strong>"${tarefa}"</strong>.`;

            showNotify(texto, titulo);
        }        
    };

    hub.client.novaNotaCadastrada = function (responsavel, texto, data) {
        if (responsavel != usuario) {
            var titulo = `olá ${usuario}!`;
            if (!responsavel) {
                responsavel = "anônimo";
            }
            var texto = `Há uma nova nota cadastrada por ${responsavel}`;

            showNotify(texto, titulo);
        }        
    }

    $.connection.hub.start().done(function () {
        if (entrou) {
            hub.server.entrou(usuario);
        }
    });
};

function showNotify(texto, titulo) {
    if (titulo) {
        $.notify({
            title: `<div style="font-size: 25px; font-weigth: bold">${titulo}</div>`,
            message: texto
        });
    } else {
        $.notify({
            message: texto
        });
    }
}