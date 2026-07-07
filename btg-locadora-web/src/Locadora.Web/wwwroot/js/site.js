// Componente de autocomplete (busca por texto, min. 3 letras) usado no formulário de Jogos
// para selecionar Gêneros/Desenvolvedores/Publicadoras.
(function () {
    function inicializarAutocomplete(container) {
        var input = container.querySelector('[data-autocomplete-input]');
        var lista = container.querySelector('[data-autocomplete-lista]');
        var tags = container.querySelector('[data-autocomplete-tags]');
        var hiddenContainer = container.querySelector('[data-autocomplete-hidden]');
        var url = container.getAttribute('data-autocomplete-url');
        var fieldName = container.getAttribute('data-autocomplete-field');
        var selecionados = [];
        var timer = null;

        function renderTags() {
            tags.innerHTML = '';
            hiddenContainer.innerHTML = '';

            selecionados.forEach(function (item, index) {
                var badge = document.createElement('span');
                badge.className = 'badge text-bg-primary me-1 mb-1 p-2';
                badge.textContent = item.nome + ' ';

                var remover = document.createElement('button');
                remover.type = 'button';
                remover.className = 'btn-close btn-close-white btn-sm ms-1';
                remover.style.fontSize = '0.55rem';
                remover.setAttribute('aria-label', 'Remover');
                remover.addEventListener('click', function () {
                    selecionados.splice(index, 1);
                    renderTags();
                });
                badge.appendChild(remover);
                tags.appendChild(badge);

                var hiddenId = document.createElement('input');
                hiddenId.type = 'hidden';
                hiddenId.name = fieldName + '[' + index + '].Id';
                hiddenId.value = item.id;
                hiddenContainer.appendChild(hiddenId);

                var hiddenNome = document.createElement('input');
                hiddenNome.type = 'hidden';
                hiddenNome.name = fieldName + '[' + index + '].Nome';
                hiddenNome.value = item.nome;
                hiddenContainer.appendChild(hiddenNome);
            });
        }

        function limparLista() {
            lista.innerHTML = '';
            lista.classList.add('d-none');
        }

        function adicionar(item) {
            if (!selecionados.some(function (s) { return s.id === item.id; })) {
                selecionados.push(item);
                renderTags();
            }
            input.value = '';
            limparLista();
            input.focus();
        }

        input.addEventListener('input', function () {
            var texto = input.value.trim();
            clearTimeout(timer);

            if (texto.length < 3) {
                limparLista();
                return;
            }

            timer = setTimeout(function () {
                fetch(url + '?texto=' + encodeURIComponent(texto), { headers: { Accept: 'application/json' } })
                    .then(function (resp) { return resp.ok ? resp.json() : []; })
                    .then(function (itens) {
                        var disponiveis = (itens || []).filter(function (item) {
                            return !selecionados.some(function (s) { return s.id === item.id; });
                        });

                        lista.innerHTML = '';

                        if (disponiveis.length === 0) {
                            limparLista();
                            return;
                        }

                        disponiveis.forEach(function (item) {
                            var opcao = document.createElement('button');
                            opcao.type = 'button';
                            opcao.className = 'list-group-item list-group-item-action';
                            opcao.textContent = item.nome;
                            opcao.addEventListener('click', function () { adicionar(item); });
                            lista.appendChild(opcao);
                        });

                        lista.classList.remove('d-none');
                    })
                    .catch(function () { limparLista(); });
            }, 250);
        });

        document.addEventListener('click', function (evento) {
            if (!container.contains(evento.target)) {
                limparLista();
            }
        });

        var iniciais = container.getAttribute('data-autocomplete-selecionados');
        if (iniciais) {
            try {
                var parsed = JSON.parse(iniciais);
                if (Array.isArray(parsed)) {
                    selecionados = parsed;
                    renderTags();
                }
            } catch (e) {
                // ignora dados iniciais inválidos
            }
        }
    }

    document.querySelectorAll('[data-autocomplete]').forEach(inicializarAutocomplete);
})();

// Linhas dinâmicas do formulário de Datas de Lançamento (Jogos).
(function () {
    var container = document.querySelector('[data-datas-lancamento]');
    if (!container) return;

    var linhas = container.querySelector('[data-datas-linhas]');
    var botaoAdicionar = container.querySelector('[data-datas-adicionar]');

    function reindexar() {
        Array.prototype.forEach.call(linhas.children, function (linha, index) {
            linha.querySelectorAll('input[data-campo]').forEach(function (input) {
                input.name = 'DatasLancamento[' + index + '].' + input.getAttribute('data-campo');
            });
        });
    }

    function anexarRemocao(linha) {
        var botao = linha.querySelector('[data-datas-remover]');
        botao.addEventListener('click', function () {
            linha.remove();
            reindexar();
        });
    }

    function criarCampo(placeholder, campo, valor) {
        var input = document.createElement('input');
        input.type = 'text';
        input.className = 'form-control';
        input.placeholder = placeholder;
        input.setAttribute('data-campo', campo);
        input.value = valor || '';
        return input;
    }

    function criarLinha(regiao, data) {
        var linha = document.createElement('div');
        linha.className = 'row g-2 mb-2 align-items-center';

        var colRegiao = document.createElement('div');
        colRegiao.className = 'col-5';
        colRegiao.appendChild(criarCampo('Região', 'Regiao', regiao));

        var colData = document.createElement('div');
        colData.className = 'col-5';
        colData.appendChild(criarCampo('Data', 'Data', data));

        var colRemover = document.createElement('div');
        colRemover.className = 'col-2';
        var botaoRemover = document.createElement('button');
        botaoRemover.type = 'button';
        botaoRemover.className = 'btn btn-outline-danger';
        botaoRemover.setAttribute('data-datas-remover', '');
        botaoRemover.textContent = 'Remover';
        colRemover.appendChild(botaoRemover);

        linha.appendChild(colRegiao);
        linha.appendChild(colData);
        linha.appendChild(colRemover);

        return linha;
    }

    Array.prototype.forEach.call(linhas.children, anexarRemocao);

    botaoAdicionar.addEventListener('click', function () {
        var linha = criarLinha('', '');
        linhas.appendChild(linha);
        anexarRemocao(linha);
        reindexar();
    });
})();
