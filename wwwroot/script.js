let cursoSelecionado = '';
let periodoSelecionado = '';
let categoriaSelecionada = '';

function adicionarMensagem(texto, tipo) {
    const messages = document.getElementById('messages');
    const div = document.createElement('div');
    div.className = `message ${tipo}-message`;
    div.textContent = texto;
    messages.appendChild(div);
    messages.scrollTop = messages.scrollHeight;
}

function limparOpcoes() {
    document.getElementById('options').innerHTML = '';
}

function criarBotao(texto, onclick) {
    const button = document.createElement('button');
    button.textContent = texto;
    button.onclick = onclick;
    return button;
}

async function iniciarChat() {
    adicionarMensagem('Olá! Bem-vindo ao chatbot informativo da faculdade. Como posso ajudá-lo?', 'bot');
    mostrarMenuPrincipal();
}

function mostrarMenuPrincipal() {
    limparOpcoes();
    const options = document.getElementById('options');
    
    options.appendChild(criarBotao('Fazer uma pergunta', selecionarCurso));
    options.appendChild(criarBotao('Enviar ticket/dúvida', mostrarFormularioTicket));
}

async function selecionarCurso() {
    adicionarMensagem('Fazer uma pergunta', 'user');
    adicionarMensagem('Selecione seu curso:', 'bot');
    
    try {
        const response = await fetch('/api/chatbot/cursos');
        const cursos = await response.json();
        
        limparOpcoes();
        const options = document.getElementById('options');
        
        cursos.forEach(curso => {
            options.appendChild(criarBotao(curso, () => selecionarPeriodo(curso)));
        });
        
        options.appendChild(criarBotao('Voltar', mostrarMenuPrincipal));
    } catch (error) {
        adicionarMensagem('Erro ao carregar cursos. Tente novamente.', 'bot');
    }
}

async function selecionarPeriodo(curso) {
    cursoSelecionado = curso;
    adicionarMensagem(curso, 'user');
    adicionarMensagem('Selecione seu período:', 'bot');
    
    try {
        const response = await fetch(`/api/chatbot/periodos/${encodeURIComponent(curso)}`);
        const periodos = await response.json();
        
        limparOpcoes();
        const options = document.getElementById('options');
        
        periodos.forEach(periodo => {
            options.appendChild(criarBotao(periodo, () => mostrarPerguntas(periodo)));
        });
        
        options.appendChild(criarBotao('Voltar', selecionarCurso));
    } catch (error) {
        adicionarMensagem('Erro ao carregar períodos. Tente novamente.', 'bot');
    }
}



async function mostrarPerguntas(periodo) {
    periodoSelecionado = periodo;
    adicionarMensagem(periodo, 'user');
    adicionarMensagem('Selecione sua pergunta:', 'bot');
    
    try {
        const response = await fetch(`/api/chatbot/perguntas/${encodeURIComponent(cursoSelecionado)}/${encodeURIComponent(periodo)}`);
        const perguntas = await response.json();
        
        limparOpcoes();
        const options = document.getElementById('options');
        
        if (perguntas.length === 0) {
            adicionarMensagem('Nenhuma pergunta encontrada para este curso/período.', 'bot');
            options.appendChild(criarBotao('Voltar', () => selecionarPeriodo(cursoSelecionado)));
            return;
        }
        
        perguntas.forEach(pergunta => {
            options.appendChild(criarBotao(pergunta.texto, () => mostrarResposta(pergunta.id, pergunta.texto)));
        });
        
        options.appendChild(criarBotao('Voltar', () => selecionarPeriodo(cursoSelecionado)));
    } catch (error) {
        adicionarMensagem('Erro ao carregar perguntas. Tente novamente.', 'bot');
    }
}

async function mostrarResposta(perguntaId, perguntaTexto) {
    adicionarMensagem(perguntaTexto, 'user');
    
    try {
        const response = await fetch(`/api/chatbot/resposta/${perguntaId}`);
        const data = await response.json();
        
        adicionarMensagem(data.resposta, 'bot');
        
        limparOpcoes();
        const options = document.getElementById('options');
        
        options.appendChild(criarBotao('Fazer outra pergunta', () => mostrarPerguntas(periodoSelecionado)));
        options.appendChild(criarBotao('Menu principal', mostrarMenuPrincipal));
        
    } catch (error) {
        adicionarMensagem('Erro ao obter resposta. Tente novamente.', 'bot');
    }
}

async function mostrarFormularioTicket() {
    adicionarMensagem('Enviar ticket/dúvida', 'user');
    adicionarMensagem('Preencha o formulário abaixo para enviar sua dúvida:', 'bot');
    
    // Carregar cursos no select
    try {
        const response = await fetch('/api/chatbot/cursos');
        const cursos = await response.json();
        
        const select = document.getElementById('curso-ticket');
        select.innerHTML = '<option value="">Selecione seu curso</option>';
        
        cursos.forEach(curso => {
            const option = document.createElement('option');
            option.value = curso;
            option.textContent = curso;
            if (curso === cursoSelecionado) option.selected = true;
            select.appendChild(option);
        });
    } catch (error) {
        console.error('Erro ao carregar cursos:', error);
    }
    
    document.getElementById('chat-area').style.display = 'none';
    document.getElementById('ticket-form').style.display = 'block';
}

function toggleAnonimo() {
    const anonimo = document.getElementById('anonimo').checked;
    const dadosPessoais = document.getElementById('dados-pessoais');
    const campos = dadosPessoais.querySelectorAll('input');
    
    if (anonimo) {
        dadosPessoais.style.display = 'none';
        campos.forEach(campo => campo.required = false);
    } else {
        dadosPessoais.style.display = 'block';
        campos.forEach(campo => campo.required = true);
    }
}

async function enviarTicket() {
    const anonimo = document.getElementById('anonimo').checked;
    const nome = anonimo ? 'Anônimo' : document.getElementById('nome').value;
    const matricula = anonimo ? 'N/A' : document.getElementById('matricula').value;
    const email = anonimo ? 'N/A' : document.getElementById('email').value;
    const curso = document.getElementById('curso-ticket').value;
    const assunto = document.getElementById('assunto').value;
    const mensagem = document.getElementById('mensagem').value;
    
    if (!curso || !assunto || !mensagem) {
        alert('Por favor, preencha curso, assunto e mensagem.');
        return;
    }
    
    if (!anonimo && (!nome || !matricula || !email)) {
        alert('Por favor, preencha todos os campos pessoais ou marque como anônimo.');
        return;
    }
    
    const ticket = {
        aluno: nome,
        matricula: matricula,
        email: email,
        anonimo: anonimo,
        curso: curso,
        assunto: assunto,
        mensagem: mensagem
    };
    
    try {
        const response = await fetch('/api/chatbot/ticket', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(ticket)
        });
        
        const result = await response.json();
        
        voltarMenu();
        adicionarMensagem(`Ticket enviado com sucesso! ID: ${result.id}`, 'bot');
        adicionarMensagem('A faculdade entrará em contato em breve.', 'bot');
        
        // Limpa os campos
        document.getElementById('nome').value = '';
        document.getElementById('matricula').value = '';
        document.getElementById('email').value = '';
        document.getElementById('anonimo').checked = false;
        document.getElementById('dados-pessoais').style.display = 'block';
        document.getElementById('curso-ticket').value = '';
        document.getElementById('assunto').value = '';
        document.getElementById('mensagem').value = '';
        
    } catch (error) {
        alert('Erro ao enviar ticket. Tente novamente.');
    }
}

function voltarMenu() {
    document.getElementById('chat-area').style.display = 'block';
    document.getElementById('ticket-form').style.display = 'none';
    mostrarMenuPrincipal();
}

// Inicia o chat quando a página carrega
window.onload = iniciarChat;